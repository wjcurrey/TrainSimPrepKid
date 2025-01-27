﻿using FreeTrainSimulator.Common;
using FreeTrainSimulator.Common.Api;

using MemoryPack;

namespace Orts.Formats.Msts
{
    [MemoryPackable]
    public sealed partial class TravellerSaveState : SaveStateBase
    {
        public Direction Direction { get; set; }
        public int TrackNodeIndex { get; set; }
        public float TrackVectorSectionOffset { get; set; }
        public int TrackVectorSectionIndex { get; set; }
    }
}
