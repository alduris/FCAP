using RWCustom;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FCAP.Graphics
{
    internal class PowerDisplay : UpdatableAndDeletable, IDrawable
    {
        public GameController game;

        private bool UpdateVis = false;

        public PowerDisplay(GameController game, Room room)
        {
            this.game = game;
            this.room = room;
            room.AddObject(this);
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            UpdateVis = true;
        }

        public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[8];
            for (int i = 0; i < 6; i++)
            {
                var sprite = sLeaser.sprites[i] = new FSprite("pixel", true);
                sprite.shader = rCam.game.rainWorld.Shaders["CustomDepth"];
                sprite.alpha = 1f - (1 / 30f);
                sprite.scaleX = 18f;
                sprite.scaleY = 36f;
            }
            sLeaser.sprites[5].scaleY = 22f;

            for (int i = 6; i < 8; i++)
            {
                var tris = new TriangleMesh.Triangle[]
                {
                    new(0, 1, 2),
                    new(0, 2, 3),
                    new(2, 3, 4),
                    new(2, 4, 5),
                    new(4, 5, 6),
                    new(4, 6, 7),
                    new(0, 6, 7),
                    new(0, 1, 6),
                };
                var sprite = sLeaser.sprites[i] = new TriangleMesh("Futile_White", tris, false);
                sprite.shader = rCam.game.rainWorld.Shaders["CustomDepth"];
                sprite.alpha = 1f - (1 / 30f);
            }

            AddToContainer(sLeaser, rCam, null);
        }

        public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            ApplyPalette(sLeaser, rCam, rCam.currentPalette);

            for (int i = 0; i < 5; i++)
            {
                sLeaser.sprites[i].SetPosition(new Vector2(22f * i + 273f, 209f) - camPos);
                if (!game.OutOfPower)
                    sLeaser.sprites[i].isVisible = i < game.PowerUsage && (Random.value > 0.001f * 2 * game.PowerUsage || !UpdateVis);
            }

            var barSize = 200f * game.Power / Constants.MaxPower;
            sLeaser.sprites[5].scaleX = barSize;
            sLeaser.sprites[5].SetPosition(new Vector2(215f + barSize / 2f, 143f) - camPos);

            var indicatorVerts = (sLeaser.sprites[6] as TriangleMesh).vertices;
            indicatorVerts[0] = new Vector2(254, 237) - camPos;
            indicatorVerts[1] = new Vector2(261, 231) - camPos;
            indicatorVerts[2] = new Vector2(373, 231) - camPos;
            indicatorVerts[3] = new Vector2(380, 237) - camPos;
            indicatorVerts[4] = new Vector2(380, 182) - camPos;
            indicatorVerts[5] = new Vector2(373, 190) - camPos;
            indicatorVerts[6] = new Vector2(261, 190) - camPos;
            indicatorVerts[7] = new Vector2(254, 182) - camPos;

            var barWrapVerts = (sLeaser.sprites[7] as TriangleMesh).vertices;
            barWrapVerts[0] = new Vector2(203, 164) - camPos;
            barWrapVerts[1] = new Vector2(212, 156) - camPos;
            barWrapVerts[2] = new Vector2(421, 156) - camPos;
            barWrapVerts[3] = new Vector2(429, 164) - camPos;
            barWrapVerts[4] = new Vector2(429, 124) - camPos;
            barWrapVerts[5] = new Vector2(421, 132) - camPos;
            barWrapVerts[6] = new Vector2(212, 132) - camPos;
            barWrapVerts[7] = new Vector2(203, 124) - camPos;

            if (game.OutOfPower && UpdateVis)
            {
                for (int i = 0; i < sLeaser.sprites.Length; i++)
                {
                    sLeaser.sprites[i].isVisible &= Random.value > 0.03f;
                }
            }

            UpdateVis = false;
        }

        public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            const float LERP_AMT = 0.4f;

            // Indicator bars
            sLeaser.sprites[0].color = Color.Lerp(new Color(0f, 1f, 0f), palette.fogColor, LERP_AMT);
            sLeaser.sprites[1].color = Color.Lerp(new Color(0f, 1f, 0f), palette.fogColor, LERP_AMT);
            sLeaser.sprites[2].color = Color.Lerp(new Color(1f, 1f, 0f), palette.fogColor, LERP_AMT);
            sLeaser.sprites[3].color = Color.Lerp(new Color(1f, .5f, 0f), palette.fogColor, LERP_AMT);
            sLeaser.sprites[4].color = Color.Lerp(new Color(1f, 0f, 0f), palette.fogColor, LERP_AMT);

            // Power bar
            var brightness = Custom.LerpMap(game.Power, Constants.MaxPower, 0, 1f, 0.75f);
            var color = (float)game.Power / Constants.MaxPower > 0.2 ? new Color(brightness, brightness, brightness) : new Color(brightness, 0, 0);
            sLeaser.sprites[5].color = Color.Lerp(color, palette.fogColor, LERP_AMT);
            if (UpdateVis && Random.value < 0.001f * game.PowerUsage)
            {
                sLeaser.sprites[5].color = Color.Lerp(sLeaser.sprites[5].color, palette.blackColor, Random.Range(0.25f, 0.5f));
            }

            // Outer containers
            sLeaser.sprites[6].color = sLeaser.sprites[7].color = Color.Lerp(Color.white, palette.fogColor, LERP_AMT);
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
