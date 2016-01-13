using System.ComponentModel;
using System.Numerics;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MandelbrotGUI.Commands;

namespace MandelbrotGUI
{
    public class MandelbrotViewModel : INotifyPropertyChanged
    {
        private const int MandelbrotWidth = 1280;
        private const int MandelbrotHeight = 720;

        public MandelbrotViewModel()
        {
            RenderMandelbrot = new RenderMandelbrot(this);
        }

        public Complex[] ComplexPlane => UpdateComplexPlane(16d/3/ZoomLevel, 9d/3/ZoomLevel);

        private Complex[] UpdateComplexPlane(double width, double height)
        {
            var centerOffset = new Complex(width/2, height/2);
            return new ComplexUtil().GetComplexPlane(
                Center - centerOffset,
                Center + centerOffset,
                MandelbrotImageSource.PixelWidth,
                MandelbrotImageSource.PixelHeight);
        }

        private static Complex Center { get; set; } = new Complex(-0.1564, -1.0320);

        public ICommand Exit { get; } = new ExitCommand();
        public ICommand RenderMandelbrot { get; }

        public WriteableBitmap MandelbrotImageSource { get; } = new WriteableBitmap(
            MandelbrotWidth,
            MandelbrotHeight,
            300, 300,
            PixelFormats.Bgra32,
            null);

        public uint IterationLimit { get; set; } = 100;

        public double RenderProgress { get; private set; }

        public double ZoomLevel { get; set; } = 1;

        public event PropertyChangedEventHandler PropertyChanged;

        public void UpdateRenderProgress(double progress)
        {
            RenderProgress += progress;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RenderProgress)));
        }

        public void ResetProgress()
        {
            RenderProgress = 0;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RenderProgress)));
        }
    }
}