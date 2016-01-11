using System.Numerics;

namespace Mandelbrot
{
    public class Result
    {
        public Result(Complex z, bool isMandelbrotNumber, uint escapedAfter)
        {
            Z = z;
            IsMandelbrotNumber = isMandelbrotNumber;
            EscapedAfter = escapedAfter;
        }

        public Result(Complex z, bool isMandelbrotNumber)
            : this(z, isMandelbrotNumber, uint.MaxValue)
        {
        }

        public Complex Z { get; }

        public bool IsMandelbrotNumber { get; }
        public uint EscapedAfter { get; }
    }
}