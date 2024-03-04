using System;
using RWCustom;
using UnityEngine;

namespace FCAP
{
    internal static class Global
    {
        public const int MaxPower = 40000;

        public enum Animatronic // todo: come up with more original characters than these?
        {
            Gourmand,
            Artificer,
            Rivulet,
            Spearmaster,
            Saint,
            None
        }
        public enum PowerStage
        {
            HasPower,
            Drone,
            MusicBox,
            Waiting
        }

        public static int Power = MaxPower;

        public static PowerStage OOPstage;
        public static int OOPTimer = 0;

        public static Map.Location CamViewing = Map.Location.ShowStage;
        public static Map.Location CamSelected = Map.Location.ShowStage;
        public static bool CamFlashlight = false;
        public static bool InCams = false;
        public static int CamViewTimer = 0;

        public static bool LeftDoorLight = false;
        public static bool LeftDoorShut = false;
        public static bool RightDoorLight = false;
        public static bool RightDoorShut = false;

        public static Animatronic CurrentJumpscare = Animatronic.None;
        public static int JumpscareTimer = 0;

        public static bool OutOfPower => Power <= 0;

        public static void Reset()
        {
            Power = MaxPower;

            OOPstage = PowerStage.HasPower;
            OOPTimer = 0;

            CamViewing = Map.Location.ShowStage;
            CamViewing = Map.Location.ShowStage;
            CamSelected = Map.Location.ShowStage;
            InCams = false;
            CamViewTimer = 0;
            
            LeftDoorLight = false;
            LeftDoorShut = false;
            RightDoorLight = false;
            RightDoorShut = false;
            
            CurrentJumpscare = Animatronic.None;
            JumpscareTimer = 0;
        }

        public static void Update()
        {
            // Power
            Power -= 1 + (LeftDoorLight ? 1 : 0) + (LeftDoorShut ? 1 : 0) + (RightDoorLight ? 1 : 0) + (RightDoorShut ? 1 : 0) + (InCams ? 1 : 0);

            if (OutOfPower)
            {
                InCams = false;
                LeftDoorLight = false;
                LeftDoorShut = false;
                RightDoorLight = false;
                RightDoorShut = false;
            }

            // Update timer thing
            if (InCams)
            {
                CamViewTimer++;
            }
            else
            {
                CamViewTimer = 0;
            }

            // Update jumpscare timer
            if (CurrentJumpscare != Animatronic.None)
            {
                JumpscareTimer++;
                if (JumpscareTimer > 30)
                {
                    // die
                    Application.Quit(); // temporary thing lol
                }
            }
        }
    }
}
