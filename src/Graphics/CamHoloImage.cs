using OverseerHolograms;
using UnityEngine;

namespace FCAP.Graphics
{
    internal partial class CamHologram
    {
        // tbh I don't even need this class I'm just using it so I don't have to make extra hooks :leditoroverload:
        internal class CamHoloImage : OverseerImage.HoloImage
        {
            public CamHologram parent;
            private int file = -1;

            public CamHoloImage(CamHologram hologram, int firstSprite, IOwnAHoloImage imageOwner) : base(hologram, firstSprite, imageOwner)
            {
                parent = hologram;

                int numImages = (14 << hologram.game.AIs.Length) / 25;
                for (int i = 0; i <= numImages; i++)
                {
                    LoadFile("FCAP_PROJ" + i);
                }

            }

            public override void Update()
            {
                base.Update();

                // Hacky way to always show correct image
                showRandomFlickerImage = true;
                randomImage = parent.CurrImageIndex;
            }

            public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos, Vector2 partPos, Vector2 headPos, float useFade, float popOut, Color useColor)
            {
                base.DrawSprites(sLeaser, rCam, timeStacker, camPos, partPos, headPos, useFade, popOut, useColor);
                if (file != parent.CurrFileIndex)
                {
                    file = parent.CurrFileIndex;
                }
                sLeaser.sprites[firstSprite].element = Futile.atlasManager.GetElementWithName("FCAP_PROJ" + file);
            }
        }
    }
}
