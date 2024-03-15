using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FCAP.Graphics
{
    internal class MapDisplay : UpdatableAndDeletable, IDrawable
    {
        public GameController game;

        private int sin;

        public MapDisplay(GameController game, Room room)
        {
            this.game = game;
            this.room = room;
            room.AddObject(this);
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            sin++;
        }

        private int GetIndex(Map.Location loc) => loc switch
        {
            Map.Location.ShowStage => 1,
            Map.Location.DiningArea => 2,
            Map.Location.PirateCove => 3,
            Map.Location.PlayArea => 4,
            Map.Location.PartyRoomA => 5,
            Map.Location.PartyRoomB => 6,
            Map.Location.PartyRoomC => 7,
            Map.Location.PartyRoomD => 8,
            Map.Location.MainEntrance => 9,
            Map.Location.Backstage => 10,
            Map.Location.Kitchen => 11,
            Map.Location.RestroomHall => 12,
            Map.Location.Storage => 13,
            Map.Location.BackHall => 14,
            _ => throw new NotImplementedException()
        };

        public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[15];
            for (int i = 0; i < 15; i++)
            {
                var sprite = sLeaser.sprites[i] = new FSprite("pixel");
                sprite.shader = rCam.game.rainWorld.Shaders["CustomDepth"];
                sprite.alpha = 1f - (1 / 30f);
                sprite.scale = 16;
            }

            sLeaser.sprites[0].scale = 16f * 20f;

            AddToContainer(sLeaser, rCam, null);
        }

        public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            ApplyPalette(sLeaser, rCam, rCam.currentPalette, timeStacker);

            sLeaser.sprites[0] .SetPosition(new Vector2(646, 184) - camPos);
            sLeaser.sprites[1] .SetPosition(new Vector2(735, 257) - camPos);
            sLeaser.sprites[2] .SetPosition(new Vector2(729, 211) - camPos);
            sLeaser.sprites[3] .SetPosition(new Vector2(630, 263) - camPos);
            sLeaser.sprites[4] .SetPosition(new Vector2(630, 204) - camPos);
            sLeaser.sprites[5] .SetPosition(new Vector2(516, 262) - camPos);
            sLeaser.sprites[6] .SetPosition(new Vector2(516, 208) - camPos);
            sLeaser.sprites[7] .SetPosition(new Vector2(769, 262) - camPos);
            sLeaser.sprites[8] .SetPosition(new Vector2(769, 208) - camPos);
            sLeaser.sprites[9] .SetPosition(new Vector2(587, 270) - camPos);
            sLeaser.sprites[10].SetPosition(new Vector2(701, 290) - camPos);
            sLeaser.sprites[11].SetPosition(new Vector2(679, 179) - camPos);
            sLeaser.sprites[12].SetPosition(new Vector2(545, 66)  - camPos);
            sLeaser.sprites[13].SetPosition(new Vector2(633, 128) - camPos);
            sLeaser.sprites[14].SetPosition(new Vector2(621, 67)  - camPos);
        }

        public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette) => ApplyPalette(sLeaser, rCam, palette, 0f);

        public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette, float timeStacker)
        {
            const float LERP_AMT = 0.4f;
            foreach (var sprite in sLeaser.sprites)
            {
                sprite.color = Color.Lerp(Color.white, palette.fogColor, LERP_AMT);

                if (game.OutOfPower)
                {
                    sprite.isVisible &= Random.value > 0.04f && (sprite != sLeaser.sprites[0]);
                }
            }

            int viewedIndex = GetIndex(game.CamViewing);
            int selectedIndex = GetIndex(game.CamSelected);
            var greenColor = Color.Lerp(new Color(0f, 1f, 0f), palette.fogColor, LERP_AMT);

            if (viewedIndex != selectedIndex)
                sLeaser.sprites[viewedIndex].color = greenColor;

            var lerpSin = Mathf.Lerp(Mathf.Sin(sin * Mathf.Deg2Rad * 9f), Mathf.Sin((sin + 1) * Mathf.Deg2Rad * 9f), timeStacker);
            sLeaser.sprites[selectedIndex].color = Color.Lerp(sLeaser.sprites[selectedIndex].color, greenColor, lerpSin);
        }

        public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            newContatiner ??= rCam.ReturnFContainer("Foreground");
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                sLeaser.sprites[i].RemoveFromContainer();
                newContatiner.AddChild(sLeaser.sprites[i]);
            }
        }
    }
}
