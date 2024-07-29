using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace FCAP.Graphics
{
    internal class DoorAnimatronic : UpdatableAndDeletable
    {
        internal static ConditionalWeakTable<Player, DoorAnimatronic> animatronicShowCWT = new();

        public GameController game;
        public Enums.Animatronic anim;
        public Player player = null;
        public bool left;

        public DoorAnimatronic(Room room, GameController controller, Enums.Animatronic animatronic, bool left)
        {
            this.room = room;
            this.left = left;
            game = controller;
            anim = animatronic;

            var absCre = new AbstractCreature(room.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Slugcat), null, new WorldCoordinate(room.abstractRoom.index, -1, -1, -1), room.game.GetNewID());
            absCre.state = new PlayerState(absCre, 0, Enums.GetFakeSlug(animatronic), true);
            absCre.saveCreature = false;
            room.abstractRoom.AddEntity(absCre);
            absCre.RealizeInRoom();

            player = absCre.realizedCreature as Player;
            player.SuperHardSetPosition(new Vector2(left ? 190 : 780, 400));
            player.controller = new Player.NullController();

            animatronicShowCWT.Add(player, this);
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            player.SuperHardSetPosition(new Vector2(left ? 190 : 780, 400));
        }

        public override void Destroy()
        {
            base.Destroy();
            if (player != null)
            {
                player.Die();
                player.Destroy();
                player.abstractCreature.Destroy();
                animatronicShowCWT.Remove(player);
                player = null;
            }
        }
    }
}
