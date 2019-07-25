﻿// COPYRIGHT 2014, 2015 by the Open Rails project.
// 
// This file is part of Open Rails.
// 
// Open Rails is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Open Rails is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Open Rails.  If not, see <http://www.gnu.org/licenses/>.

using Newtonsoft.Json;
using Orts.Formats.Msts;
using Orts.Common;
using System;
using System.Drawing;

namespace Orts.Formats.OR
{
    /// <summary>
    /// GlobalItem: The generic item for the viewer and json
    /// </summary>
    public class GlobalItem
    {
        [JsonProperty("Location")]
        public PointF Location;
        [JsonProperty("Location2D")]
        public PointF Location2D;
        [JsonProperty("typeWidget")]
        public int typeItem;
        [JsonProperty("CoordMSTS")]
        public MSTSCoord Coord;
        [JsonProperty("NodeIDX")]
        public int associateNodeIdx { get; protected set; }

        /// <summary>
        /// The default constructor
        /// </summary>
        public GlobalItem()
        {
            typeItem = (int)TypeItem.GLOBAL_ITEM;
            Location = new PointF(float.NegativeInfinity, float.NegativeInfinity);
            Location2D = new PointF(float.NegativeInfinity, float.NegativeInfinity);
        }
    }
}
