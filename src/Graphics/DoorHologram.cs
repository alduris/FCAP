using System;
using MoreSlugcats;
using OverseerHolograms;
using RWCustom;
using UnityEngine;

namespace FCAP.Graphics
{
    internal partial class DoorHologram : OverseerHologram
    {
        internal enum Door
        {
            Left,
            Right,
            None
        }

        public Door door;
        public GameController game;
        public Vector2 midpoint = Vector2.zero;

        public DoorHologram(GameController game, Overseer overseer, Message message, Creature communicateWith, float importance) : base(overseer, message, communicateWith, importance)
        {
            this.game = game;
            var type = CWTs.GetTask(overseer);
            door = type switch
            {
                Enums.OverseerTask.LeftDoor => Door.Left,
                Enums.OverseerTask.RightDoor => Door.Right,
                _ => Door.None,
            };

            if (door != Door.None)
            {
                Vector2 posTL = door == Door.Left ? new(213, 573) : new(731, 573);
                Vector2 posTR = door == Door.Left ? new(227, 577) : new(744, 577);
                Vector2 posBL = door == Door.Left ? new(213, 395) : new(731, 395);
                Vector2 posBR = door == Door.Left ? new(227, 406) : new(744, 406);

                midpoint = (posTL + posTR + posBL + posBR) / 4f;

                parts.Add(new DoorFramePart(posTL - midpoint, posTR - midpoint, posBL - midpoint, posBR - midpoint, this, totalSprites));
                parts.Add(new DoorBarrierParts(posTL - midpoint, posTR - midpoint, posBL - midpoint, posBR - midpoint, this, totalSprites));
            }
        }

        public DoorHologram(AncientBot robo, Message message, Creature communicateWith, float importance) : base(robo, message, communicateWith, importance)
        {
            throw new NotImplementedException();
        }

        public override float DisplayPosScore(IntVector2 testPos)
        {
            var pos = room.MiddleOfTile(testPos);
            return Mathf.Pow(10f, Custom.Dist(midpoint, pos));
        }

        public override void Update(bool eu)
        {
            base.Update(eu);

            if (door == Door.None)
            {
                Destroy();
            }

            stillRelevant = !game.OutOfPower;
        }
    }
}
