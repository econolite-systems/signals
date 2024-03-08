// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Collections.Immutable;
using Econolite.Ode.Status.Signal;

namespace Econolite.Ode.Models.Signals;

public static class PreemptAdapter
{
    /// <summary>
    /// Given a list of preemptStates we return the type of preempt and the preempt number.
    /// No preempt = 0, Railroad are values 13-16. Emergency vehicles are 1-12. Other Preempts are 21-30.
    /// </summary>
    /// <param name="preemptStates"></param>
    /// <returns></returns>
    public static Preempt ConvertToPreempt(this ImmutableArray<PreemptState> preemptStates)
    {            
        byte preempt = 0;

        //Note:  pulled from Centracs - Device Manager - DeviceStatus - ToCentracs
        //Note:  Ray's SignalStatus object has the PreemptState as an ImmutableArray but we have ours as a List
        if (preemptStates != null && (!(preemptStates is ImmutableArray<PreemptState>) || !((ImmutableArray<PreemptState>)preemptStates).IsDefaultOrEmpty))
        {
            var idx = 0;
            foreach (var preemptState in preemptStates)
            {
                if (preemptState != PreemptState.NotActive && preemptState != PreemptState.NotActiveWithCall && preemptState != PreemptState.Unknown)
                {
                    preempt = (byte)(idx + 1);
                    break;
                }

                idx++;
            }
        }

        //Note:  pulled from Centracs - Econolite.Genesis.Common.Monitoring.ShortSignalStatus - ActivePreempt

        // Default to Unsupported.
        PreemptKind preemptKind = PreemptKind.Unsupported;

        // Get associated Preempt number.
        byte preemptNumber = preempt;
        byte preemptSpecificNumber = 0;

        // No active preempt
        if (preemptNumber == 0)
            preemptKind = PreemptKind.None;

        // Emergency Vehicle Preempt
        // 1 -- 12 (Corresponding to Emergency Vehicle preempts 1 -- 12)
        else if (preemptNumber >= 1 && preemptNumber <= 12)
        {
            preemptSpecificNumber = preemptNumber;
            preemptKind = PreemptKind.EmergencyVehicle;
        }

        // Railroad Preempt
        // 13 -- 16 (Corresponding to Railroad preempts 1 -- 4)
        else if (preemptNumber >= 13 && preemptNumber <= 16)
        {
            preemptSpecificNumber = (byte)(preemptNumber - 12);
            preemptKind = PreemptKind.Railroad;
        }

        // Other Preempt (Ex: Drawbridge Up)
        // 21 -- 30 (Corresponding to Other preempts 1 -- 10)
        else if (preemptNumber >= 21 && preemptNumber <= 30)
        {
            preemptSpecificNumber = (byte)(preemptNumber - 20);
            preemptKind = PreemptKind.Other;
        }

        return new Preempt(preemptKind, preemptSpecificNumber, preemptNumber);
    }
}
