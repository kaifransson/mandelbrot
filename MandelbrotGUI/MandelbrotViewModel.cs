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
            Exit = new ExitCommand();
            RenderMandelbrot = new RenderMandelbrot(this);
        }

        public Complex[] ComplexPlane => new ComplexUtil().GetComplexPlane(
            Center - new Complex(16d/6, 9d/6),
            Center + new Complex(16d/6, 9d/6),
            MandelbrotImageSource.PixelWidth,
            MandelbrotImageSource.PixelHeight);

        private static Complex Center { get; } = new Complex(-0.5, 0);

        public ICommand Exit { get; }
        public ICommand RenderMandelbrot { get; }

        public WriteableBitmap MandelbrotImageSource { get; } = new WriteableBitmap(
            MandelbrotWidth,
            MandelbrotHeight,
            300, 300,
            PixelFormats.Bgra32,
            null);

        public int IterationLimit { get; set; } = 100;

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