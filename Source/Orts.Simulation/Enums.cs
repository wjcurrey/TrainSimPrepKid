﻿using System.ComponentModel;

namespace Orts.Simulation
{
    public enum SignalItemFindState
    {
        None = 0,
        Item = 1,
        EndOfTrack = -1,
        PassedDanger = -2,
        PassedMaximumDistance = -3,
        TdbError = -4,
        EndOfAuthority = -5,
        EndOfPath = -6,
    }

    public enum SignalItemType
    {
        Any,
        Signal,
        SpeedLimit,
    }

    public enum SpeedItemType
    {
        Standard = 0,
        TempRestrictedStart = 1,
        TempRestrictedResume = 2,
    }

    public enum TrainPathItemType
    {
        Signal,
        SpeedPost,
        Station,
        Authority,
        Reversal,
        OutOfControl,
        WaitingPoint,
        MilePost,
        FacingSwitch
    }

    public enum OutOfControlReason
    {
        [Description("SPAD")]PassedAtDanger,   //SignalPassedAtDanger
        [Description("SPAD-Rear")] RearPassedAtDanger,
        [Description("Misalg Sw")] MisalignedSwitch,
        [Description("Off Auth")] OutOfAuthority,
        [Description("Off Path")] OutOfPath,
        [Description("Splipped")] SlippedIntoPath,
        [Description("Slipped")] SlippedToEndOfTrack,
        [Description("Off Track")] OutOfTrack,
        [Description("Slip Turn")] SlippedIntoTurnTable,
        [Description("Undefined")] UnDefined
    }

    public enum EndAuthorityType
    {
        [Description("End Trck")]EndOfTrack,
        [Description("End Path")] EndOfPath,
        [Description("Switch")] ReservedSwitch,
        [Description("TrainAhd")] TrainAhead,
        [Description("Max Dist")] MaxDistance,
        [Description("Loop")] Loop,
        [Description("Signal")] Signal,                                       // in Manual mode only
        [Description("End Auth")] EndOfAuthority,                             // when moving backward in Auto mode
        [Description("No Path")] NoPathReserved,
    }

    public enum TrainType
    {
        Player,
        PlayerIntended,
        Static,
        Ai,
        AiNotStarted,
        AiAutoGenerated,
        Remote,
        AiPlayerDriven,     //Player is on board and is durrently driving train
        AiPlayerHosting,    //Player is on board, but train is currently autopiloted
        AiIncorporated,      // AI train is incorporated in other train
    }

}
