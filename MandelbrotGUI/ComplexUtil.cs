using System.Numerics;

namespace MandelbrotGUI
{
    public class ComplexUtil
    {
        public Complex[] GetComplexPlane(Complex lowerLeft, Complex upperRight, int width, int height)
        {
            var complexPlane = new Complex[width*height];
            var xDistance = upperRight.Real - lowerLeft.Real;
            var yDistance = upperRight.Imaginary - lowerLeft.Imaginary;
            var xDistancePerStep = xDistance/width;
            var yDistancePerStep = yDistance/height;
            for (var row = 0; row < height; row++)
            {
                for (var rowOffset = 0; rowOffset < width; rowOffset++)
                {
                    var real = lowerLeft.Real + rowOffset*xDistancePerStep;
                    var imaginary = upperRight.Imaginary - row*yDistancePerStep;
                    complexPlane[row*width + rowOffset] = new Complex(real, imaginary);
                }
            }
            return complexPlane;
        }
    }
}