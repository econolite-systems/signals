// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Status.Signal;
// ReSharper disable HeapView.BoxingAllocation
namespace Econolite.Ode.Models.Signals;

public static class AlarmAggregator
{
    private static int GetCount(this ShortAlarmStatus shortAlarmStatus)
    {
        return GetCountEnum(shortAlarmStatus, true);
    }

    private static IEnumerable<string> List(this ShortAlarmStatus shortAlarmStatus)
    {
        return GetStatuses(shortAlarmStatus);
    }

    private static int GetCount(this UnitAlarmStatus1 unitAlarmStatus1)
    {
        //remote localFree; shouldn't show as an alarm
        return GetCountEnum(unitAlarmStatus1 & ~UnitAlarmStatus1.LocalFree);
    }

    private static IEnumerable<string> List(this UnitAlarmStatus1 unitAlarmStatus1)
    {
        //remote localFree; shouldn't show as an alarm
        return GetStatuses(unitAlarmStatus1 & ~UnitAlarmStatus1.LocalFree);
    }

    private static int GetCount(this UnitAlarmStatus2 unitAlarmStatus2)
    {
        return GetCountEnum(unitAlarmStatus2);
    }

    private static IEnumerable<string> List(this UnitAlarmStatus2 unitAlarmStatus2)
    {
        return GetStatuses(unitAlarmStatus2);
    }

    public static int GetCount(ShortAlarmStatus shortAlarmStatus, UnitAlarmStatus1 unitAlarmStatus1, UnitAlarmStatus2 unitAlarmStatus2)
    {
        return shortAlarmStatus.GetCount() + unitAlarmStatus1.GetCount() + unitAlarmStatus2.GetCount();
    }

    public static IEnumerable<string> GetAlarms(ShortAlarmStatus shortAlarmStatus, UnitAlarmStatus1 unitAlarmStatus1, UnitAlarmStatus2 unitAlarmStatus2)
    {
        List<string> alarms = new List<string>();
        alarms.AddRange(shortAlarmStatus.List());
        alarms.AddRange(unitAlarmStatus1.List());
        alarms.AddRange(unitAlarmStatus2.List());
        return alarms;
    }

    private static int GetCountEnum(Enum status, bool removeNoneEnum = false)
    {
        int count = Enum.GetValues(status.GetType()).Cast<Enum>().Count(status.HasFlag);
        if (removeNoneEnum && count > 0) count--;
        return count;
    }

    private static IEnumerable<string> GetStatuses(Enum status)
    {
        return Enum.GetValues(status.GetType()).Cast<Enum>()
            .Where(@enum => status.HasFlag(@enum) && @enum.ToString() != "None")
            .Select(@enum => @enum.ToString());
    }
}
