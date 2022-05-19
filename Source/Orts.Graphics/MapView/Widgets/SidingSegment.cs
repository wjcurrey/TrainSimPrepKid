﻿using System.Collections.Specialized;

using Microsoft.Xna.Framework;

using Orts.Common.Position;
using Orts.Graphics.MapView.Shapes;

namespace Orts.Graphics.MapView.Widgets
{

    internal class SidingSegment : SegmentBase, IDrawable<VectorPrimitive>
    {
        public SidingSegment(SegmentBase source) : base(source)
        {
            Size = 3;
        }

        public SidingSegment(SegmentBase source, in PointD start, in PointD end) : base(source, start, end)
        {
            Size = 3;
        }

        public SidingSegment(in PointD start, in PointD end) : base(start, end)
        {
            Size = 3;
        }

        public virtual void Draw(ContentArea contentArea, ColorVariation colorVariation = ColorVariation.None, double scaleFactor = 1)
        {
            Color drawColor = this.GetColor<SidingSegment>(colorVariation);
            if (Curved)
                BasicShapes.DrawArc(contentArea.WorldToScreenSize(Size * scaleFactor), drawColor, contentArea.WorldToScreenCoordinates(in Location), contentArea.WorldToScreenSize(Radius), Direction, Angle, contentArea.SpriteBatch);
            else
                BasicShapes.DrawLine(contentArea.WorldToScreenSize(Size * scaleFactor), drawColor, contentArea.WorldToScreenCoordinates(in Location), contentArea.WorldToScreenSize(Length), Direction, contentArea.SpriteBatch);
        }

        public override NameValueCollection DebugInfo => null;
    }
}
