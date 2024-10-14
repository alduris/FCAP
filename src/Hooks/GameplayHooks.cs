using System;
using MonoMod.RuntimeDetour;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;
using FCAP.Graphics;

namespace FCAP.Hooks
{
    internal static class GameplayHooks
    {
        public static void Apply()
        {
            On.RoomSpecificScript.AddRoomSpecificScript += AddGameScript;
            On.Player.checkInput += NightguardInputRevamp;
            On.RainWorldGame.ShowPauseMenu += NoPauseWhenGameOver;
            _ = new Hook(typeof(RoomCamera).GetProperty(nameof(RoomCamera.DarkPalette), BindingFlags.NonPublic | BindingFlags.Instance)!.GetGetMethod(true), PowerOutDarkFader);
        }

        private static void NoPauseWhenGameOver(On.RainWorldGame.orig_ShowPauseMenu orig, RainWorldGame self)
        {
            if (GameController.Instance == null || !GameController.Instance.OutOfPower)
            {
                orig(self);
            }
        }

        private static void AddGameScript(On.RoomSpecificScript.orig_AddRoomSpecificScript orig, Room room)
        {
            orig(room);

            if (room.abstractRoom.name == "SS_FCAP" && room.game.IsStorySession && room.game.StoryCharacter == Constants.Nightguard)
            {
                room.AddObject(new GameController(room));
            }
        }

        private static void NightguardInputRevamp(On.Player.orig_checkInput orig, Player self)
        {
            if (DoorAnimatronic.IsAnimatronic(self)) return;

            if (GameController.Instance != null && self.room.abstractRoom.name == "SS_FCAP" && self.controller is not Player.NullController && !self.isNPC && !self.playerState.isGhost)
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
                        // In not cams mode, player can move but cannot throw, pick up, or use map
                        float x = self.bodyChunks[0].pos.x / self.room.PixelWidth;
                        int inpX = (currInput.x < 0 && x < 0.3f) || (currInput.x > 0 && x > 0.7f) ? 0 : currInput.x;
                        self.input[0] = new Player.InputPackage(controls.gamePad, controls.GetActivePreset(), inpX, currInput.y, currInput.jmp, false, false, false, currInput.crouchToggle);

                        // Controls
                        if (!game.OutOfPower)
                        {
                            if (currInput.pckp)
                            {
                                switch (x)
                                {
                                    case < 0.425f:
                                        {
                                            if (game.LeftDoorLightCounter <= 0)
                                                game.LeftDoorLightCounter = Random.Range(5, 20);
                                            break;
                                        }
                                    case < 0.575f:
                                        {
                                            if (!lastInput.pckp)
                                                game.ToggleCams();
                                            break;
                                        }
                                    default:
                                        {
                                            if (game.RightDoorLightCounter <= 0)
                                                game.RightDoorLightCounter = Random.Range(5, 20);
                                            break;
                                        }
                                }
                            }

                            if (currInput.thrw && !lastInput.thrw)
                            {
                                if (x < 0.45f)
                                {
                                    game.ToggleDoor(Map.Direction.Left);
                                }
                                else if (x > 0.55f)
                                {
                                    game.ToggleDoor(Map.Direction.Right);
                                }
                            }
                        }
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

        private static float PowerOutDarkFader(Func<RoomCamera, float> orig, RoomCamera self)
        {
            if (GameController.Instance != null)
            {
                return 2f * Mathf.Atan(GameController.Instance.OOPTimer / 4f) / Mathf.PI;
            }
            else
            {
                return orig(self);
            }
        }
    }
}
