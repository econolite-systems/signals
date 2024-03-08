// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Status.Common;

namespace Econolite.Ode.Models.Signals;

public static class IsCommsDeadAdapter
{
    /// <summary>
    /// Defines which comm states indicate that the status data is unusable in the display.
    /// </summary>
    /// <param name="commStatus"></param>
    /// <returns></returns>
    public static bool ConvertToIsCommsDead(this CommStatus commStatus)
    {
        return commStatus != CommStatus.Good && commStatus != CommStatus.Marginal;
    }
}
