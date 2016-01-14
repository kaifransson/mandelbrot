using System.Numerics;

namespace Mandelbrot
{
    public abstract class FractalCalculatorBase : IFractalCalculator
    {
        public Result IsMemberOfFractalSet(Complex z, uint limit)
        {
            var c = z;
            for (var i = 1u; i <= limit; i++)
            {
                if ((z = DoIterationStep(z, c)).Magnitude > MaxOrbit) return new Result(z, false, i);
            }
            return new Result(z, true, limit);
        }

        protected abstract double MaxOrbit { get; }

        protected abstract Complex DoIterationStep(Complex z, Complex c);
    }
}