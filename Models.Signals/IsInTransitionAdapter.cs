// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Status.Signal;

namespace Econolite.Ode.Models.Signals;

public static class IsInTransitionAdapter
{

    //Note:  the Centracs code seemed to vary if it was a MIB, Oasis or W4; this may need more work in the future
    //Note:  Per Ray should just need to check the UnitAlarmStatus2 field. Pulled from Centracs - DeviceManager.Impl.Devices.Signals.Polling.PollingContext - IsInTransition
    
    /// <summary>
    /// Returns whether or not a signal is in transition.
    /// </summary>
    /// <returns></returns>
    public static bool ConvertToIsInTransition(this UnitAlarmStatus2 unitAlarmStatus)
    {
        return unitAlarmStatus.HasFlag(UnitAlarmStatus2.OffsetTransitioning);
    }
}
