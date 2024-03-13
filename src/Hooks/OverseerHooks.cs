using System;
using System.Runtime.CompilerServices;
using MonoMod.Cil;
using MoreSlugcats;
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
            On.Overseer.TryAddHologram += Overseer_TryAddHologram1;
            IL.Overseer.TryAddHologram += Overseer_TryAddHologram;
            On.WorldLoader.OverseerSpawnConditions += WorldLoader_OverseerSpawnConditions;
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

            if (GameController.Instance != null)
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
            orig(self, eu);
            if (GameController.Instance != null && GameController.Instance.InCams && self.PlayerGuide && self.hologram == null)
            {
                if (GameController.Instance.InCams)
                {
                    if (self.hologram == null)
                    {
                        self.TryAddHologram(MoreSlugcatsEnums.OverseerHologramMessage.Advertisement, null, float.MaxValue); // temporary
                        //self.TryAddHologram(Constants.CamsHologram, null, float.MaxValue);
                    }
                }
                else if (self.hologram != null)
                {
                    self.hologram.Destroy();
                    self.hologram = null;
                }
            }
        }

        private static void Overseer_TryAddHologram(ILContext il)
        {
            var c = new ILCursor(il);

            if (c.TryGotoNext(x => x.MatchLdarg(0), x => x.MatchLdfld<Overseer>(nameof(Overseer.hologram)), x => x.MatchCallvirt<Room>(nameof(Room.AddObject))))
            {
                c.MoveAfterLabels();
            }
            else
            {
                Plugin.Logger.LogWarning("Overseer.TryAddHologram hook did not match!");
            }
        }

        private static void Overseer_TryAddHologram1(On.Overseer.orig_TryAddHologram orig, Overseer self, OverseerHolograms.OverseerHologram.Message message, Creature communicateWith, float importance)
        {
            if (GameController.Instance == null || message == CamsHologram || (ModManager.MSC && message == MoreSlugcatsEnums.OverseerHologramMessage.Advertisement))
            {
                orig(self, message, communicateWith, importance);
            }
        }

        private static bool WorldLoader_OverseerSpawnConditions(On.WorldLoader.orig_OverseerSpawnConditions orig, WorldLoader self, SlugcatStats.Name character)
        {
            return orig(self, character) || character == Nightguard;
        }
    }
}
