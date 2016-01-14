using System;
using Mandelbrot;

namespace MandelbrotGUI
{
    internal class FractalCalculatorFactory
    {
        public IFractalCalculator Build(FractalType value)
        {
            switch (value)
            {
                case FractalType.Mandelbrot:
                    return new MandelbrotCalculator();
                case FractalType.BurningShip:
                    return new BurningShipCalculator();
                default:
                    throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
    }
}