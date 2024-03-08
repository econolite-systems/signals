// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.Signals;

public class OdeSpatData
{
    public OdeSpatMessage[] SpatMessageContent { get; set; } = Array.Empty<OdeSpatMessage>();
}

public class OdeSpatMessage
{
    public string Payload { get; set; } = string.Empty;
}
