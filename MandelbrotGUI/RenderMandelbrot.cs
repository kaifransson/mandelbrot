using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Mandelbrot;

namespace MandelbrotGUI
{
    internal class RenderMandelbrot : ICommand
    {
        private readonly MandelbrotCalculator _mandelbrotCalculator = new MandelbrotCalculator();
        private readonly MandelbrotViewModel _viewModel;
        private WriteableBitmap _bitmap;
        private IReadOnlyList<Complex> _complexPlane;
        private int _iterationLimit;

        public RenderMandelbrot(MandelbrotViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _complexPlane = _viewModel.ComplexPlane;
            _bitmap = _viewModel.MandelbrotImageSource;
            _iterationLimit = _viewModel.IterationLimit;
            var bytesPerPixel = _bitmap.Format.BitsPerPixel/8;
            var pixelWidth = _bitmap.PixelWidth;
            var stride = pixelWidth*bytesPerPixel;
            var imageBytes = new byte[stride*_bitmap.PixelHeight];
            var chunks = _bitmap.PixelHeight;
            var chunkSize = pixelWidth;
            var renderTasks = new List<Task>();
            foreach (var chunk in Enumerable.Range(0, chunks))
            {
                var renderTask = Task.Run(() =>
                {
                    return RenderChunk(chunk, chunkSize, bytesPerPixel, imageBytes, stride, pixelWidth);
                });
                var continuation = renderTask.ContinueWith(completedTask =>
                {
                    var renderInfo = completedTask.Result;
                    _bitmap.WritePixels(renderInfo.ImageRect, renderInfo.ImageBytes, stride, renderInfo.Offset);
                    _viewModel.UpdateRenderProgress(100d/chunks);
                }, TaskScheduler.FromCurrentSynchronizationContext());
                renderTasks.Add(continuation);
            }
            Task.WhenAll(renderTasks).ContinueWith(allDone => _viewModel.ResetProgress());
        }


        public event EventHandler CanExecuteChanged;

        private RenderInfo RenderChunk(int chunk, int chunkSize, int bytesPerPixel, byte[] imageBytes, int stride,
            int pixelWidth)
        {
            var chunkPixelIndex = chunk*chunkSize;
            for (var pixel = chunkPixelIndex; pixel < chunkPixelIndex + chunkSize; pixel++)
            {
                ColorPixel(pixel, bytesPerPixel, imageBytes);
            }
            var imageRect = new Int32Rect(0, chunk, pixelWidth, 1);
            var offset = chunkPixelIndex*bytesPerPixel;
            return new RenderInfo(imageRect, imageBytes, stride, offset);
        }

        private void ColorPixel(int pixel, int bytesPerPixel, byte[] imageBytes)
        {
            var pixelColor = CalculatePixelColor(pixel);
            var pixelIndex = pixel*bytesPerPixel;
            imageBytes[pixelIndex] = GetBlue(pixelColor);
            imageBytes[pixelIndex + 1] = GetGreen(pixelColor);
            imageBytes[pixelIndex + 2] = GetRed(pixelColor);
            imageBytes[pixelIndex + 3] = GetAlpha(pixelColor);
        }

        private uint CalculatePixelColor(int pixel)
        {
            var result = _mandelbrotCalculator.IsMandelbrotNumber(_complexPlane[pixel], _iterationLimit);
            return result.IsMandelbrotNumber
                ? MandelbrotColor()
                : CalculateFancyGradient(result.Z, result.EscapedAfter);
        }

        private static uint MandelbrotColor()
        {
            return 0x000000FF;
        }

        private uint CalculateFancyGradient(Complex z, uint escapedAfter)
        {
            var smooth = (escapedAfter + 1 - Math.Log(Math.Log(z.Magnitude)) / Math.Log(2)) / _iterationLimit;
            return ((uint) (smooth * 0xFFFFFF) << 2*4) | 0xFF;
        }

        private static byte GetAlpha(uint pixelColor)
        {
            return (byte) (pixelColor & 0xFF);
        }

        private static byte GetRed(uint pixelColor)
        {
            return (byte) ((pixelColor & 0xFF00) >> 2*4);
        }

        private static byte GetGreen(uint pixelColor)
        {
            return (byte) ((pixelColor & 0xFF0000) >> 4*4);
        }

        private static byte GetBlue(uint pixelColor)
        {
            return (byte) ((pixelColor & 0xFF000000) >> 6*4);
        }
    }

    internal class RenderInfo
    {
        public RenderInfo(Int32Rect imageRect, byte[] imageBytes, int stride, int offset)
        {
            Stride = stride;
            Offset = offset;
            ImageRect = imageRect;
            ImageBytes = imageBytes;
        }

        public int Stride { get; }
        public int Offset { get; }
        public Int32Rect ImageRect { get; }
        public byte[] ImageBytes { get; set; }
    }
}