using System;
using System.Numerics;

namespace Mandelbrot
{
    public class BurningShipCalculator : FractalCalculatorBase
    {
        protected override double MaxOrbit => 2d;

        protected override Complex DoIterationStep(Complex z, Complex c)
        {
            return Square(AbsoluteCoorinates(z)) + c;
        }

        private static Complex AbsoluteCoorinates(Complex z) => new Complex(Math.Abs(z.Real), Math.Abs(z.Imaginary));

        private static Complex Square(Complex z) => z*z;
    }
}