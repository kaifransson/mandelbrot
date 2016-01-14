using System.ComponentModel;
using System.Numerics;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Mandelbrot;
using MandelbrotGUI.Commands;

namespace MandelbrotGUI
{
    public class FractalViewModel : INotifyPropertyChanged
    {
        private const int MandelbrotWidth = 1280;
        private const int MandelbrotHeight = 720;
        private const double DefaultFractalWidth = 16d/3;
        private const double DefaultFractalHeight = 9d/3;
        private readonly FractalCalculatorFactory _fractalCalculatorFactory = new FractalCalculatorFactory();
        private FractalType _fractalType;

        public FractalViewModel()
        {
            FractalType = FractalType.Mandelbrot;
            RenderFractal.Execute(this);
        }

        public Complex[] ComplexPlane
            => UpdateComplexPlane(DefaultFractalWidth/ZoomLevel, DefaultFractalHeight/ZoomLevel);

        private static Complex Center { get; } = new Complex(-0.5, 0);

        public ICommand Exit { get; } = new ExitCommand();

        public ICommand RenderFractal { get; set; }

        public WriteableBitmap MandelbrotImageSource { get; } = new WriteableBitmap(
            MandelbrotWidth,
            MandelbrotHeight,
            300, 300,
            PixelFormats.Bgra32,
            null);

        public uint IterationLimit { get; set; } = 100;

        public double RenderProgress { get; private set; }

        public double ZoomLevel { get; set; } = 1;

        public FractalType FractalType
        {
            get { return _fractalType; }
            set
            {
                _fractalType = value;
                RenderFractal = new RenderFractal(this, _fractalCalculatorFactory.Build(value));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RenderFractal)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Complex[] UpdateComplexPlane(double width, double height)
        {
            var centerOffset = new Complex(width/2, height/2);
            return new ComplexUtil().GetComplexPlane(
                Center - centerOffset,
                Center + centerOffset,
                MandelbrotImageSource.PixelWidth,
                MandelbrotImageSource.PixelHeight);
        }

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