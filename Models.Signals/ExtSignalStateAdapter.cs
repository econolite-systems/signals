// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Status.Common;
using Econolite.Ode.Status.Signal;

namespace Econolite.Ode.Models.Signals;

public static class ExtSignalStateAdapter
{
    /// <summary>
    /// Converts the signal status data into an extendedSignalState enum which is used to determine the color of the status dot
    /// </summary>
    /// <param name="commStatus">the communication status of the signal</param>
    /// <param name="isCommsDead">whether or not we have comms to the signal</param>
    /// <param name="flashState">whether or not the signal is in flash and what state it is in</param>
    /// <param name="coordState">whether or not the signal is in coordination and what state it is in</param>
    /// <param name="preemptState">whether or not the signal has a preempt and which preempt it is in</param>
    /// <returns></returns>
    public static ExtSignalState ConvertToExtSignalState(this CommStatus commStatus, bool isCommsDead, UnitFlashStatus flashState, CoordState coordState, Preempt preemptState)
    {
        if (commStatus == CommStatus.Offline || commStatus == CommStatus.Unknown)
            return (ExtSignalState.Offline);
        if (commStatus == CommStatus.Standby)
            // Check standby before comm fail otherwise we'll only get comm fail never standby
            return (ExtSignalState.Standby);
        if (isCommsDead) //commStatus != CommStatus.Good && commStatus != CommStatus.Marginal;
            return (ExtSignalState.CommFail);
        //Note:  Handling this different than Centracs.  Exclude FlashStatus.None Centracs it is FlashState.Unknown and has seems to have more logic around its usage.
        if (flashState != UnitFlashStatus.NotFlash && flashState != UnitFlashStatus.Automatic && flashState != UnitFlashStatus.None) 
            return (ExtSignalState.Flash);
        if (flashState != UnitFlashStatus.NotFlash && flashState == UnitFlashStatus.Automatic)
            return (ExtSignalState.AutomaticFlash);
        if (preemptState.PreemptNumber != 0)
            return (ExtSignalState.Preempt);
        if (coordState == CoordState.Transition)
            return (ExtSignalState.Transition);
        if (coordState == CoordState.InSync)
            return (ExtSignalState.Coordinated);
        return (ExtSignalState.Free);
    }
}
