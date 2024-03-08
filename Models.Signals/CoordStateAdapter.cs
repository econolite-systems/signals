// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Status.Signal;

namespace Econolite.Ode.Models.Signals;

public static class CoordStateAdapter
{
    //Note:  pulled from Centracs - ShortSignalStatusViewModel - UpdateCoordState
    //Note:  using the DM's LocalFreeStatus enum instead of the Centracs' FreeState.  They are pretty much the same.
    
    /// <summary>
    /// Converts signal status to a coordination state
    /// </summary>
    /// <param name="isCommsDead">whether or not we have comms to the signal</param>
    /// <param name="flashState">whether or not the signal is in flash and what state it is in</param>
    /// <param name="freeState">whether or not the signal is in free and what state it is in</param>
    /// <param name="isInTransition">whether or not the signal is in transition</param>
    /// <returns></returns>
    public static CoordState ConvertToCoordState(this bool isCommsDead, UnitFlashStatus flashState, LocalFreeStatus freeState, bool isInTransition)
    {

        CoordState coordState;
         
        if (isCommsDead || flashState == UnitFlashStatus.None || freeState == LocalFreeStatus.None) //the equivalent of Centracs' FlashState.Unknown and FreeState.Unknown
            coordState = CoordState.Unknown;

        // Check for flash
        else if (flashState != UnitFlashStatus.NotFlash)
            coordState = CoordState.Flash;

        // Check for free
        else if (freeState != LocalFreeStatus.NotFree)
            coordState = CoordState.Free;

        // Check for transition
        else if (isInTransition)
            coordState = CoordState.Transition;

        // Otherwise coordinated
        else
            coordState = CoordState.InSync;

        return coordState;
    }
}
