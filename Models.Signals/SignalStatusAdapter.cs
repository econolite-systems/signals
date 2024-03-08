// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Status.Common;
using Econolite.Ode.Status.Signal;

namespace Econolite.Ode.Models.Signals;

public static class SignalStatusAdapter
{
    public static IEnumerable<MapSignalState> ConvertToMapSignalState(this IEnumerable<Econolite.Ode.Status.Signal.SignalStatus> statuses)
    {
        return statuses.Select(ConvertToMapSignalState);
    }
    
    public static MapSignalState ConvertToMapSignalState(this Econolite.Ode.Status.Signal.SignalStatus status)
    {
        return new MapSignalState
        {
            Id = status.DeviceId,
            State = status.ConvertToSignalState(),
            Time = status.TimeStamp,
            SignalStatusSource = status.SignalStatusSource
        };
    }

    public static SignalState ConvertToSignalState(this SignalStatus s)
    {
        var state = SignalState.Free;

        if (s.CommStatus == CommStatus.Offline || s.CommStatus == CommStatus.Unknown)
        {
            state = SignalState.Offline;
        }
        else if (s.CommStatus == CommStatus.Standby)
        {
            state = SignalState.Standby;
        }
        else if (s.IsCommsDead())
        {
            state = SignalState.CommFail;
        }
        else if (s.UnitFlashStatus != UnitFlashStatus.NotFlash && s.UnitFlashStatus != UnitFlashStatus.Automatic &&
                 s.UnitFlashStatus != UnitFlashStatus.None)
        {
            state = SignalState.Flash;
        }
        else if (s.UnitFlashStatus != UnitFlashStatus.NotFlash && s.UnitFlashStatus == UnitFlashStatus.Automatic)
        {
            state = SignalState.AutomaticFlash;
        }
        else if (s.HasActivePreempt())
        {
            state = SignalState.Preempt;
        }
        else if (s.IsInTransition())
        {
            state = SignalState.Transition;
        }
        else if (s.AsCoordState() == CoordState.InSync)
        {
            state = SignalState.Coordination;
        }


        return state;
    }

    public static bool IsCommsDead(this SignalStatus s)
    {
        return s.CommSuccessRate == 0;
    }

    public static bool HasActivePreempt(this SignalStatus s)
    {
        return s.PreemptStates.Any(p =>
            p != PreemptState.NotActive && p != PreemptState.NotActiveWithCall && p != PreemptState.Unknown);
    }

    public static bool IsInTransition(this SignalStatus s)
    {
        return s.UnitAlarmStatus2.HasFlag(UnitAlarmStatus2.OffsetTransitioning);
    }

    public static CoordState AsCoordState(this SignalStatus s)
    {
        var state = CoordState.InSync;

        if (s.IsCommsDead() || s.UnitFlashStatus == UnitFlashStatus.None || s.LocalFreeStatus == LocalFreeStatus.None)
        {
            state = CoordState.Unknown;
        }
        else if (s.UnitFlashStatus != UnitFlashStatus.NotFlash)
        {
            state = CoordState.Flash;
        }
        else if (s.LocalFreeStatus != LocalFreeStatus.NotFree)
        {
            state = CoordState.Free;
        }
        else if (s.IsInTransition())
        {
            state = CoordState.Transition;
        }

        return state;
    }

    /// <summary>
    /// Converts a centracs status byte array into a signal status.  
    /// If getPhaseData is true then it will return all of the phase signal status data.
    /// </summary>
    /// <param name="status">Centracs byte array</param>
    /// <param name="getPhaseData">Whether or not to return each phase's status.</param>
    /// <param name="phasesInUse">For now we are assuming all 40 phases.  This is base 1 so the range is 1-40. In the future, we might want to get this data from Brandon's database editor.</param>
    /// <param name="statusTimeout"></param>
    public static SignalStatusModel ConvertToSignalStatus(this Econolite.Ode.Status.Signal.SignalStatus status,
        bool getPhaseData = false, int phasesInUse = 40, double statusTimeout = 0)
    {
        // Disabling coord active from alarms to ensure counts match the filtering Centracs has
        status.UnitAlarmStatus1 &= ~UnitAlarmStatus1.CoordActive;
        status.ShortAlarmStatus &= ~ShortAlarmStatus.CoordinationAlarm;

        bool oldStatus = statusTimeout > 0 && (DateTime.UtcNow - status.TimeStamp).TotalMinutes > statusTimeout;
        var adjustedCommStatus = status.CommSuccessRate > 100 || oldStatus ? CommStatus.Bad : status.CommStatus;
        var adjustedCommSuccessrate = status.CommSuccessRate > 100 || oldStatus ? 0 : status.CommSuccessRate;

        SignalStatusModel response = new SignalStatusModel
        {
            RingStatuses = status.RingStatuses.IsDefaultOrEmpty
                ? Array.Empty<RingStatus>()
                : status.RingStatuses.ToArray(),
            RingStatusTerminations = status.RingStatusTerminations.IsDefaultOrEmpty
                ? Array.Empty<RingStatusTermination>()
                : status.RingStatusTerminations.ToArray(),
            SignalId = status.DeviceId,
            CommStatus = adjustedCommStatus,
            CommSuccessRate = adjustedCommSuccessrate,
            IsCommsDead = adjustedCommStatus.ConvertToIsCommsDead(),
            LocalFreeStatus = status.LocalFreeStatus,
            UnitFlashStatus = status.UnitFlashStatus,
            IsInTransition = status.UnitAlarmStatus2.ConvertToIsInTransition(),
            PreemptState = status.PreemptStates.ConvertToPreempt(),
            UnitControlMode = status.UnitControl.ConvertToString(),
            AlarmCount =
                AlarmAggregator.GetCount(status.ShortAlarmStatus, status.UnitAlarmStatus1, status.UnitAlarmStatus2),
            Alarms = AlarmAggregator
                .GetAlarms(status.ShortAlarmStatus, status.UnitAlarmStatus1, status.UnitAlarmStatus2).OrderBy(a => a)
                .ToArray(),
            Timestamp = status.TimeStamp,
            LocalClock = status.LocalClock,
            SystemClock = status.SystemClock,
            Offset = status.Offset,
            TspCallStatus = status.TspCallStatus2s.IsDefaultOrEmpty
                ? Array.Empty<TspCallStatus2>()
                : status.TspCallStatus2s.ToArray(),
            SignalStatusSource = status.SignalStatusSource
        };

        response.CoordPattern = status.CoordPattern.ConvertToPatternString(response.IsCommsDead,
            response.UnitFlashStatus, response.LocalFreeStatus);
        response.CoordState = response.IsCommsDead.ConvertToCoordState(response.UnitFlashStatus,
            response.LocalFreeStatus, response.IsInTransition);
        response.ExtendedSignalState = response.CommStatus.ConvertToExtSignalState(response.IsCommsDead,
            response.UnitFlashStatus, response.CoordState, response.PreemptState);

        var phaseStatus = new List<PhaseStatusModel>();
        var overlapStatus = new List<OverlapStatusModel>();
        if (getPhaseData)
        {
            for (var i = 0; i <= phasesInUse - 1; i++)
            {
                phaseStatus.Add(status.ConvertToPhaseStatusResponse(i, response.IsCommsDead));
            }

            for (var i = 0; i < 16; i++)
            {
                overlapStatus.Add(status.ConvertToOverlapStatusResponse(i, response.IsCommsDead));
            }
        }

        response.PhaseStatus = phaseStatus.ToArray();
        response.OverlapStatus = overlapStatus.ToArray();


        if (!response.IsCommsDead && getPhaseData)
        {
            List<string> notificationStrings = new List<string>();

            // Check flash
            if (response.UnitFlashStatus != UnitFlashStatus.NotFlash)
            {
                switch (response.UnitFlashStatus)
                {
                    case UnitFlashStatus.Automatic:
                        notificationStrings.Add("Controller in Automatic Flash");
                        break;
                    case UnitFlashStatus.LocalManual:
                        notificationStrings.Add("Controller in Local Manual Flash");
                        break;
                    case UnitFlashStatus.Mmu:
                        notificationStrings.Add("MMU Flash Active");
                        break;
                    case UnitFlashStatus.Startup:
                        notificationStrings.Add("Controller in Startup Flash");
                        break;
                    case UnitFlashStatus.Preempt:
                        notificationStrings.Add("Controller in Preempt Flash");
                        break;
                    case UnitFlashStatus.FaultMonitor:
                        notificationStrings.Add("Controller in Fault Monitor Flash");
                        break;
                }
            }

            // Check free
            if (response.LocalFreeStatus != LocalFreeStatus.NotFree)
            {
                switch (response.LocalFreeStatus)
                {
                    case LocalFreeStatus.CommandFree:
                        notificationStrings.Add("Controller running command free");
                        break;
                    case LocalFreeStatus.TransitionFree:
                        notificationStrings.Add("Controller running transition free");
                        break;
                    case LocalFreeStatus.InputFree:
                        notificationStrings.Add("Controller running input free");
                        break;
                    case LocalFreeStatus.CoordFree:
                        notificationStrings.Add("Controller running coordination free");
                        break;
                    case LocalFreeStatus.BadPlan:
                        notificationStrings.Add("Controller running bad plan free");
                        break;
                    case LocalFreeStatus.BadCycleTime:
                        notificationStrings.Add("Controller running bad cycle free");
                        break;
                    case LocalFreeStatus.SplitOverrun:
                        notificationStrings.Add("Controller running split overrun free");
                        break;
                    case LocalFreeStatus.InvalidOffset:
                        notificationStrings.Add("Controller running invalid offset free");
                        break;
                }
            }

            // Check unit control fields (status byte 3)
            switch (status.UnitControl)
            {
                case UnitControl.Other:
                    notificationStrings.Add("Unit control state is 'Other'");
                    break;
                case UnitControl.SystemControl:
                    notificationStrings.Add("System control active");
                    break;
                case UnitControl.SystemStandby:
                    notificationStrings.Add("Unit control is in standby");
                    break;
                case UnitControl.BackupMode:
                    notificationStrings.Add("Unit control is in backup mode");
                    break;
                case UnitControl.Manual:
                    notificationStrings.Add("Unit control is in manual mode");
                    break;
                case UnitControl.TimeBase:
                    notificationStrings.Add("Unit control is in timebase mode");
                    break;
                case UnitControl.Interconnect:
                    notificationStrings.Add("Unit control is in interconnect mode");
                    break;
                case UnitControl.InterconnectBackup:
                    notificationStrings.Add("Unit control is in interconnect backup mode");
                    break;
            }

            // Check status byte 5
            if (response.CommStatus == CommStatus.Offline)
            {
                notificationStrings.Add("Comm is set to Offline");
            }
            else if (response.CommStatus == CommStatus.Standby)
                notificationStrings.Add("Polling is set to Standby");

            // Use user defined special function names
            for (int i = 0; i < 8; i++)
            {
                if (((status.SpecialFunctionStatus >> i) & 1) > 0)
                {
                    notificationStrings.Add($"Special Function: {i + 1} - ON");
                }
            }

            // Handle all of the bit-mapped alarms
            if (response.Alarms.Any())
            {
                foreach (var alarm in (ShortAlarmStatus[]) System.Enum.GetValues(typeof(ShortAlarmStatus)))
                {
                    if ((status.ShortAlarmStatus & alarm) == 0)
                        continue;

                    switch (alarm)
                    {
                        // Short Alarm Status
                        case ShortAlarmStatus.Preempt:
                            notificationStrings.Add("Alarm: Preempt");
                            break;
                        case ShortAlarmStatus.TandFFlash:
                            notificationStrings.Add("Alarm: T and F Flash");
                            break;
                        case ShortAlarmStatus.LocalZero:
                            notificationStrings.Add("Alarm: Local Zero");
                            break;
                        case ShortAlarmStatus.LocalOverride:
                            notificationStrings.Add("Alarm: Local Override");
                            break;
                        case ShortAlarmStatus.DetectorFaulted:
                            notificationStrings.Add("Alarm: Detector Faulted");
                            break;
                        case ShortAlarmStatus.NonCriticalAlarm:
                            notificationStrings.Add("Alarm: Non Critical Alarm");
                            break;
                        case ShortAlarmStatus.CriticalAlarm:
                            notificationStrings.Add("Alarm: Critical Alarm");
                            break;
                    }
                }

                foreach (var alarm in (UnitAlarmStatus1[]) System.Enum.GetValues(typeof(UnitAlarmStatus1)))
                {
                    if ((status.UnitAlarmStatus1 & alarm) == 0)
                        continue;

                    switch (alarm)
                    {
                        // Unit Alarm Status 1
                        case UnitAlarmStatus1.CycleFault:
                            notificationStrings.Add("Alarm: Cycle Fault");
                            break;
                        case UnitAlarmStatus1.CoordFault:
                            notificationStrings.Add("Alarm: Coord Fault");
                            break;
                        case UnitAlarmStatus1.CoordFail:
                            notificationStrings.Add("Alarm: Coord Fail");
                            break;
                        case UnitAlarmStatus1.CycleFail:
                            notificationStrings.Add("Alarm: Cycle Fail");
                            break;
                        case UnitAlarmStatus1.MMUFlash:
                            notificationStrings.Add("Alarm: MMU Flash");
                            break;
                        case UnitAlarmStatus1.LocalFlash:
                            notificationStrings.Add("Alarm: Local Flash");
                            break;
                    }
                }

                foreach (var alarm in (UnitAlarmStatus2[]) System.Enum.GetValues(typeof(UnitAlarmStatus2)))
                {
                    if ((status.UnitAlarmStatus2 & alarm) == 0)
                        continue;

                    switch (alarm)
                    {
                        // Unit Alarm Status 2
                        case UnitAlarmStatus2.PowerRestart:
                            notificationStrings.Add("Alarm: Power Restart");
                            break;
                        case UnitAlarmStatus2.LowBattery:
                            notificationStrings.Add("Alarm: Low Battery");
                            break;
                        case UnitAlarmStatus2.ResponseFault:
                            notificationStrings.Add("Alarm: Response Fault");
                            break;
                        case UnitAlarmStatus2.ExternalStart:
                            notificationStrings.Add("Alarm: External Start");
                            break;
                        case UnitAlarmStatus2.StopTime:
                            notificationStrings.Add("Alarm: Stop Time");
                            break;
                        case UnitAlarmStatus2.OffsetTransitioning:
                            notificationStrings.Add("Alarm: Offset Transitioning");
                            break;
                    }
                }
            }

            // TSP
            if (response.TspCallStatus.Any())
            {
                for (var i = 0; i < response.TspCallStatus.Length; i++)
                {
                    var tspResp = response.TspCallStatus[i];
                    switch (tspResp)
                    {
                        case TspCallStatus2.CallFromInput:
                            notificationStrings.Add($"TSP {i + 1} call from input");
                            break;
                        case TspCallStatus2.CallFromNTCIP:
                            notificationStrings.Add($"TSP {i + 1} call from NTCIP");
                            break;
                        case TspCallStatus2.CallBeingServed:
                            notificationStrings.Add($"TSP {i + 1} call being serviced");
                            break;
                        case TspCallStatus2.CallReserviced:
                            notificationStrings.Add($"TSP {i + 1} call being reserviced");
                            break;
                        case TspCallStatus2.CallInhibited:
                            notificationStrings.Add($"TSP {i + 1} call inhibited");
                            break;
                        case TspCallStatus2.Disabled:
                            notificationStrings.Add($"TSP {i + 1} disabled");
                            break;
                        case TspCallStatus2.ProgrammingError:
                            notificationStrings.Add($"TSP {i + 1} programming error");
                            break;
                        case TspCallStatus2.EarlyExtendedGreen:
                            notificationStrings.Add($"TSP {i + 1} extended early green");
                            break;
                    }
                }
            }

            response.Notifications = notificationStrings.ToArray();
        }

        return response;
    }
}
