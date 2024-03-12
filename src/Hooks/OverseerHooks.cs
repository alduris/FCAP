using System;
using static FCAP.Constants;

namespace FCAP.Hooks
{
    internal static class OverseerHooks
    {
        public static void Apply()
        {
            On.OverseerAbstractAI.RoomAllowed += OverseerAbstractAI_RoomAllowed;
            On.OverseerAbstractAI.PlayerGuideUpdate += OverseerAbstractAI_PlayerGuideUpdate;
            On.Overseer.Update += Overseer_Update;
        }

        private static bool OverseerAbstractAI_RoomAllowed(On.OverseerAbstractAI.orig_RoomAllowed orig, OverseerAbstractAI self, int room)
        {
            // Overseer only allowed in SS_FCAP in nightguard
            return orig(self, room) && (!self.world.game.IsStorySession || self.world.game.StoryCharacter != Nightguard || self.world.GetAbstractRoom(room).name == "SS_FCAP");
        }

        private static void OverseerAbstractAI_PlayerGuideUpdate(On.OverseerAbstractAI.orig_PlayerGuideUpdate orig, OverseerAbstractAI self, int time)
        {
            // Overseer want to stay with player unless power is out in which case they promptly die
            orig(self, time);

            if (self.world.game.IsStorySession && self.world.game.StoryCharacter == Nightguard && GameController.Instance != null)
            {
                if (GameController.Instance.OutOfPower)
                {
                    self.goToPlayer = false;
                    self.parent.Die();
                }
                else
                {
                    self.goToPlayer = true;
                    self.playerGuideCounter = 1000;
                }
            }
        }

        private static void Overseer_Update(On.Overseer.orig_Update orig, Overseer self, bool eu)
        {
            // Overseer wants to display cams
            throw new NotImplementedException();
        }
    }
}
