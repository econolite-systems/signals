// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Status.Signal;

namespace Econolite.Ode.Models.Signals;

public class MapStatusModel
{
    public SignalStatusSource SignalStatusSource { get; set; }

    public string State { get; set; }


}
