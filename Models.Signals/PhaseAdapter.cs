// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.Signals;

public class PhaseAdapter
{
    public static int ConvertToPhase(int phase)
    {
        //raw data is base 0; for the ui we want base 1
        return (phase + 1);
    }

    public static string ConvertToOverlap(int overlap)
    {
        //raw data is base 0; for the ui we want base 1
        return ((char)(overlap + 'A')).ToString();
    }
}
