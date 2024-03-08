// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.Signals;

public class IndexRequest
{
    public bool GetPhaseData { get; set; }

    public IEnumerable<Guid> SignalIds { get; set; } = new List<Guid>();
}
