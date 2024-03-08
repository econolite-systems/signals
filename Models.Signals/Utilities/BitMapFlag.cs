// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.Signals.Utilities;

public static class BitMapFlag
{
    /// <summary>
    /// Checks bit in <see cref="ulong"/> to see if it is 'set' or on, or == 1.
    /// </summary>
    /// <param name="data"><see cref="ulong"/> to be inspected.</param>
    /// <param name="bit">Zero-based bit number between 0 and 63 inclusive.</param>
    /// <exception cref="ArgumentOutOfRangeException">Throws if argument bit is less than 0 or greater than 63</exception>
    /// <returns>True if bit is 1, false if 0.</returns>
    public static bool IsBitOn(ulong data, int bit)
    {
        if (bit < 0 || bit > 63)
            throw new ArgumentOutOfRangeException(nameof(IsBitOn), "Value must be between 0 and 63 inclusive.");

        ulong mask = (ulong)((ulong)0x1 << bit);
        return Convert.ToBoolean(data & mask);
    }

    /// <summary>
    /// Sets a given bit on or off based on isOn argument.
    /// </summary>
    /// <param name="data"><see cref="ushort"/> to have it's bit set.</param>
    /// <param name="bit">Zero-based bit number between 0 and 39 inclusive.</param>
    /// <param name="isOn">State to which to set the bit, on or off.</param>
    /// <exception cref="ArgumentOutOfRangeException">Throws if argument bit is less than 0 or greater than 15</exception>
    public static void SetBit(ref ulong data, int bit, bool isOn)
    {
        if (bit < 0 || bit > 39)
            throw new ArgumentOutOfRangeException(nameof(SetBit), "Value must be between 0 and 39 inclusive.");

        ulong mask = (ulong)(0x1 << bit);
        if (isOn)
            data |= mask;
        else
            data &= (ulong)~mask;
    }
}
