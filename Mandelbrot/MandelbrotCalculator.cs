using System.Numerics;

namespace Mandelbrot
{
    public class MandelbrotCalculator
    {
        public Result IsMandelbrotNumber(Complex z, uint limit)
        {
            var acc = z;
            for (var i = 1u; i <= limit; i++)
            {
                if ((acc = Square(acc) + z).Magnitude > 2) return new Result(z, false, i);
            }
            return new Result(z, true, limit);
        }

        private static Complex Square(Complex z)
        {
            return z*z;
        }
    }
}