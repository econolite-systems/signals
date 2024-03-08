// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.Signals;

public class OdeSsmData
{
    public OdeSsmMessage[] SsmMessageContent { get; set; } = Array.Empty<OdeSsmMessage>();
}

public class OdeSsmMessage
{
    public string Payload { get; set; } = string.Empty;
}
