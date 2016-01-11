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
        private const int MandelbrotWidth = 1080;
        private const int MandelbrotHeight = 720;

        public MandelbrotViewModel()
        {
            Exit = new ExitCommand();
            ComplexPlane = new ComplexUtil()
                .GetComplexPlane(
                    new Complex(-2.5, -1.8),
                    new Complex(0.75, 1.8),
                    MandelbrotImageSource.PixelWidth,
                    MandelbrotImageSource.PixelHeight);
            RenderMandelbrot = new RenderMandelbrot(this);
        }

        public Complex[] ComplexPlane { get; }

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