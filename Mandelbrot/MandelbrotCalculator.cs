using System.Numerics;

namespace Mandelbrot
{
    public class MandelbrotCalculator : FractalCalculatorBase
    {
        protected override double MaxOrbit => 2d;

        protected override Complex DoIterationStep(Complex z, Complex c)
        {
            return Square(z) + c;
        }

        private static Complex Square(Complex z) => z*z;
    }
}