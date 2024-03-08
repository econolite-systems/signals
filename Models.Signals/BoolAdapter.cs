// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.Signals.Utilities;

namespace Econolite.Ode.Models.Signals;

public class BoolAdapter
{
    /// <summary>
    /// Looks up a given phase in the byte array to see if the bit is turned on or not.
    /// </summary>
    /// <param name="phase">the specified phase to look up; centracs byte array is zero based; 0-39 phase index</param>
    /// <param name="boolByteArray"></param>
    /// <returns></returns>
    public static bool ConvertToBool(int phase, ulong boolByteArray)
    {
        return BitMapFlag.IsBitOn(boolByteArray, phase);
    }
}
