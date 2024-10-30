using System.Reflection;
using FCAP.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RWCustom;
using UnityEngine;

namespace FCAP.Hooks
{
    internal static class GraphicsHooks
    {
        public static void Apply()
        {
            On.PlayerGraphics.DrawSprites += NightcatEyesAndCameraBlink;
            On.PlayerGraphics.PlayerObjectLooker.HowInterestingIsThisObject += PlayerEyeInterest;
        }

        private static float PlayerEyeInterest(On.PlayerGraphics.PlayerObjectLooker.orig_HowInterestingIsThisObject orig, PlayerGraphics.PlayerObjectLooker self, PhysicalObject obj)
        {
            var val = orig(self, obj);
            if (DoorAnimatronic.IsAnimatronic(self.owner.player))
            {
                if (obj is Player p && !DoorAnimatronic.IsAnimatronic(p))
                {
                    // Prioritize first player, but treat any player as very cool to look at just in case
                    // Mostly for stuff like jolly coop and slugpups, should they be there for some reason
                    return p == self.owner.owner.room?.game?.FirstAnyPlayer?.realizedCreature ? float.MaxValue : 9999f;
                }
            }
            else if (self.owner.player is Player p && p.SlugCatClass == Constants.Nightguard && obj is OverseerCarcass)
            {
                // Ignore the accumulated overseer eyes
                return 0f;
            }
            return val;
        }

        private static void NightcatEyesAndCameraBlink(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);

            if (DoorAnimatronic.IsAnimatronic(self.player, out var controller))
            {
                self.blink = 0;
                for (int i = 0; i < sLeaser.sprites.Length; i++)
                {
                    var sprite = sLeaser.sprites[i];
                    sprite.isVisible = controller.left ? controller.game.LeftDoorLight : controller.game.RightDoorLight; // I hope this is compatible with DMS I have no clue how DMS works

                    if (i == 9 && controller.anim == Enums.Animatronic.Nightcat)
                    {
                        // always show eye sprite for night when power out
                        sprite.isVisible = !controller.flickerEyes || controller.flickerOn;
                        sprite.color = Color.white;
                    }
                    else if (i == 7 || i == 8)
                    {
                        // Hide arms
                        sprite.isVisible = false;
                    }
                    else
                    {
                        // Fuck you Jolly Coop, ruining my animatronics' colors!!!
                        var palette = rCam.currentPalette;
                        sprite.color = i == 9 ? palette.blackColor : PlayerGraphics.DefaultSlugcatColor(Enums.GetFakeSlug(controller.anim));
                        if (controller.anim == Enums.Animatronic.Nightcat)
                        {
                            sprite.color = Color.Lerp(palette.blackColor, Custom.HSL2RGB(0.63055557f, 0.54f, 0.5f), Mathf.Lerp(0.08f, 0.04f, palette.darkness));
                        }
                    }
                }
            }
            else if (GameController.Instance != null && GameController.Instance.InCams)
            {
                self.blink = 5;
            }
        }
    }
}
