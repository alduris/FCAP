using System;
using MonoMod.RuntimeDetour;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;
using FCAP.Graphics;
using MonoMod.Cil;

namespace FCAP.Hooks
{
    internal static class GameplayHooks
    {
        public static void Apply()
        {
            On.RoomSpecificScript.AddRoomSpecificScript += AddGameScript;
            On.Player.checkInput += NightguardInputRevamp;
            IL.Player.Die += PlayerNoPlayDeathSound;
            On.HUD.TextPrompt.EnterGameOverMode += HUDNoPlayGameOverSound;
            On.HUD.TextPrompt.Update += HUDNoGameOverPrompt;
            _ = new Hook(typeof(RoomCamera).GetProperty(nameof(RoomCamera.DarkPalette), BindingFlags.NonPublic | BindingFlags.Instance)!.GetGetMethod(true), PowerOutDarkFader);
            _ = new Hook(typeof(RoomCamera).GetProperty(nameof(RoomCamera.roomSafeForPause)).GetGetMethod(false), NoPauseWhenGameOver);
            On.RainCycle.GetDesiredCycleLength += RainCycle_GetDesiredCycleLength;
        }

        private static int RainCycle_GetDesiredCycleLength(On.RainCycle.orig_GetDesiredCycleLength orig, RainCycle self)
        {
            if (self.world.game.IsStorySession && self.world.game.StoryCharacter == Constants.Nightguard)
            {
                return Constants.CycleLength;
            }
            else
            {
                return orig(self);
            }
        }

        private static bool NoPauseWhenGameOver(Func<RoomCamera, bool> orig, RoomCamera self) => orig(self) || (GameController.Instance != null && GameController.Instance.OutOfPower);

        private static void AddGameScript(On.RoomSpecificScript.orig_AddRoomSpecificScript orig, Room room)
        {
            orig(room);

            if (room.abstractRoom.name == Constants.RoomName && room.game.IsStorySession && room.game.StoryCharacter == Constants.Nightguard)
            {
                room.AddObject(new GameController(room));
            }
        }

        private static void NightguardInputRevamp(On.Player.orig_checkInput orig, Player self)
        {
            if (DoorAnimatronic.IsAnimatronic(self)) return;

            if (GameController.Instance != null && self.room.abstractRoom.name == Constants.RoomName && self.controller is not Player.NullController && !self.isNPC && !self.playerState.isGhost)
            {
                bool isFirstPlayer = self.playerState.playerNumber == 0;
                var game = GameController.Instance;

                for (int i = self.input.Length - 1; i > 0; i--)
                {
                    self.input[i] = self.input[i - 1];
                }

                var currInput = RWInput.PlayerInput(0);
                var lastInput = CWTs.LastInput(self);

                // Only control cams/lights/doors if first player
                if (isFirstPlayer)
                {
                    var controls = self.room.game.rainWorld.options.controls[0];
                    if (game.InCams)
                    {
                        // In cams mode, player cannot move
                        self.input[0] = new Player.InputPackage(controls.gamePad, controls.GetActivePreset(), 0, 0, false, false, false, false, false);

                        // Stop using cams if grab
                        if (currInput.pckp && !lastInput.pckp)
                        {
                            game.ToggleCams();
                        }

                        // Switch cams
                        if (currInput.jmp && !lastInput.jmp)
                            game.SwitchCamViewing();
                        // Or switch cam selection
                        else if (currInput.x > 0 && lastInput.x <= 0 && currInput.y == 0)
                            game.SwitchCamSelecting(Map.Direction.Right);
                        else if (currInput.x < 0 && lastInput.x >= 0 && currInput.y == 0)
                            game.SwitchCamSelecting(Map.Direction.Left);
                        else if (currInput.y > 0 && lastInput.y <= 0 && currInput.x == 0)
                            game.SwitchCamSelecting(Map.Direction.Up);
                        else if (currInput.y < 0 && lastInput.y >= 0 && currInput.x == 0)
                            game.SwitchCamSelecting(Map.Direction.Down);
                    }
                    else
                    {
                        // Controls
                        float x = self.bodyChunks[0].pos.x / self.room.PixelWidth;
                        if (!game.OutOfPower)
                        {
                            if (currInput.pckp)
                            {
                                switch (x)
                                {
                                    case < Constants.LeftThresh:
                                        {
                                            currInput.x = 0;
                                            if (game.LeftDoorLightCounter <= 0)
                                                game.LeftDoorLightCounter = Random.Range(5, 20);
                                            break;
                                        }
                                    case > Constants.RightThresh:
                                        {
                                            currInput.x = 0;
                                            if (game.RightDoorLightCounter <= 0)
                                                game.RightDoorLightCounter = Random.Range(5, 20);
                                            break;
                                        }
                                    default:
                                        {
                                            if (!lastInput.pckp)
                                                game.ToggleCams();
                                            break;
                                        }
                                }
                            }

                            if (currInput.thrw && !lastInput.thrw)
                            {
                                switch (x)
                                {
                                    case < Constants.LeftThresh:
                                        {
                                            game.ToggleDoor(Map.Direction.Left);
                                            break;
                                        }
                                    case > Constants.RightThresh:
                                        {
                                            game.ToggleDoor(Map.Direction.Right);
                                            break;
                                        }
                                    default:
                                        {
                                            game.phoneGuy?.Destroy();
                                            break;
                                        }
                                }
                                if (x < Constants.LeftThresh)
                                {
                                }
                                else if (x > Constants.RightThresh)
                                {
                                }
                            }
                        }

                        // In not cams mode, player can move but cannot throw, pick up, or use map
                        int inpX = (currInput.x < 0 && x < 0.3f) || (currInput.x > 0 && x > 0.7f) ? 0 : currInput.x;
                        self.input[0] = new Player.InputPackage(controls.gamePad, controls.GetActivePreset(), inpX, currInput.y, currInput.jmp, false, false, false, currInput.crouchToggle);
                    }
                }
                else
                {
                    // Other players that aren't player 1:
                    // Don't escape the room. But you can run around inside of it :)
                    var controls = self.room.game.rainWorld.options.controls[self.playerState.playerNumber];
                    float x = self.bodyChunks[0].pos.x / self.room.PixelWidth;
                    int inpX = (currInput.x < 0 && x < 0.3f) || (currInput.x > 0 && x > 0.7f) ? 0 : currInput.x;
                    self.input[0] = new Player.InputPackage(controls.gamePad, controls.GetActivePreset(), inpX, currInput.y, currInput.jmp, false, false, false, currInput.crouchToggle);
                }

                CWTs.UpdateLastInput(self, currInput);
            }
            else
            {
                orig(self);
            }
        }

        private static void PlayerNoPlayDeathSound(ILContext il)
        {
            // Make it so we don't play death sound if dying to a jumpscare
            var c = new ILCursor(il);

            c.GotoNext(MoveType.After, x => x.MatchLdsfld<SoundID>(nameof(SoundID.UI_Slugcat_Die)));
            c.EmitDelegate((SoundID old) => GameController.Instance != null && GameController.Instance.CurrentJumpscare != Enums.Animatronic.None ? SoundID.None : old);
        }

        private static void HUDNoPlayGameOverSound(On.HUD.TextPrompt.orig_EnterGameOverMode orig, HUD.TextPrompt self, Creature.Grasp dependentOnGrasp, int foodInStomach, int deathRoom, Vector2 deathPos)
        {
            orig(self, dependentOnGrasp, foodInStomach, deathRoom, deathPos);
            if (GameController.Instance != null && GameController.Instance.CurrentJumpscare != Enums.Animatronic.None)
                self.playGameOverSound = false;
        }

        private static void HUDNoGameOverPrompt(On.HUD.TextPrompt.orig_Update orig, HUD.TextPrompt self)
        {
            bool oldGOM = self.gameOverMode;
            self.gameOverMode &= (GameController.Instance == null || GameController.Instance.CurrentJumpscare == Enums.Animatronic.None);
            orig(self);
            self.gameOverMode = oldGOM;
        }

        private static float PowerOutDarkFader(Func<RoomCamera, float> orig, RoomCamera self)
        {
            if (GameController.Instance != null)
            {
                return 1f - Mathf.Pow((float)Math.E, GameController.Instance.OOPTimer / -4f);
            }
            else
            {
                return orig(self);
            }
        }
    }
}
