using FCAP.Graphics;
using UnityEngine;

namespace FCAP.Hooks
{
    internal static class GraphicsHooks
    {
        public static void Apply()
        {
            On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
            On.PlayerGraphics.PlayerObjectLooker.HowInterestingIsThisObject += PlayerObjectLooker_HowInterestingIsThisObject;
        }

        private static float PlayerObjectLooker_HowInterestingIsThisObject(On.PlayerGraphics.PlayerObjectLooker.orig_HowInterestingIsThisObject orig, PlayerGraphics.PlayerObjectLooker self, PhysicalObject obj)
        {
            var val = orig(self, obj);
            if (DoorAnimatronic.animatronicShowCWT.TryGetValue(self.owner.player, out _))
            {
                if (obj is Player p && !DoorAnimatronic.animatronicShowCWT.TryGetValue(p, out _))
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

        private static void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);

            if (DoorAnimatronic.animatronicShowCWT.TryGetValue(self.player, out var controller))
            {
                self.blink = 0;
                for (int i = 0; i < sLeaser.sprites.Length; i++)
                {
                    var sprite = sLeaser.sprites[i];
                    sprite.isVisible = controller.left ? controller.game.LeftDoorLight : controller.game.RightDoorLight; // I hope this is compatible with DMS I have no clue how DMS works

                    // always show eye sprite for night when power out
                    if (i == 9 && controller.anim == Enums.Animatronic.Nightcat)
                    {
                        sprite.isVisible = !controller.flickerEyes || controller.flickerOn;
                        sprite.color = Color.white;
                    }
                }
            }
        }
    }
}
