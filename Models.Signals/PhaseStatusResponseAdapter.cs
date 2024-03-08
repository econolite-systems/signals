// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Status.Signal;

namespace Econolite.Ode.Models.Signals;

public static class PhaseStatusResponseAdapter
{
    /// <summary>
    /// Creates an array of the status details for each phase in use.
    /// </summary>
    /// <param name="status">Byte array that is saved to the redis cache and comes from the device manager</param>
    /// <param name="phase">The phase number that corresponds to the position in the byte array. Phases are base 0 (0-39) in the byte arrays</param>
    public static PhaseStatusModel ConvertToPhaseStatusResponse(this Econolite.Ode.Status.Signal.SignalStatus status, int phase, bool isCommsDead)
    {
        PhaseStatusModel response = new PhaseStatusModel();
            
        //set flash first because those are needed in other conversions
        response.IsPhaseFlash = BoolAdapter.ConvertToBool(phase, status.PhaseFlash) || (status.UnitFlashStatus != UnitFlashStatus.None && status.UnitFlashStatus != UnitFlashStatus.NotFlash);

        response.Phase = PhaseAdapter.ConvertToPhase(phase);
        response.ExtendedPhaseState = ExtPhaseStateAdapter.ConvertToExtPhaseState(phase, status.PhaseGreen, status.PhaseYellow, status.PhaseRed, isCommsDead);
        response.ExtendedPedState = ExtPedStateAdapter.ConvertToExtPedState(phase, status.Walk, status.PedClearance, status.DontWalk, isCommsDead);
        response.IsNextPhase = BoolAdapter.ConvertToBool(phase, status.PhaseNext);
        response.IsVehCall = BoolAdapter.ConvertToBool(phase, status.VehCalls);
        response.IsPedCall = BoolAdapter.ConvertToBool(phase, status.PedCalls);

        return response;
    }

    public static OverlapStatusModel ConvertToOverlapStatusResponse(this Econolite.Ode.Status.Signal.SignalStatus status, int overlap, bool isCommsDead)
    {
        OverlapStatusModel response = new OverlapStatusModel();

        //set flash first because those are needed in other conversions
        response.IsOverlapFlash = BoolAdapter.ConvertToBool(overlap, status.OverlapFlash) || (status.UnitFlashStatus != UnitFlashStatus.None && status.UnitFlashStatus != UnitFlashStatus.NotFlash);

        response.Overlap = PhaseAdapter.ConvertToOverlap(overlap);
        response.ExtendedPhaseState = ExtPhaseStateAdapter.ConvertToExtPhaseState(overlap, status.OverlapGreen, status.OverlapYellow, status.OverlapRed, isCommsDead);
        response.ExtendedPedState = ExtPedStateAdapter.ConvertToExtPedState(overlap, status.OverlapWalk, status.OverlapPedClearance, status.OverlapDontWalk, isCommsDead);

        return response;
    }
}
