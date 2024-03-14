using System;
using System.Runtime.CompilerServices;
using FCAP.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MoreSlugcats;
using OverseerHolograms;
using UnityEngine;
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
            On.Overseer.TryAddHologram += Overseer_TryAddHologram;
            On.Room.AddObject += Room_AddObject;
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
            if (GameController.Instance != null && CWTs.HasTask(self))
            {
                var value = CWTs.GetTask(self);
                bool destroy = false;

                switch (value)
                {
                    case Enums.OverseerTask.Cameras:
                        {
                            destroy = !GameController.Instance.InCams;

                            if (self.hologram == null)
                            {
                                self.TryAddHologram(MoreSlugcatsEnums.OverseerHologramMessage.Advertisement, null, float.MaxValue); // temporary
                                //self.TryAddHologram(Constants.CamsHologram, null, float.MaxValue);
                            }
                            break;
                        }
                    case Enums.OverseerTask.LeftDoor:
                        {
                            destroy = !GameController.Instance.LeftDoorShut;

                            if (self.hologram == null)
                            {
                                self.TryAddHologram(DoorHolo, null, float.MaxValue);
                            }
                            break;
                        }
                    case Enums.OverseerTask.RightDoor:
                        {
                            destroy = !GameController.Instance.RightDoorShut;

                            if (self.hologram == null)
                            {
                                self.TryAddHologram(DoorHolo, null, float.MaxValue);
                            }
                            break;
                        }
                }

                if (destroy)
                {
                    self.hologram?.Destroy();
                    self.hologram = null;
                    self.Die();
                }
            }
        }

        private static void Overseer_TryAddHologram(On.Overseer.orig_TryAddHologram orig, Overseer self, OverseerHologram.Message message, Creature communicateWith, float importance)
        {
            orig(self, message, communicateWith, importance);

            if (self.hologram == null)
            {
                if (message == CamsHolo)
                {
                    //
                }
                else if (message == DoorHolo)
                {
                    self.hologram = new DoorHologram(GameController.Instance, self, message, null, float.MaxValue);
                }
                self.room.AddObject(self.hologram);
            }
        }

        private static void Room_AddObject(On.Room.orig_AddObject orig, Room self, UpdatableAndDeletable obj)
        {
            if (obj != null)
            {
                orig(self, obj);
            }
        }
    }
}
