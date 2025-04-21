﻿using FCAP.Graphics;
using UnityEngine;
using static FCAP.Map;

namespace FCAP.AI
{
    /// <summary>
    /// A sort of combination between Freddy and Foxy. Quick to move towards you, then you hope that your door isn't open.
    /// In cameras, likes the dark.
    /// </summary>
    internal class NightcatAI(GameController game, int night) : BaseAI(game, Enums.Animatronic.Nightcat, Location.SecondaryStage, NightDifficulties[night], 368)
    {
        private static readonly int CamAdditionalWait = 280;
        private static readonly int[] NightDifficulties = [0, 0, 1, 3, 5, 7, -1];
        private const float ReturnHomeChance = 0.45f;

        private bool WaitingForCamDown = false;
        private bool ForceMove = false;

        protected override int PowerDrainOnLeave => 50;

        private enum EvilPhase
        {
            Init,
            Stage,
            Door,
            Suspense,
            Jumpscare
        }
        private EvilPhase OOPPhase;
        private int OOPCountdown = 0;

        public override void Update()
        {
            if (game.OutOfPower)
            {
                OOPCountdown--;
                switch (OOPPhase)
                {
                    case EvilPhase.Init:
                        location = Location.SecondaryStage;
                        OOPPhase = EvilPhase.Stage;
                        OOPCountdown = Random.Range(120, 400);
                        break;
                    case EvilPhase.Stage:
                        if (OOPCountdown <= 0)
                        {
                            location = Random.value < 0.5 ? Location.LeftDoor : Location.RightDoor;
                            OOPPhase = EvilPhase.Door;
                            OOPCountdown = Random.Range(1, 5) * 40 * 5; // intervals of exactly 5 seconds

                            // Play song
                            MusicEvent musicEvent = new()
                            {
                                songName = "toreador-march-wawa",
                                cyclesRest = 0,
                                prio = 1f,
                                volume = 0.3f
                            };
                            game.room.game.manager.musicPlayer?.GameRequestsSong(musicEvent);

                            // Spawn animatronic
                            doorRepresentation = new DoorAnimatronic(game.room, game, animatronic, location == Location.LeftDoor, true) { flickerEyes = true };
                            game.room.AddObject(doorRepresentation);
                        }
                        break;
                    case EvilPhase.Door:
                        if (OOPCountdown <= 0)
                        {
                            location = Location.You;
                            OOPPhase = EvilPhase.Suspense;
                            OOPCountdown = 160 + Random.Range(0, 12) * 40;
                            // Stop music
                            var stopEvent = new StopMusicEvent()
                            {
                                fadeOutTime = 0,
                                type = StopMusicEvent.Type.AllSongs,
                                prio = 1f,
                            };
                            game.room.game.manager.musicPlayer?.GameRequestsSongStop(stopEvent);

                            // Despawn animatronic
                            doorRepresentation.Destroy();
                        }
                        break;
                    case EvilPhase.Suspense:
                        if (OOPCountdown <= 0)
                        {
                            game.CurrentJumpscare = animatronic;
                            game.room.AddObject(game.jumpscareObj ??= new Jumpscare(game.room, animatronic));
                            OOPPhase = EvilPhase.Jumpscare;
                        }
                        break;
                    case EvilPhase.Jumpscare:
                        break;
                }
            }
            else
            {
                if (ForceMove)
                {
                    if (GameController.Instance.InCams)
                    {
                        counter = 2;
                    }
                    else if (WaitingForCamDown)
                    {
                        WaitingForCamDown = false;
                        counter = CamAdditionalWait;
                    }
                }
                else if (counter == 1 && GameController.Instance.InCams && GameController.Instance.CamSelected == location)
                {
                    counter = 2;
                    WaitingForCamDown = true;
                    ForceMove = true;
                }
                base.Update();
            }
        }

        public override bool MoveCheck()
        {
            bool b = base.MoveCheck() && (!WaitingForCamDown || ForceMove || location == Location.LeftDoor || location == Location.RightDoor);
            if (b) ForceMove = false;
            return b;
        }

        public override bool CanJumpscare()
        {
            if (game.OutOfPower)
            {
                return Random.value < 0.25f;
            }
            return ((location == Location.LeftDoor && !game.LeftDoorShut) || (location == Location.RightDoor && !game.RightDoorShut));
        }

        public override Location NextMove()
        {
            return location switch
            {
                Location.SecondaryStage => Random.value < 0.5f ? Location.LeftHall : Location.Storage,

                Location.LeftHall => Location.LeftDoor,
                Location.LeftDoor => Random.value < ReturnHomeChance ? Location.SecondaryStage : Location.LeftDoor,

                Location.Storage => Location.RightDoor,
                Location.RightDoor => Random.value < ReturnHomeChance ? Location.SecondaryStage : Location.RightDoor,
                _ => location
            };
        }
    }
}
