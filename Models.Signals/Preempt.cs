// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.Signals;

/// Note:  pulled from Centracs -  Econolite.Genesis.Common.Monitoring - PreemptState
/// Note:  change the name from preemptState to preempt so it won't be confused with Econolite.Ode.Status.Signal.PreemptState
/// <summary>
/// Represents a preempt, i.e. its type (RR or EV) and the number.  This is used in signal status.
/// </summary>
public class Preempt
{
    public PreemptKind PreemptType { get; set; }

    /// <summary>
    /// The preempt number starting at 1.
    /// </summary>
    public int PreemptNumber { get; set; }

    public int PreemptCentracsNumber { get; set; }

    public Preempt() { }
    public Preempt(PreemptKind type, int number, int numberCentracs)
    {
        PreemptType = type;
        PreemptNumber = number;
        PreemptCentracsNumber = numberCentracs;
    }

    public override int GetHashCode()
    {
        return PreemptNumber.GetHashCode() + PreemptCentracsNumber.GetHashCode() * 17 + PreemptType.GetHashCode() * 23;
    }

    public override bool Equals(object obj)
    {
        bool same = false;

        if (obj != null)
        {
            Preempt other = obj as Preempt;

            if (!ReferenceEquals(other, null))
            {
                same = (other.PreemptType == PreemptType) &&
                       (other.PreemptNumber == PreemptNumber) &&
                       (other.PreemptCentracsNumber == PreemptCentracsNumber);

                return same;
            }
        }

        return same;
    }

    public static bool operator == (Preempt a, Preempt b)
    {
        if (!ReferenceEquals(a, null))
            return a.Equals(b);
        else if (!ReferenceEquals(b, null))
            return b.Equals(a);
        else
            return true;
    }

    public static bool operator != (Preempt a, Preempt b)
    {
        return !(a == b);
    }

}
