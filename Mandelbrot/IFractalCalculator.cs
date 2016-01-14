using System.Numerics;

namespace Mandelbrot
{
    public interface IFractalCalculator
    {
        Result IsMemberOfFractalSet(Complex z, uint limit);
    }
}