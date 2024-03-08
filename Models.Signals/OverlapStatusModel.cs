// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.Signals;

public class OverlapStatusModel
{
    /// <summary>
    /// the phase number
    /// </summary>
    public string Overlap { get; set; }

    /// <summary>
    /// The extended phase state
    /// </summary>
    public ExtPhaseState ExtendedPhaseState { get; set; }

    /// <summary>
    /// The extended pedestrian state
    /// </summary>
    public ExtPedState ExtendedPedState { get; set; }

    /// <summary>
    /// Gets whether a phase is flashing. Not the same as tech or cabinet flash. 
    /// A phase can be red or yellow and be flashing.
    /// This is used for Canada or Tucson individual phase or overlap flashing.
    /// </summary>
    public bool IsOverlapFlash { get; set; }
}
