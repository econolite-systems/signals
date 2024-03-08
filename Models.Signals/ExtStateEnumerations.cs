// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.Signals;

/// <summary>
/// Extended phase states used by the map.
/// </summary>
public enum ExtPhaseState
{
    CommFail,
    Off,
    Green,
    Yellow,
    Red,
    Orange
}

/// <summary>
/// Extended signal state used by the map.
/// </summary>
public enum ExtSignalState
{
    Offline,
    CommFail,
    Standby,
    Flash,
    Preempt,
    Transition,
    Coordinated,
    Free,
    AutomaticFlash
}

/// <summary>
/// Main/Side Street Green signal states used by the map.
/// </summary>
public enum ExtMSGSSGState
{
    Offline,
    CommFail,
    Off,
    Standby,
    Flash,
    Green,
    Red,
    AutomaticFlash
}

/// <summary>
/// States that a pedestrian phase can be in.
/// </summary>
public enum ExtPedState
{
    CommFail,
    Off,
    Walk,

    /// <summary>
    /// Ped Clear, aka Flashing Don't Walk
    /// </summary>
    FDW,

    /// <summary>
    /// Don't walk
    /// </summary>
    DW,

    /// <summary>
    /// Walk, DW, FDW all off, Dark
    /// </summary>
    Dark,
}

/// <summary>
/// States that Opacity can be in.
/// </summary>
public enum ExtOpacityState
{
    Clear,
    Opacity25,
    Opacity50,
    Opacity75,
    Opaque
}

/// <summary>
/// Extended phase states used by the map.
/// </summary>
public enum ExtPGPhaseState
{
    Unknown,
    CommFail,
    Off,
    Green,
    Yellow,
    Red,
    FlashGreen,
    FlashYellow,
    Orange
}
