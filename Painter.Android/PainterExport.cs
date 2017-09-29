using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using static Android.Graphics.Bitmap;
using System.IO;
using Android.Graphics;
using Android.Util;
using Painter.Interfaces;
using Painter.Android;

namespace Painter.Android
{
    public class PainterExport : IPainterExport
    {
        private DisplayMetrics metrics;
        private byte[] BackgroundImage { get; set; }
        public PainterExport()
        {
            IWindowManager windowManager = Application.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            metrics = new DisplayMetrics();
            windowManager.DefaultDisplay.GetMetrics(metrics);
        }

        public void SetBackgroundImage(byte[] backgroundImage)
        {
            this.BackgroundImage = backgroundImage;
        }

        public async Task<byte[]> GetCurrentImageAsPNG(int width, int height, List<Abstractions.Stroke> strokes, Abstractions.Scaling scaling = Abstractions.Scaling.Relative_None, int quality = 80, Painter.Abstractions.Color BackgroundColor = null)
        {
            return await ExportCurrentImage(width, height, strokes, scaling, Abstractions.ExportFormat.Png, quality, BackgroundColor ?? new Abstractions.Color(1, 1, 1, 1));
        }

        public async Task<byte[]> GetCurrentImageAsJPG(int width, int height, List<Abstractions.Stroke> strokes, Abstractions.Scaling scaling = Abstractions.Scaling.Relative_None, int quality = 80, Painter.Abstractions.Color BackgroundColor = null)
        {
            return await ExportCurrentImage(width, height, strokes, scaling, Abstractions.ExportFormat.Jpeg, quality, BackgroundColor ?? new Abstractions.Color(1, 1, 1, 1));
        }

        public async Task<byte[]> ExportCurrentImage(int width, int height, List<Abstractions.Stroke> strokes, Abstractions.Scaling scaling, Abstractions.ExportFormat format, int quality, Painter.Abstractions.Color BackgroundColor)
        {
            //Initialize data holders
            byte[] data;
            Stream str = new MemoryStream();

            //Calculate the actual area the image will take up
            float minWidth = 0.0f;
            float minHeight = 0.0f;
            float minX = float.MaxValue;
            float minY = float.MaxValue;

            foreach (var stroke in strokes)
            {
                foreach (var point in stroke.Points)
                {
                    minWidth = (float)Math.Max(point.X + stroke.Thickness, minWidth);
                    minHeight = (float)Math.Max(point.Y + stroke.Thickness, minHeight);

                    minX = (float)Math.Min(point.X + stroke.Thickness, minX);
                    minY = (float)Math.Min(point.Y + stroke.Thickness, minY);
                }
            }

            float scaleX = 1;
            float scaleY = 1;
            float offsetX = 0;
            float offsetY = 0;

            var dx = minWidth - width;
            var dy = minHeight - height;

            switch (scaling)
            {
                //Fill
                case Abstractions.Scaling.Relative_Fill:
                    scaleX = (float)width / (float)minWidth;
                    scaleY = (float)height / (float)minHeight;
                    break;
                case Abstractions.Scaling.Absolute_Fill:
                    scaleX = (float)width / (float)(minWidth - minX);
                    scaleY = (float)height / (float)(minHeight - minY);
                    offsetX = -minX;
                    offsetY = -minY;
                    break;

                //Fit
                case Abstractions.Scaling.Relative_Fit:
                    if (Math.Abs(dx) < Math.Abs(dy))
                    {
                        //Scale based on the width
                        scaleX = scaleY = (float)width / (float)minWidth;
                    }
                    else
                    {
                        //Scale based on the height
                        scaleX = scaleY = (float)height / (float)minHeight;
                    }
                    break;
                case Abstractions.Scaling.Absolute_Fit:
                    if (Math.Abs(dx) < Math.Abs(dy))
                    {
                        //Scale based on the width
                        scaleX = scaleY = (float)width / (float)(minWidth - minX);
                    }
                    else
                    {
                        //Scale based on the height
                        scaleX = scaleY = (float)height / (float)(minHeight - minY);
                    }
                    offsetX = -minX;
                    offsetY = -minY;
                    break;

                //None
                case Abstractions.Scaling.Relative_None:
                    break;
                case Abstractions.Scaling.Absolute_None:
                    offsetX = -minX;
                    offsetY = -minY;
                    break;
            }

            //Compress the image
            Bitmap tempImage = null;
            Canvas tempCanvas = null;
            if (BackgroundImage == null)
            {
                tempImage = Bitmap.CreateBitmap(metrics, width, height, Bitmap.Config.Argb8888);
                tempCanvas = new Canvas(tempImage);
                DrawStrokes(tempCanvas, new Color((byte)(BackgroundColor.R * 255.0), (byte)(BackgroundColor.G * 255.0), (byte)(BackgroundColor.B * 255.0), (byte)(BackgroundColor.A * 255.0)), strokes, scaleX, scaleY, offsetX, offsetY);
            }
            else
            {
                tempCanvas = new Canvas(BitmapFactory.DecodeByteArray(BackgroundImage, 0, BackgroundImage.Length));
                DrawStrokes(tempCanvas, strokes, scaleX, scaleY, offsetX, offsetY);
            }

            //Compress the image and save it to the stream
            switch (format)
            {
                case Abstractions.ExportFormat.Png:
                    await tempImage.CompressAsync(CompressFormat.Png, quality, str);
                    break;
                case Abstractions.ExportFormat.Jpeg:
                    await tempImage.CompressAsync(CompressFormat.Jpeg, quality, str);
                    break;
            }
            
            //Memory management
            tempImage.Dispose();
            tempCanvas.Dispose();
            tempImage = null;
            tempCanvas = null;

            //Read the data
            data = new byte[str.Length];
            str.Seek(0, SeekOrigin.Begin);
            await str.ReadAsync(data, 0, (int)str.Length);

            //Return the data
            return data;
        }

        private void DrawStrokes(Canvas _canvas, Color backgroundColor, List<Abstractions.Stroke> strokes, float scaleX, float scaleY, float offsetX, float offsetY)
        {
            _canvas.DrawColor(backgroundColor, PorterDuff.Mode.Src);
            foreach (var stroke in strokes)
            {
                double lastX = (stroke.Points[0].X + offsetX) * scaleX;
                double lastY = (stroke.Points[0].Y + offsetY) * scaleY;

                var paint = CreatePaint(stroke.StrokeColor.R, stroke.StrokeColor.G, stroke.StrokeColor.B, stroke.StrokeColor.A, stroke.Thickness, metrics.Density);

                foreach (var p in stroke.Points)
                {
                    _canvas.DrawLine((float)lastX, (float)lastY, (float)(p.X + offsetX) * scaleX, (float)(p.Y + offsetY) * scaleY, paint);
                    lastX = (p.X + offsetX) * scaleX;
                    lastY = (p.Y + offsetY) * scaleY;
                }
            }
        }

        private void DrawStrokes(Canvas _canvas, List<Abstractions.Stroke> strokes, float scaleX, float scaleY, float offsetX, float offsetY)
        {
            foreach (var stroke in strokes)
            {
                double lastX = (stroke.Points[0].X + offsetX) * scaleX;
                double lastY = (stroke.Points[0].Y + offsetY) * scaleY;

                var paint = CreatePaint(stroke.StrokeColor.R, stroke.StrokeColor.G, stroke.StrokeColor.B, stroke.StrokeColor.A, stroke.Thickness, metrics.Density);

                foreach (var p in stroke.Points)
                {
                    _canvas.DrawLine((float)lastX, (float)lastY, (float)(p.X + offsetX) * scaleX, (float)(p.Y + offsetY) * scaleY, paint);
                    lastX = (p.X + offsetX) * scaleX;
                    lastY = (p.Y + offsetY) * scaleY;
                }
            }
        }

        private Paint CreatePaint(double R, double G, double B, double A, double Thickness, float Density)
        {
            var paint = new Paint()
            {
                Color = new Color((byte)(R * 255.0), (byte)(G * 255.0), (byte)(B * 255.0), (byte)(A * 255.0)),
                StrokeWidth = (float)Thickness * Density,
                AntiAlias = true,
                StrokeCap = Paint.Cap.Round
            };

            return paint;
        }
    }
}