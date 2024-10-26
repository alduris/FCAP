using System.Runtime.CompilerServices;
using UnityEngine;

namespace FCAP.Graphics
{
    public class DoorAnimatronic : UpdatableAndDeletable
    {
        internal static ConditionalWeakTable<Player, DoorAnimatronic> animatronicShowCWT = new();
        public static bool IsAnimatronic(Player player) => animatronicShowCWT.TryGetValue(player, out _);

        public GameController game;
        public Enums.Animatronic anim;
        public Player player = null;
        public bool left;
        public Vector2 stayPos;

        // Used by nightcat only
        public bool flickerEyes = false;
        private int flickerCounter = Random.Range(10, 30);
        public bool flickerOn = true;

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
            stayPos = new Vector2(left ? 190 : 780, 400);
            player.SuperHardSetPosition(stayPos);
            player.controller = new Player.NullController();

            animatronicShowCWT.Add(player, this);
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            if (player == null) return;

            player.standing = true;

            if (flickerEyes)
            {
                flickerCounter--;
                if (flickerCounter <= 0)
                {
                    flickerCounter = Random.Range(4, 10);
                    flickerOn = !flickerOn;
                }
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            if (player == null) return;

            player.Destroy();
            player.abstractCreature.Destroy();
            animatronicShowCWT.Remove(player);
            player = null;

            if (left)
            {
                game.LeftDoorLightCounter = 0;
            }
            else
            {
                game.RightDoorLightCounter = 0;
            }
        }
    }
}
