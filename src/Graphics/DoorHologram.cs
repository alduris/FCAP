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
        public PositionedSoundEmitter sound;

        public DoorFramePart frame;
        public DoorBarrierParts barriers;

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
                Vector2 posTL = door == Door.Left ? new(223, 573) : new(744, 573);
                Vector2 posTR = door == Door.Left ? new(237, 577) : new(731, 577);
                Vector2 posBL = door == Door.Left ? new(223, 395) : new(744, 395);
                Vector2 posBR = door == Door.Left ? new(237, 406) : new(731, 406);

                midpoint = (posTL + posTR + posBL + posBR) / 4f;

                frame = new DoorFramePart(posTL - midpoint, posTR - midpoint, posBL - midpoint, posBR - midpoint, this, totalSprites);
                parts.Add(frame);
                barriers = new DoorBarrierParts(posTL - midpoint, posTR - midpoint, posBL - midpoint, posBR - midpoint, this, totalSprites);
                parts.Add(barriers);
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

        public override float InfluenceHoverScoreOfTile(IntVector2 testTile, float f)
        {
            float num = 0;
            if (door == Door.Left && testTile.x * 20f < midpoint.x) num -= 200 * Mathf.Abs(testTile.x * 20f - midpoint.x);
            if (door == Door.Right && testTile.x * 20f > midpoint.x) num -= 200 * Mathf.Abs(testTile.x * 20f - midpoint.x);
            return 10f * Mathf.Abs(testTile.y * 20f - midpoint.y) + 2f * Mathf.Abs(testTile.x * 20f - midpoint.x) + num;
        }

        public override void Update(bool eu)
        {
            base.Update(eu);

            if (door == Door.None)
            {
                Destroy();
            }

            if (sound == null)
            {
                sound = new PositionedSoundEmitter(pos, 0f, 1f);
                room.PlaySound(SoundID.Overseer_Image_LOOP, sound, true, 0f, 1f, false);
                sound.requireActiveUpkeep = true;
            }
            else
            {
                sound.alive = true;
                sound.pos = pos;
                sound.volume = Mathf.Pow(fade, 0.25f);
                if (sound.slatedForDeletetion && !sound.soundStillPlaying)
                {
                    sound = null;
                }
            }

            stillRelevant = !game.OutOfPower;
        }
    }
}
