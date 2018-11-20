using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using System.IO;
using Painter.Interfaces;
using UIKit;
using CoreGraphics;
using Foundation;

namespace Painter.iOS
{
    public class PainterExport : IPainterExport 
    {
        public PainterExport()
        {
            
        }

        public async Task<byte[]> GetCurrentImageAsPNG(int width, int height, float scale, List<Abstractions.Stroke> strokes, Abstractions.Scaling scaling = Abstractions.Scaling.Relative_None, int quality = 80, Painter.Abstractions.Color BackgroundColor = null, bool useDevicePixelDensity = false, byte[] BackgroundImage = null)
        {
            return await ExportCurrentImage(width, height, scale, strokes, scaling, Abstractions.ExportFormat.Png, quality, BackgroundColor ?? new Abstractions.Color(1, 1, 1, 1), useDevicePixelDensity, BackgroundImage);
        }

        public async Task<byte[]> GetCurrentImageAsJPG(int width, int height, float scale, List<Abstractions.Stroke> strokes, Abstractions.Scaling scaling = Abstractions.Scaling.Relative_None, int quality = 80, Painter.Abstractions.Color BackgroundColor = null, bool useDevicePixelDensity = false, byte[] BackgroundImage = null)
        {
            return await ExportCurrentImage(width, height, scale, strokes, scaling, Abstractions.ExportFormat.Jpeg, quality, BackgroundColor ?? new Abstractions.Color(1, 1, 1, 1), useDevicePixelDensity, BackgroundImage);
        }

        public async Task<byte[]> ExportCurrentImage(int width, int height, float scale, List<Abstractions.Stroke> strokes, Abstractions.Scaling scaling, Abstractions.ExportFormat format, int quality, Painter.Abstractions.Color BackgroundColor, bool useDevicePixelDensity, byte[] BackgroundImage = null)
        {
            //useDevicePixelDensity is not used on iOS

            UIImage image = drawPath(width, height, strokes);

            NSData imageData;
            if (format == Abstractions.ExportFormat.Png)
            {
                imageData = image.AsPNG();
            }
            else
            {
                imageData = image.AsJPEG(quality);
            }

            return imageData.ToArray();
        }

        private UIImage drawPath(int width, int height, List<Abstractions.Stroke> Strokes)
        {
            UIGraphics.BeginImageContextWithOptions(new CGSize(width, height), false, UIScreen.MainScreen.Scale);//Support Retina when drawing
            CGContext context = UIGraphics.GetCurrentContext();

            context.SetLineCap(CGLineCap.Round);//TODO decide to make this an abstract (requires support on Android and WP)
            context.SetLineJoin(CGLineJoin.Round);//TODO decide to make this an abstract (requires support on Android and WP)

            foreach (var stroke in Strokes)
            {
                if (stroke.Points.Count == 0)
                    continue;

                context.SetLineWidth((float)stroke.Thickness);
                context.SetStrokeColor(new UIColor((float)stroke.StrokeColor.R, (float)stroke.StrokeColor.G, (float)stroke.StrokeColor.B, (float)stroke.StrokeColor.A).CGColor);

                var p = UIBezierPath.Create();
                p.MoveTo(new CGPoint(stroke.Points[0].X, stroke.Points[0].Y));
                for (int i = 1; i < stroke.Points.Count; i++)
                    p.AddLineTo(new CGPoint(stroke.Points[i].X, stroke.Points[i].Y));

                if (stroke.ClosePath)
                    p.AddLineTo(new CGPoint(stroke.Points[0].X, stroke.Points[0].Y));

                context.AddPath(p.CGPath);
                context.StrokePath();
            }

            //Get the image
            UIImage image = UIGraphics.GetImageFromCurrentImageContext();

            //End the CGContext
            UIGraphics.EndImageContext();
            
            return image;
        }
    }
}