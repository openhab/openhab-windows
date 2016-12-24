using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using ColorHelper = OpenHAB.Core.Common.ColorHelper;

namespace OpenHAB.Windows.Controls
{
    public sealed partial class ColorMap : UserControl
    {
        private PointerPoint lastPoint;
        private double colorX, colorY;
        private WriteableBitmap bmp3;

        private readonly LinearGradientBrush _lightnessGradient;
        private readonly GradientStop _lightnessStart;
        private readonly GradientStop _lightnessMid;
        private readonly GradientStop _lightnessEnd;

        private bool _settingColor;
        private bool _settingLightness;

        public ColorMap()
        {
            InitializeComponent();

            Loaded += MeshCanvas_Loaded;

            ellipse.PointerMoved += Image3_PointerMoved;
            ellipse.PointerPressed += Image3_PointerPressed;
            ellipse.PointerReleased += Image3_PointerReleased;

            _lightnessGradient = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1)
            };
            _lightnessStart = new GradientStop();
            _lightnessMid = new GradientStop { Offset = 0.5 };
            _lightnessEnd = new GradientStop { Offset = 1 };
            _lightnessGradient.GradientStops = new GradientStopCollection()
            {
                _lightnessStart, _lightnessMid, _lightnessEnd,
            };
            //this.LightnessBackground.Fill = this._lightnessGradient;
        }

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ColorMap), new PropertyMetadata(new Color(), ColorChanged));
        private bool isloaded;

        private static void ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var map = d as ColorMap;
            if (map != null && !map._settingColor)
            {
                var col = (Color)e.NewValue;
                var hsl = ColorHelper.ToHSL(col);

                map._settingLightness = true;
                //map.LightnessSlider.Value = hsl.Z;
                map._settingLightness = false;
                map._lightnessMid.Color = ColorHelper.FromHSL(new Vector4(hsl.X, 1, 0.5f, 1));

                double angle = Math.PI * 2 * hsl.X;
                Vector2 normalized = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Vector2 halfSize = new Vector2(
                    (float)map.ellipse.ActualWidth / 2,
                    (float)map.ellipse.ActualHeight / 2);
                Vector2 pos = (hsl.Y / 2) * normalized * halfSize * new Vector2(1, -1) + halfSize;

                map.colorX = pos.X;
                map.colorY = pos.Y;
                map.UpdateThumb();
            }
        }

        private void Image3_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ellipse.CapturePointer(e.Pointer);
            this.lastPoint = e.GetCurrentPoint(ellipse);
            this.colorX = lastPoint.Position.X;
            this.colorY = lastPoint.Position.Y;
            this.UpdateColor();
            this.UpdateThumb();
            e.Handled = true;
        }

        private void Image3_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            ellipse.ReleasePointerCapture(e.Pointer);
            this.lastPoint = null;
            e.Handled = true;
        }

        private void Image3_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (ellipse.PointerCaptures?.Any(p => p.PointerId == e.Pointer.PointerId) == true)
            {
                this.lastPoint = e.GetCurrentPoint(ellipse);
                this.colorX = lastPoint.Position.X;
                this.colorY = lastPoint.Position.Y;
                var bounds = new Rect(0, 0, ellipse.ActualWidth, ellipse.ActualHeight);
                if (bounds.Contains(lastPoint.Position) && UpdateColor())
                {
                    UpdateThumb();
                    e.Handled = true;
                }
            }
        }

        private void UpdateThumb()
        {
            Canvas.SetLeft(thumb, this.colorX - thumb.ActualWidth / 2);
            Canvas.SetTop(thumb, this.colorY - thumb.ActualHeight / 2);
            thumb.Visibility = Visibility.Visible;
        }

        private bool UpdateColor()
        {
            if (!this.isloaded) return false;
            var x = this.colorX / ellipse.ActualWidth;
            var y = 1 - this.colorY / ellipse.ActualHeight;
            var selectedColor = CalcWheelColor((float)x, 1 - (float)y, 100);

            if (selectedColor.A > 0)
            {
                this.SetColor(selectedColor);
                this._lightnessStart.Color = Colors.White;
                this._lightnessMid.Color = CalcWheelColor((float)x, 1 - (float)y, 0.5f);
                this._lightnessEnd.Color = Colors.Black;
                return true;
            }

            return false;
        }

        private void SetColor(Color color)
        {
            this._settingColor = true;
            this.Color = color;
            this._settingColor = false;
        }

        private async void MeshCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            bmp3 = new WriteableBitmap(1000, 1000);
            await CreateHueCircle(0.5f);
            this.image3.ImageSource = bmp3;
            this.isloaded = true;
        }

        private Task CreateHueCircle(float lightness)
        {
            return FillBitmap(bmp3, (x, y) =>
            {
                return CalcWheelColor(x, y, lightness);
            });
        }

        public static Color CalcWheelColor(float x, float y, float lightness)
        {
            x = x - 0.5f;
            y = (1 - y) - 0.5f;
            float saturation = 2 * (float)Math.Sqrt(x * x + y * y);
            float hue = y < 0 ?
                (float)Math.Atan2(-y, -x) + (float)Math.PI :
                (float)Math.Atan2(y, x);
            if (saturation > 1)
            {
                saturation = 1;
            }

            return ColorHelper.FromHSL(new Vector4(hue / ((float)Math.PI * 2), saturation, lightness, 1));
        }

        private void lightnessChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (_settingLightness) return;
            this.UpdateColor();
        }

        private static async Task FillBitmap(WriteableBitmap bmp, Func<float, float, Color> fillPixel)
        {
            var stream = bmp.PixelBuffer.AsStream();
            int width = bmp.PixelWidth;
            int height = bmp.PixelHeight;
            await Task.Run(() =>
            {
                for (int y = 0; y < width; y++)
                {
                    for (int x = 0; x < height; x++)
                    {
                        var color = fillPixel((float)x / width, (float)y / height);
                        WriteBGRA(stream, color);
                    }
                }
            });
            stream.Dispose();
            bmp.Invalidate();
        }

        private static void WriteBGRA(Stream stream, Color color)
        {
            stream.WriteByte(color.B);
            stream.WriteByte(color.G);
            stream.WriteByte(color.R);
            stream.WriteByte(color.A);
        }
    }
}
