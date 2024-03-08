// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.Signals;

public class ExtPhaseStateAdapter
{
    /// <summary>
    /// Checks to see if the phase is turned on for red, green, or yellow in that order. Returns the appropriate enum value.  
    /// Checks the byte array for the given phase index position and sees if the bit is turned on.
    /// </summary>
    /// <param name="phase">the specified phase to convert; centracs byte array is zero based; 0-39 phase index</param>
    /// <param name="phaseGreen">the green data for all phasees in byte array form</param>
    /// <param name="phaseYellow">the yellow data for all phasees in byte array form</param>
    /// <param name="phaseRed">the red data for all phasees in byte array form</param>
    /// <param name="isCommsDead">whether or not we have comms to the signal</param>
    /// <returns></returns>
    public static ExtPhaseState ConvertToExtPhaseState(int phase, ulong phaseGreen, ulong phaseYellow, ulong phaseRed, bool isCommsDead)
    {
        if (isCommsDead)
            return ExtPhaseState.CommFail;

        //Note:  I don't think we are concerned with flash because we will still want color - Ex: flashing red vs. flashing yellow
        ExtPhaseState s = ExtPhaseState.Off;

        if (PhaseIsRed(phase, phaseRed))
            s = ExtPhaseState.Red;
        else if (PhaseIsGreen(phase, phaseGreen))
            s = ExtPhaseState.Green;
        else if (PhaseIsYellow(phase, phaseYellow))
            s = ExtPhaseState.Yellow;

        return s;
    }

    private static bool PhaseIsGreen(int phase, ulong phaseGreen)
    {
        return BoolAdapter.ConvertToBool(phase, phaseGreen);
    }

    private static bool PhaseIsYellow(int phase, ulong phaseYellow)
    {
        return BoolAdapter.ConvertToBool(phase, phaseYellow);
    }

    private static bool PhaseIsRed(int phase, ulong phaseRed)
    {
        return BoolAdapter.ConvertToBool(phase, phaseRed);
    }
}
