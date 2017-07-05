using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Painter.Abstractions
{
    public enum Scaling
    {
        //Keep the original X and Y locations
        Relative_None,
        Relative_Fill,
        Relative_Fit,

        //Subtract the lowest X and Y locations from each point
        Absolute_None,
        Absolute_Fill,
        Absolute_Fit
    }
}
