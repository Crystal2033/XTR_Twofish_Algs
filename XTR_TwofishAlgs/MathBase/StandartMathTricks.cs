using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XTR_TwofishAlgs.MathBase
{
    public static class StandartMathTricks
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static int GetHighest2DegreeValue(uint value)
        {
            int degree = 0;
            while (value != 1)
            {
                value >>= 1;
                degree++;
            }
            return degree;
        }

        public static int GetHighest2DegreeValue(BigInteger value)
        {
            int degree = 0;
            while (value != 1)
            {
                value >>= 1;
                degree++;
            }
            return degree;
        }

        // производится k раундов проверки числа n на простоту
        public static bool FermaTestIsPrime(BigInteger n, int k)
        {
            // если n == 2 или n == 3 - эти числа простые, возвращаем true
            if (n == 2 || n == 3)
                return true;

            // если n < 2 или n четное - возвращаем false
            if (n < 2 || n % 2 == 0)
                return false;

            for(int i = 0; i < k; i++)
            {
                using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                {
                    BigInteger a;
                    do
                    {
                        a = RandomInRange(rng, 2, n - 1);
                    } while (Euclid.ExtendedGcd(a, n).nod != 1);
                    
                    if(BigInteger.ModPow(a, n-1, n) != 1)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static (BigInteger p, BigInteger q) GeneratePQ(int minSizePBits, int minSizeQBits)
        {
            if(minSizePBits < minSizeQBits)
            {
                return GeneratePQ(minSizeQBits, minSizePBits);
            }
            BigInteger minValueForQ = BigInteger.Pow(2, minSizeQBits / 2 - 1); //Because in q gen r*r. Order*2
            _log.Info($"minValueForQ = {minValueForQ.GetBitLength()}");

            BigInteger r;
            BigInteger q;
            do
            {
                using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                {
                    r = RandomInRange(rng, minValueForQ * 2 - 1, minValueForQ * 4 - 1);
                    q = r * r - r + 1;
                }
                    
            } while (!MillerRabinTestIsPrime(q, 10));

            BigInteger k;
            BigInteger bitsForK = BigInteger.Pow(2, minSizePBits - minSizeQBits);
            BigInteger p;
            do
            {
                using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                {
                    k = RandomInRange(rng, bitsForK, bitsForK * 2 - 1);
                    p = r + k * q;
                    //_log.Info($"P = {p.GetBitLength()}");
                }
            } while ((!MillerRabinTestIsPrime(p, 10)) || (p % 3 != 2));

            _log.Info($"P = {p.GetBitLength()}");
            _log.Info($"Q = {q.GetBitLength()}");

            return (p, q);
        }
        public static bool MillerRabinTestIsPrime(BigInteger n, int k)
        {
            // если n == 2 или n == 3 - эти числа простые, возвращаем true
            if (n == 2 || n == 3)
                return true;

            // если n < 2 или n четное - возвращаем false
            if (n < 2 || n % 2 == 0)
                return false;

            // представим n − 1 в виде (2^s)·t, где t нечётно, это можно сделать последовательным делением n - 1 на 2
            BigInteger t = n - 1;

            int s = 0;

            while (t % 2 == 0)
            {
                t >>= 1;
                s += 1;
            }

            // повторить k раз
            for (int i = 0; i < k; i++)
            {
                // выберем случайное целое число a в отрезке [2, n − 2]
                //RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

                //byte[] _a = new byte[n.ToByteArray().LongLength];

                //BigInteger a;

                //do
                //{
                //    rng.GetBytes(_a);
                //    a = new BigInteger(_a);
                //}
                //while (a < 2 || a >= n - 2);
                using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                {
                    BigInteger a = RandomInRange(rng, 2, n - 2);

                    // x ← a^t mod n, вычислим с помощью возведения в степень по модулю
                    BigInteger x = BigInteger.ModPow(a, t, n);

                    // если x == 1 или x == n − 1, то перейти на следующую итерацию цикла
                    if (x == 1 || x == n - 1)
                        continue;

                    // повторить s − 1 раз
                    for (int r = 1; r < s; r++)
                    {
                        // x ← x^2 mod n
                        x = BigInteger.ModPow(x, 2, n);

                        // если x == 1, то вернуть "составное"
                        if (x == 1)
                            return false;

                        // если x == n − 1, то перейти на следующую итерацию внешнего цикла
                        if (x == n - 1)
                            break;
                    }

                    if (x != n - 1)
                        return false;
                }

                // вернуть "вероятно простое"
                return true;
            }
            return true;
        }

        public static BigInteger RandomInRange(RandomNumberGenerator rng, BigInteger min, BigInteger max)
        {
            if (min > max)
            {
                var buff = min;
                min = max;
                max = buff;
            }

            // offset to set min = 0
            BigInteger offset = -min;
            min = 0;
            max += offset;

            var value = RandomInRangeFromZeroToPositive(rng, max) - offset;
            return value;
        }

        private static BigInteger RandomInRangeFromZeroToPositive(RandomNumberGenerator rng, BigInteger max)
        {
            BigInteger value;
            var bytes = max.ToByteArray();

            // count how many bits of the most significant byte are 0
            // NOTE: sign bit is always 0 because `max` must always be positive
            byte zeroBitsMask = 0b00000000;

            var mostSignificantByte = bytes[bytes.Length - 1];

            // we try to set to 0 as many bits as there are in the most significant byte, starting from the left (most significant bits first)
            // NOTE: `i` starts from 7 because the sign bit is always 0
            for (var i = 7; i >= 0; i--)
            {
                // we keep iterating until we find the most significant non-0 bit
                if ((mostSignificantByte & (0b1 << i)) != 0)
                {
                    var zeroBits = 7 - i;
                    zeroBitsMask = (byte)(0b11111111 >> zeroBits);
                    break;
                }
            }
            do
            {
                rng.GetBytes(bytes);

                // set most significant bits to 0 (because `value > max` if any of these bits is 1)
                bytes[bytes.Length - 1] &= zeroBitsMask;

                value = new BigInteger(bytes);

                // `value > max` 50% of the times, in which case the fastest way to keep the distribution uniform is to try again
            } while (value > max);

            return value;
        }
    }
}
