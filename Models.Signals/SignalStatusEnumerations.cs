// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.Signals;
//Note: pulled from Centracs - Econolite.Genesis.Common.Monitoring - SignalStatusEnums

/// <summary>
/// Possible states that are represented in the coordination section of the status display.
/// </summary>
public enum CoordState
{
    Unknown,

    /// <summary>
    /// In coordination.
    /// </summary>
    InSync,

    /// <summary>
    /// Attempting to get in sync.
    /// </summary>
    Transition,

    Flash,
    Free
}

/// <summary>
/// Available preemption sources.
/// </summary>
public enum PreemptKind
{
    /// <summary> No Active Preempt
    /// </summary>
    /// <value>0</value>
    None,

    /// <summary> Train Comming on Railroad crossing.
    /// </summary>
    /// <value>1</value>
    Railroad,

    /// <summary> Emergency Vehicle comming through intersection. Ambulance, Fire Engine, etc.
    /// </summary>
    /// <value>2</value>
    EmergencyVehicle,

    /// <summary> Other type of Preempt, such as Drawbridge up.
    /// </summary>
    /// <value>3</value>
    Other,

    /// <summary> An unsupported Preempt number
    /// </summary>
    /// <value>4</value>
    Unsupported
}
