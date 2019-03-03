using System;
using System.Globalization;
using System.IO;
using System.Linq;
using SkiaSharp;

namespace NestedRectangleDrawer
{
    public class NestedRectangleDrawer
    {
        readonly string[] _colorsSetup =
        {
            "#ffb365", "#e2ef77", "#ff6565", "#77efa1", "#7777ff"
        };

        readonly int _colorSwitchStep = 10;

        public NestedRectangleDrawer()
        {
        }

        public NestedRectangleDrawer(int colorSwitchStep)
        {
            _colorSwitchStep = colorSwitchStep;
        }

        public NestedRectangleDrawer(int colorSwitchStep, string[] colorSetup)
        {
            _colorSwitchStep = colorSwitchStep;
            _colorsSetup = colorSetup;
        }

        public NestedRectangleDrawer(string[] colorSetup)
        {
            _colorsSetup = colorSetup;
        }

        public byte[] GetImageBytes(int width, int height, string imageFormat)
        {
            if (!Enum.TryParse(imageFormat, true, out SKEncodedImageFormat format))
            {
                throw new Exception("Image format is not recognized!");
            }

            byte[] result;

            var bitmap = new SKBitmap(width, height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);

            var data = DrawImage(bitmap).Snapshot().Encode(format, 80);

            using (var memoryStream = new MemoryStream())
            {
                data.SaveTo(memoryStream);
                result = memoryStream.ToArray();
            }

            return result;
        }

        private SKSurface DrawImage(SKBitmap bitmap)
        {
            var surface = SKSurface.Create(bitmap.Info);

            SKCanvas canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            DrawNestedRectangles(canvas, bitmap.Info.Width, bitmap.Info.Height);

            return surface;
        }

        private void DrawNestedRectangles(SKCanvas canvas, int width, int height)
        {
            var colors = _colorsSetup.Select(ParseColor).ToArray();
            
            var paint = new SKPaint
            {
                IsAntialias = true,
                Color = colors[0],
                StrokeWidth = 1
            };

            var colorSwitchIterator = _colorSwitchStep;
            var iterationLimit = width > height ? height : width;

            for (int i = 0; i < iterationLimit / 2; i++)
            {
                canvas.DrawRect(i, i, width - 2 * i, height - 2 * i, paint);

                colorSwitchIterator--;
                if (colorSwitchIterator == 0)
                {
                    var index = (Array.FindIndex(colors, color => color == paint.Color) + 1) % colors.Length;
                    paint.Color = colors[index];
                    colorSwitchIterator = _colorSwitchStep;
                }
            }
        }
        
        public static SKColor ParseColor(string hexstring)
        {
            if (hexstring.StartsWith("#"))
            {
                hexstring = hexstring.Substring(1);
            }

            if (hexstring.StartsWith("0x"))
            {
                hexstring = hexstring.Substring(2);
            }

            if (hexstring.Length != 6)
            {
                throw new Exception(string.Format("{0} is not a valid color string.", hexstring));
            }

            byte r = byte.Parse(hexstring.Substring(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hexstring.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hexstring.Substring(4, 2), NumberStyles.HexNumber);

            return new SKColor(r, g, b);
        }
    }
}
