﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using Orts.Common.Calc;

using SharpDX.X3DAudio;

namespace Orts.ActivityRunner.Viewer3D.PopupWindows
{
    internal static class ColorCoding
    {

        private const double speedThreshold = 5 / 3.6; // 5km/h to m/s

        internal static Color ArrivalColor(TimeSpan expected, TimeSpan? actual)
        {
            return actual.HasValue && actual.Value <= expected ? Color.LightGreen : Color.LightSalmon;
        }

        internal static Color ArrivalColor(TimeSpan expected, TimeSpan actual)
        {
            return actual <= expected ? Color.LightGreen : Color.LightSalmon;
        }

        internal static Color DepartureColor(TimeSpan expected, TimeSpan? actual)
        {
            return actual.HasValue && actual.Value >= expected ? Color.LightGreen : Color.LightSalmon;
        }

        internal static Color DepartureColor(TimeSpan expected, TimeSpan actual)
        {
            return actual >= expected ? Color.LightGreen : Color.LightSalmon;
        }

        internal static Color SpeedingColor(double speed, double allowedSpeed)
        {
            speed = Math.Abs(speed);
            return speed < (allowedSpeed - speedThreshold) ? Color.LimeGreen :
                speed <= allowedSpeed ? Color.PaleGreen :
                speed < allowedSpeed + speedThreshold ? Color.Orange : Color.Red;
        }
    }
}