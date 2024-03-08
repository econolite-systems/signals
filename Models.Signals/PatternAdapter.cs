// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Status.Signal;

namespace Econolite.Ode.Models.Signals;

public static class PatternAdapter
{
    const int defaultFreePat = 254;
    const int defaultFlashPat = 255;
    
    public static string ConvertToPatternString(this byte pattern, bool isCommsDead, UnitFlashStatus flashState, LocalFreeStatus localFreeStatus, byte freePattern = defaultFreePat, byte flashPattern = defaultFlashPat)
    {
        if (isCommsDead)
            return "?";

        if (pattern == 0 && localFreeStatus != LocalFreeStatus.NotFree)
        {
            // Do we need to suppress FREE with FLASH - PR 19125
            if (flashState == UnitFlashStatus.Automatic)
                return "PFLSH";
            else if (flashState != UnitFlashStatus.Mmu && flashState != UnitFlashStatus.LocalManual)
                return "FREE";
            else
                return "FLSH";
        }

        bool isProgrammed = flashState == UnitFlashStatus.Automatic;

        // Zero in ASC/3 is AUTO - this may need to change for different controller types
        if (pattern == 0)
            return "AUTO";

        // Try to get the controller's specific free and flash pattern numbers
        string txt;
        if (TryFreeOrFlash((int)pattern, out txt, freePattern, flashPattern, isProgrammed))
            return txt;

        return pattern.ToString();
    }

    /// <summary>
    /// Determines if the pattern is free or flash.
    /// </summary>
    /// <param name="signal"></param>
    /// <param name="pattern"></param>
    /// <param name="txt"></param>
    /// <returns></returns>
    internal static bool TryFreeOrFlash(int pattern, out string txt, byte freePattern, byte flashPattern, bool isProgrammed = false)
    {                
        // Check for free and flash states
        if (pattern == freePattern)
        {
            txt = "FREE";
            return true;
        }
        if (pattern == flashPattern)
        {
            if (isProgrammed)
                txt = "PFLSH";
            else
                txt = "FLSH";
            return true;
        }
            
        txt = string.Empty;
        return false;
    }
}
