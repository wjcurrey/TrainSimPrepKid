// COPYRIGHT 2014, 2015 by the Open Rails project.
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

using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Orts.Formats.Msts;
using Orts.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Orts.Formats.OR
{

    #region StationItem

    public class StationItem : GlobalItem
    {
        [JsonProperty("nameStation")]
        public string nameStation;
        [JsonProperty("nameVisible")]
        public bool nameVisible;
        [JsonProperty("icoAngle")]
        public float icoAngle;
        [JsonProperty("areaCompleted")]
        public bool areaCompleted;

        [JsonIgnore]
        public AETraveller traveller { get; protected set; }

        public void setTraveller(AETraveller travel)
        {
            this.traveller = travel;
        }

    }
    #endregion
}
