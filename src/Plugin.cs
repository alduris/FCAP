using System;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using BepInEx;
using UnityEngine;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;
using RWCustom;
using FCAP.Hooks;
using BepInEx.Logging;
using MonoMod.RuntimeDetour;

#pragma warning disable CS0618
[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace FCAP
{
    [BepInPlugin("alduris.fcap", "Five Cycles at Pebbles", "0.1.0")]
    class Plugin : BaseUnityPlugin
    {
        public static new ManualLogSource Logger;

        // Add hooks
        public void OnEnable()
        {
            Logger = base.Logger;
            Logger.LogDebug("hi");

            try
            {
                On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);

                // Game stuff
                On.RainWorldGame.Update += RainWorldGame_Update;
                On.RoomSpecificScript.AddRoomSpecificScript += AddGameScript;
                On.Player.checkInput += NightguardInputRevamp;

                // Other stuff
                OverseerHooks.Apply();
                RainMeterHooks.Apply();
                _ = new Hook(typeof(RoomCamera).GetProperty(nameof(RoomCamera.DarkPalette), BindingFlags.NonPublic | BindingFlags.Instance)!.GetGetMethod(true), PowerOutDarkFader);

                Logger.LogDebug("yay");
            }
            catch (Exception ex)
            {
                Logger.LogError("boowomp");
                Logger.LogError(ex);
            }
        }

        private void RainWorldGame_Update(On.RainWorldGame.orig_Update orig, RainWorldGame self)
        {
            orig(self);
            if (self.devToolsActive && Input.GetKeyDown(KeyCode.Backslash))
            {
                Debug.Log((Vector2)Futile.mousePosition + self.cameras[0].pos);
            }
        }

        private void AddGameScript(On.RoomSpecificScript.orig_AddRoomSpecificScript orig, Room room)
        {
            orig(room);

            if (room.abstractRoom.name == "SS_FCAP")
            {
                room.AddObject(new GameController(room));
            }
        }

        private void NightguardInputRevamp(On.Player.orig_checkInput orig, Player self)
        {
            if (self.playerState.playerNumber == 0 && self.SlugCatClass == Constants.Nightguard && GameController.Instance != null)
            {
                var game = GameController.Instance;
                for (int i = self.input.Length - 1; i > 0; i--)
                {
                    self.input[i] = self.input[i - 1];
                }

                var currInput = RWInput.PlayerInput(0, Custom.rainWorld);
                var lastInput = CWTs.LastInput(self);
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
                    self.input[0] = new Player.InputPackage(controls.gamePad, controls.GetActivePreset(), currInput.x, currInput.y, currInput.jmp, false, false, false, currInput.crouchToggle);

                    // Toggle door or cams
                    if (currInput.pckp && !lastInput.pckp && !game.OutOfPower)
                    {
                        float x = self.bodyChunks[0].pos.x / self.room.PixelWidth;
                        switch (x)
                        {
                            case < 0.45f:
                                {
                                    game.ToggleDoor(Map.Direction.Left);
                                    break;
                                }
                            case < 0.55f:
                                {
                                    game.ToggleCams();
                                    break;
                                }
                            default:
                                {
                                    game.ToggleDoor(Map.Direction.Right);
                                    break;
                                }
                        }
                    }
                }
                CWTs.UpdateLastInput(self, currInput);
            }
            else
            {
                orig(self);
            }
        }
        
        private float PowerOutDarkFader(Func<RoomCamera, float> orig, RoomCamera self)
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


        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld)
        {
            Constants.Register();
        }
    }
}