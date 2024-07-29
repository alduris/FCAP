using System;
using RWCustom;
using UnityEngine;

namespace FCAP
{
    internal static class Enums
    {

        public enum Animatronic // todo: come up with more original characters than these?
        {
            Survivor,
            Monk,
            Hunter,
            Nightcat,
            Golden,
            None
        }

        public static SlugcatStats.Name GetFakeSlug(Animatronic animatronic)
        {
            return animatronic switch
            {
                Animatronic.Survivor => SlugcatStats.Name.White,
                Animatronic.Monk => SlugcatStats.Name.Yellow,
                Animatronic.Hunter => SlugcatStats.Name.Red,
                Animatronic.Nightcat => SlugcatStats.Name.Night,
                _ => throw new NotImplementedException()
            };
        }

        public enum PowerStage
        {
            HasPower,
            Drone,
            MusicBox,
            Waiting
        }

        public enum OverseerTask
        {
            LeftDoor,
            RightDoor,
            Cameras,
            None
        }
    }
}
