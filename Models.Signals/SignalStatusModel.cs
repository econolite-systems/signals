// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Status.Common;
using Econolite.Ode.Status.Signal;

namespace Econolite.Ode.Models.Signals;

public class SignalStatusModel { 

    /// <summary>
    /// The spm signal id.
    /// </summary>
    public Guid SignalId { get; set; }

    /// <summary>
    /// The level of comms we have with the signal
    /// </summary>
    public CommStatus CommStatus { get; set; }

    /// <summary>
    /// The percentage of our communications successes
    /// </summary>
    public int CommSuccessRate { get; set; }

    /// <summary>
    /// Extended comms state - includes extra states like flash, preempt, etc.
    /// </summary>
    public ExtSignalState ExtendedSignalState { get; set; }

    /// <summary>
    /// Whether or not the controller is in transition (offset seeking)
    /// </summary>
    public bool IsInTransition { get; set; }

    /// <summary>
    /// Whether or not we have comms
    /// </summary>
    public bool IsCommsDead { get; set; }

    /// <summary>
    /// Indicates whether the signal is in Free and for what reason.  The Centracs UI uses FreeState but the enums are essentially the same.
    /// </summary>
    public LocalFreeStatus LocalFreeStatus { get; set; }

    /// <summary>
    /// Whether or not the controller is in flash and if so which type of flash.    The Centracs UI uses FlashState but the enums are essentially the same.
    /// </summary>
    public UnitFlashStatus UnitFlashStatus { get; set; }

    /// <summary>
    /// The coordination state
    /// </summary>
    public CoordState CoordState { get; set; }

    /// <summary>
    /// Whether or not the signal has a preempt and if so what preempt it is
    /// </summary>
    public Preempt PreemptState { get; set; } = new();

    /// <summary>
    /// Signal's name
    /// </summary>
    public string SignalName { get; set; } = string.Empty;

    /// <summary>
    /// The unit control state.
    /// </summary>
    public string UnitControlMode { get; set; } = string.Empty;

    /// <summary>
    /// The current pattern or state the signal is operating.
    /// </summary>
    public string CoordPattern { get; set; } = string.Empty;

    /// <summary>
    /// The current alarm count.
    /// </summary>
    public int AlarmCount { get; set; }

    /// <summary>
    /// Array of alarms.
    /// </summary>
    public string[] Alarms { get; set; } = Array.Empty<string>();

    /// <summary>
    /// A collection of status data for each phase that is in use. Only populated is the api request has getPhaseData = true
    /// </summary>
    public PhaseStatusModel[] PhaseStatus { get; set; } = Array.Empty<PhaseStatusModel>();

    /// <summary>
    /// A collection of overlap status data for each overlap that is in use. Only populated is the api request has getOverlapData = true
    /// </summary>
    public OverlapStatusModel[] OverlapStatus { get; set; } = Array.Empty<OverlapStatusModel>();

    /// <summary>
    /// A collection of Ring Statuses
    /// </summary>
    public RingStatus[] RingStatuses { get; set; } = Array.Empty<RingStatus>();

    public RingStatusTermination[] RingStatusTerminations { get; set; } = Array.Empty<RingStatusTermination>();

    public DateTime Timestamp { get; set; }

    public ushort SystemClock { get; set; }

    public ushort LocalClock { get; set; }

    public byte Offset { get; set; }

    public TspCallStatus2[] TspCallStatus { get; set; } = Array.Empty<TspCallStatus2>();

    public string[] Notifications { get; internal set; } = Array.Empty<string>();

    public SignalStatusSource SignalStatusSource { get; set; }
}
