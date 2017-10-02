using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Painter.Interfaces
{
    public interface IPainterExport
    {
        Task<byte[]> GetCurrentImageAsPNG(int width, int height, List<Abstractions.Stroke> strokes, Abstractions.Scaling scaling = Abstractions.Scaling.Relative_None, int quality = 80, Painter.Abstractions.Color BackgroundColor = null, bool useDevicePixelDensity = false, byte[] BackgroundImage = null);
        Task<byte[]> GetCurrentImageAsJPG(int width, int height, List<Abstractions.Stroke> strokes, Abstractions.Scaling scaling = Abstractions.Scaling.Relative_None, int quality = 80, Painter.Abstractions.Color BackgroundColor = null, bool useDevicePixelDensity = false, byte[] BackgroundImage = null);
        Task<byte[]> ExportCurrentImage(int width, int height, List<Abstractions.Stroke> strokes, Abstractions.Scaling scaling, Abstractions.ExportFormat format, int quality, Painter.Abstractions.Color BackgroundColor, bool useDevicePixelDensity = false, byte[] BackgroundImage = null);
    }
}
