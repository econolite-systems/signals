// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Status.Common;
using Econolite.Ode.Status.Signal;

namespace Api.Signals.Extensions
{
    public static class SignalStatusControllerExtensions
    {
        public static void CheckTimeout(this SignalStatus signalStatus, int timeout)
        {
            if (signalStatus.TimeStamp < DateTime.UtcNow.AddMinutes(-timeout))
            {
                signalStatus.CommStatus = CommStatus.Offline;
            }
        }
    }
}
