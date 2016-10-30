using System;
using System.Collections.Generic;

namespace TypeTheory.CallByPushValue
{
    public static class Encoding
    {
        public static IEnumerable<T> Concatenate<T>(params IEnumerable<T>[] sequneces)
        {
            foreach (var sequence in sequneces)
            {
                foreach (var element in sequence)
                {
                    yield return element;
                }
            }
        }

        public static IEnumerable<Bits> EncodePolarity(Polarity polarity)
        {
            switch (polarity)
            {
                case Polarity.Forall:
                    yield return Bits.Zero;
                    yield break;
                case Polarity.Exists:
                    yield return Bits.One;
                    yield break;
                default:
                    throw new InvalidProgramException("Should never happen.");
            }
        }

        public static IEnumerable<Bits> EncodeNumber(uint number)
        {
            while (number != 0)
            {
                var digit = number % 2;

                yield return EncodeDigit(digit);

                number /= 2;
            }

            yield return Bits.Zero;
            yield break;
        }

        public static Bits EncodeDigit(uint digit)
        {
            switch (digit)
            {
                case 0:
                    return Bits.Zero;
                case 1:
                    return Bits.One;
                default:
                    throw new InvalidProgramException("Should never happen.");
            }
        }

        public static IEnumerable<Bits> EncodeNullable<T>(Func<T, IEnumerable<Bits>> encodeT, T? value)
            where T : struct
        {
            if (value.HasValue)
            {
                yield return Bits.One;

                foreach (var bit in encodeT(value.Value))
                {
                    yield return bit;
                }

                yield break;
            }

            yield return Bits.Zero;
        }
    }
}