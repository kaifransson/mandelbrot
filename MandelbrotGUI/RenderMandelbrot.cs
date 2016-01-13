using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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
        private bool _isRendering;
        private uint _iterationLimit;

        public RenderMandelbrot(MandelbrotViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private bool IsRendering
        {
            get { return _isRendering; }
            set
            {
                _isRendering = value;
                CanExecuteChanged?.Invoke(this, new EventArgs());
            }
        }

        public bool CanExecute(object parameter)
        {
            return !IsRendering;
        }

        public async void Execute(object parameter)
        {
            IsRendering = true;
            _complexPlane = _viewModel.ComplexPlane;
            _bitmap = _viewModel.MandelbrotImageSource;
            _iterationLimit = _viewModel.IterationLimit;
            var bytesPerPixel = _bitmap.Format.BitsPerPixel/8;
            var pixelWidth = _bitmap.PixelWidth;
            var pixelHeight = _bitmap.PixelHeight;
            var stride = pixelWidth*bytesPerPixel;
            var imageBytes = new byte[stride*_bitmap.PixelHeight];
            var chunks = _bitmap.PixelHeight/(1 << 3);
            var chunkSize = pixelWidth*pixelHeight/chunks;
            var renderTasks = Enumerable.Range(0, chunks)
                .Select(chunk => Task.Run(() =>
                    RenderChunk(chunk, chunkSize, bytesPerPixel, imageBytes, pixelWidth)))
                .Select(
                    renderTask => renderTask.ContinueWith(
                        OnRenderComplete(stride, chunks),
                        TaskScheduler.FromCurrentSynchronizationContext()));
            await Task.WhenAll(renderTasks);
            _viewModel.ResetProgress();
            IsRendering = false;
        }

        public event EventHandler CanExecuteChanged;

        private Action<Task<RenderInfo>> OnRenderComplete(int stride, int chunks)
        {
            return completedTask =>
            {
                var renderInfo = completedTask.Result;
                _bitmap.WritePixels(renderInfo.ImageRect, renderInfo.ImageBytes, stride, renderInfo.Offset);
                _viewModel.UpdateRenderProgress(100d/chunks);
            };
        }

        private RenderInfo RenderChunk(int chunk, int chunkPixelSize, int bytesPerPixel, byte[] imageBytes,
            int pixelWidth)
        {
            var stride = pixelWidth*bytesPerPixel;
            var chunkPixelIndex = chunk*chunkPixelSize;
            for (var pixel = chunkPixelIndex; pixel < chunkPixelIndex + chunkPixelSize; pixel++)
            {
                ColorPixel(pixel, bytesPerPixel, imageBytes);
            }
            var x = chunkPixelIndex%pixelWidth; // NOTE: this is wrong
            var y = chunk*(chunkPixelSize/pixelWidth);
            var width = pixelWidth;
            var height = chunkPixelSize/pixelWidth;
            var imageRect = new Int32Rect(x, y, width, height);
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
                : NonMandelbrotColor(result.Z, result.EscapedAfter);
        }

        private static uint MandelbrotColor()
        {
            return 0x000000FF;
        }

        private static uint NonMandelbrotColor(Complex z, uint escapedAfter)
        {
            var smooth = Math.Log(Math.Log(escapedAfter))/Math.Log(escapedAfter);
            var grayscale = smooth*0xFF;
            var bgr = (uint) (grayscale == 1 ? 0xFFFFFF : grayscale*0xFFFFFF);
            return (bgr << 8) | 0xFF;
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
        public byte[] ImageBytes { get; }
    }
}