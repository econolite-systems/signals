// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Domain.Signals;

public class SignalS2SOptions
{
    public string Uri { get; set; } = "";
    public int Retries { get; set; } = 5;
}
