using System;
using System.Windows;
using System.Windows.Media;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace WpfScaleIssue
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private float _scale;
        private float _width;
        private float _height;

        public MainWindow()
        {
            InitializeComponent();

            // Solution 1) Ignore pixel scaling
            // SkiaCanvas.IgnorePixelScaling = true;

            SkiaCanvas.PaintSurface += SkiaCanvasOnPaintSurface;
            CompositionTarget.Rendering += (sender, args) => InvalidateVisual();

            SizeChanged += (sender, args) =>
            {
                _scale = DetermineSkiaScale();
                _width = (float)SkiaCanvas.ActualWidth;
                _height = (float)SkiaCanvas.ActualHeight;
            };
        }

        private void SkiaCanvasOnPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            // Solution 2) Scale the canvas
            // args.Surface.Canvas.SetMatrix(SKMatrix.MakeScale(_scale, _scale));
            
            Draw(args.Surface.Canvas);
        }

        private void Draw(SKCanvas canvas)
        {
            canvas.Clear(SKColors.LightPink);
            canvas.DrawLine(0, 0, _width, _height, new SKPaint { Color = SKColors.Red });
            canvas.DrawCircle(_width, _height, 10, new SKPaint { Color = SKColors.Red });
        }

        private float DetermineSkiaScale()
        {
            var presentationSource = PresentationSource.FromVisual(this);
            if (presentationSource == null) throw new Exception("PresentationSource is null");
            var compositionTarget = presentationSource.CompositionTarget;
            if (compositionTarget == null) throw new Exception("CompositionTarget is null");

            var matrix = compositionTarget.TransformToDevice;

            var dpiX = matrix.M11;
            var dpiY = matrix.M22;

            if (dpiX != dpiY) throw new ArgumentException();

            return (float)dpiX;
        }
    }
}