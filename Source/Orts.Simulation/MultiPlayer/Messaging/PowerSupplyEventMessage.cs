﻿using MemoryPack;

using Orts.Common;
using Orts.Simulation.Physics;

namespace Orts.Simulation.Multiplayer.Messaging
{
    [MemoryPackable]
    public partial class PowerSupplyEventMessage : MultiPlayerMessageContent
    {
        public PowerSupplyEvent PowerSupplyEvent { get; set; }

        public int CarIndex { get; set; }

        public override void HandleMessage()
        {
            Train train = MultiPlayerManager.FindPlayerTrain(User);
            if (train == null)
                return;

            if (CarIndex > -1)
                train.SignalEvent(PowerSupplyEvent, CarIndex);
            else
                train.SignalEvent(PowerSupplyEvent);
        }
    }
}