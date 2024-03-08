// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Status.Signal;

namespace Econolite.Ode.Models.Signals;

public static class UnitControlModeAdapter
{

    public static string ConvertToString(this UnitControl mode)
    {
        switch (mode)
        {
            case UnitControl.Unknown:
                return "?";
            case UnitControl.BackupMode:
                return "BM";
            case UnitControl.Interconnect:
                return "IC";
            case UnitControl.InterconnectBackup:
                return "ICB";
            case UnitControl.Manual:
                return "MAN";
            case UnitControl.Other:
                return "DFLT";
            case UnitControl.SystemControl:
                return "SYS";
            case UnitControl.SystemStandby:
                return "STBY";
            case UnitControl.TimeBase:
                return "TOD";                    
        }
        return string.Empty;
    }
}
