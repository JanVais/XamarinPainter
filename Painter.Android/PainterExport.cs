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
using Android.Provider;
using Android.Content.Res;

namespace Painter.Droid
{
    public class PainterExport : IPainterExport 
    {
        private DisplayMetrics metrics;
        public PainterExport()
        {
            IWindowManager windowManager = Application.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            metrics = new DisplayMetrics();
            windowManager.DefaultDisplay.GetMetrics(metrics);
        }

        public async Task<byte[]> GetCurrentImageAsPNG(int width, int height, List<Abstractions.Stroke> strokes, Abstractions.Scaling scaling = Abstractions.Scaling.Relative_None, int quality = 80, Painter.Abstractions.Color BackgroundColor = null, bool useDevicePixelDensity = false, byte[] BackgroundImage = null)
        {
            return await ExportCurrentImage(width, height, strokes, scaling, Abstractions.ExportFormat.Png, quality, BackgroundColor ?? new Abstractions.Color(1, 1, 1, 1), useDevicePixelDensity, BackgroundImage);
        }

        public async Task<byte[]> GetCurrentImageAsJPG(int width, int height, List<Abstractions.Stroke> strokes, Abstractions.Scaling scaling = Abstractions.Scaling.Relative_None, int quality = 80, Painter.Abstractions.Color BackgroundColor = null, bool useDevicePixelDensity = false, byte[] BackgroundImage = null)
        {
            return await ExportCurrentImage(width, height, strokes, scaling, Abstractions.ExportFormat.Jpeg, quality, BackgroundColor ?? new Abstractions.Color(1, 1, 1, 1), useDevicePixelDensity, BackgroundImage);
        }

        public async Task<byte[]> ExportCurrentImage(int width, int height, List<Abstractions.Stroke> strokes, Abstractions.Scaling scaling, Abstractions.ExportFormat format, int quality, Painter.Abstractions.Color BackgroundColor, bool useDevicePixelDensity, byte[] BackgroundImage = null)
        {
            //Initialize data holders
            byte[] data;
            Stream str = new MemoryStream();

            if(useDevicePixelDensity)
            {
                var density = Resources.System.DisplayMetrics.Density;
                width *= (int)density;
                height *= (int)density;
            }
            
            Bitmap tempImage = null;
            Bitmap backgroundBitmap = null;
            Canvas tempCanvas = null;
            if (BackgroundImage == null)
            {
                tempImage = Bitmap.CreateBitmap(metrics, width, height, Bitmap.Config.Argb8888);
                tempCanvas = new Canvas(tempImage);
            }
            else
            {
                backgroundBitmap = await BitmapFactory.DecodeByteArrayAsync(BackgroundImage, 0, BackgroundImage.Length);
                var backgroundScale = 1f;
                if (scaling == Abstractions.Scaling.Absolute_Fit || scaling == Abstractions.Scaling.Relative_Fit)
                {
                    if (backgroundBitmap.Width > backgroundBitmap.Height)
                    {
                        backgroundScale = (float)width / (float)backgroundBitmap.Width;
                    }
                    else
                    {
                        backgroundScale = (float)height / (float)backgroundBitmap.Height;
                    }   
                }

                tempImage = Bitmap.CreateBitmap(metrics, (int)Math.Min(width, backgroundBitmap.Width * backgroundScale), (int)Math.Min(height, backgroundBitmap.Height * backgroundScale), Bitmap.Config.Argb8888);
                tempCanvas = new Canvas(tempImage);
            }

            DrawStrokes(tempCanvas, strokes, backgroundBitmap, BackgroundColor, scaling, width, height);

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
            var read = await str.ReadAsync(data, 0, (int)str.Length);

            //Return the data
            return data;
        }
        
        private void DrawStrokes(Canvas _canvas, List<Abstractions.Stroke> strokes, Bitmap backgroundBitmap, Abstractions.Color backgroundColor, Abstractions.Scaling backgroundScaling, int Width, int Height)
        {
            if (backgroundColor != null)
            {
                _canvas.DrawColor(new Color((byte)(backgroundColor.R * 255f), (byte)(backgroundColor.G * 255f), (byte)(backgroundColor.B * 255f)), PorterDuff.Mode.Src);
            }

            if (backgroundBitmap != null)
            {
                switch (backgroundScaling)
                {
                    case Abstractions.Scaling.Absolute_None:
                    case Abstractions.Scaling.Relative_None:
                        _canvas.DrawBitmap(backgroundBitmap, 0, 0, new Paint());
                        break;
                    case Abstractions.Scaling.Absolute_Fit:
                    case Abstractions.Scaling.Relative_Fit:
                        float scale = 1.0f;
                        if (backgroundBitmap.Width > backgroundBitmap.Height)
                        {
                            scale = (float)Width / (float)backgroundBitmap.Width;
                        }
                        else
                        {
                            scale = (float)Height / (float)backgroundBitmap.Height;
                        }
                        _canvas.DrawBitmap(backgroundBitmap, new Rect(0, 0, backgroundBitmap.Width, backgroundBitmap.Height), new Rect(0, 0, (int)(backgroundBitmap.Width * scale), (int)(backgroundBitmap.Height * scale)), new Paint());
                        break;
                    case Abstractions.Scaling.Absolute_Fill:
                    case Abstractions.Scaling.Relative_Fill:
                        _canvas.DrawBitmap(backgroundBitmap, new Rect(0, 0, backgroundBitmap.Width, backgroundBitmap.Height), new Rect(0, 0, Width, Height), new Paint());
                        break;
                }
            }

            foreach (var stroke in strokes)
            {
                double lastX = stroke.Points[0].X;
                double lastY = stroke.Points[0].Y;

                var paint = CreatePaint(stroke.StrokeColor.R, stroke.StrokeColor.G, stroke.StrokeColor.B, stroke.StrokeColor.A, stroke.Thickness, metrics.Density);
                
                var path = new Android.Graphics.Path();
                path.MoveTo((float)stroke.Points[0].X, (float)stroke.Points[0].Y);

                foreach (var p in stroke.Points)
                    path.LineTo((float)p.X, (float)p.Y);

                _canvas.DrawPath(path, paint);
            }
        }

        private Paint CreatePaint(double R, double G, double B, double A, double Thickness, float Density)
        {
            var paint = new Paint()
            {
                Color = new Color((byte)(R * 255.0), (byte)(G * 255.0), (byte)(B * 255.0), (byte)(A * 255.0)),
                StrokeWidth = (float)Thickness * Density,
                AntiAlias = true,
                StrokeCap = Paint.Cap.Round,
            };
            paint.SetStyle(Paint.Style.Stroke);

            return paint;
        }
    }
}