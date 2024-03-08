// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.Signals;

/// <summary>
/// The status for each individual phase
/// </summary>
public class PhaseStatusModel
{
    /// <summary>
    /// the phase number
    /// </summary>
    public int Phase { get; set; }

    /// <summary>
    /// The extended phase state
    /// </summary>
    public ExtPhaseState ExtendedPhaseState { get; set; }

    /// <summary>
    /// The extended pedestrian state
    /// </summary>
    public ExtPedState ExtendedPedState { get; set; }

    /// <summary>
    /// Whether or not this is the next phase in the cycle
    /// </summary>
    public bool IsNextPhase { get; set; }

    /// <summary>
    /// Whether or not there is a vehicle call
    /// </summary>
    public bool IsVehCall { get; set; }

    /// <summary>
    /// Whether or not there is a pedestrian call
    /// </summary>
    public bool IsPedCall { get; set; }

    /// <summary>
    /// Gets whether a phase is flashing. Not the same as tech or cabinet flash. 
    /// A phase can be red or yellow and be flashing.
    /// This is used for Canada or Tucson individual phase or overlap flashing.
    /// </summary>
    public bool IsPhaseFlash { get; set; }
}
