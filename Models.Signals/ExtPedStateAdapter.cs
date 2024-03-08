// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.Signals;

public class ExtPedStateAdapter
{

    #region public methods

    /// <summary>
    /// Checks to see if the phase is turned on for walk, pedClearance or dontWalk in that order. Returns the appropriate enum value.  
    /// Checks the byte array for the given phase index position and sees if the bit is turned on.
    /// </summary>
    /// <param name="phase">the specified phase to convert; centracs byte array is zero based; 0-39 phase index</param>
    /// <param name="walk">the walk data for all phasees in byte array form</param>
    /// <param name="pedClearance">the pedClearance data for all phasees in byte array form</param>
    /// <param name="dontWalk">the dontWalk data for all phasees in byte array form</param>
    /// <returns></returns>
    public static ExtPedState ConvertToExtPedState(int phase, ulong walk, ulong pedClearance, ulong dontWalk, bool isCommsDead)
    {
        if (isCommsDead)
            return ExtPedState.CommFail;

        ExtPedState returnValue;

        if (PedPhaseIsWalk(phase, walk))
            returnValue = ExtPedState.Walk;
        else if (PedPhaseIsClear(phase, pedClearance))
            returnValue = ExtPedState.FDW;
        else if (PedPhaseIsDontWalk(phase, dontWalk))
            returnValue = ExtPedState.DW;
        else
            returnValue = ExtPedState.Dark;

        return returnValue;
    }

    #endregion

    #region private methods

    private static bool PedPhaseIsWalk(int phase, ulong walk)
    {
        return BoolAdapter.ConvertToBool(phase, walk); 
    }

    private static bool PedPhaseIsClear(int phase, ulong pedClearance)
    {
        return BoolAdapter.ConvertToBool(phase, pedClearance);
    }

    private static bool PedPhaseIsDontWalk(int phase, ulong dontWalk)
    {
        return BoolAdapter.ConvertToBool(phase, dontWalk);
    }

    #endregion

}
