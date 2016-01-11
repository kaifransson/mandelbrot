using System.Numerics;
using Xunit;

namespace Mandelbrot.Tests
{
    public class MandelbrotTest
    {
        private readonly MandelbrotCalculator _mandelbrotCalculator = new MandelbrotCalculator();

        private void AssertIsMandelbrotNumber(Complex z, int limit)
        {
            Assert.True(_mandelbrotCalculator.IsMandelbrotNumber(z, limit).IsMandelbrotNumber);
        }

        private void AssertIsNotMandelbrotNumber(Complex z, int limit)
        {
            Assert.False(_mandelbrotCalculator.IsMandelbrotNumber(z, limit).IsMandelbrotNumber);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(-1, 0)]
        [InlineData(-2, 0)]
        public void IsMandelbrotNumber_ZThatStaysBoundedOnFirstIteration_True(double real, double imaginary)
        {
            AssertIsMandelbrotNumber(new Complex(real, imaginary), 1);
        }

        [Theory]
        [InlineData(2, 0, 1)]
        [InlineData(0, 2, 1)]
        public void IsMandelbrotNumber_ZEscapesOnFirstIteration_False(double real, double imaginary, int limit)
        {
            AssertIsNotMandelbrotNumber(new Complex(real, imaginary), limit);
        }
    }
}