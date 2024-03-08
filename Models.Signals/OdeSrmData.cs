// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.Signals;

public class OdeSrmData
{
    public OdeSrmMessage[] SrmMessageContent { get; set; } = Array.Empty<OdeSrmMessage>();
}

public class OdeSrmMessage
{
    public string Payload { get; set; } = string.Empty;
}
