using System;
using RWCustom;
using UnityEngine;

namespace FCAP
{
    internal static class Global
    {
        public const int MaxPower = 24000;

        public enum Animatronic // todo: come up with more original characters than these?
        {
            Sluggy,
            Greenie,
            Pinky,
            Bluey,
            None
        }
        public enum PowerStage
        {
            HasPower,
            Drone,
            MusicBox,
            Waiting
        }
    }
}
