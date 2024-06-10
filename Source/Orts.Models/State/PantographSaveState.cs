﻿using FreeTrainSimulator.Common.Api;

using MemoryPack;

using Orts.Common;

namespace Orts.Models.State
{
    [MemoryPackable]
    public sealed partial class PantographSaveState : SaveStateBase
    {
        public PantographState PantographState { get; set; }
        public double Time { get; set; }
        public double Delay { get; set; }
    }
}