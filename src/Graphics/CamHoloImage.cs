using OverseerHolograms;

namespace FCAP.Graphics
{
    internal partial class CamHologram
    {
        // tbh I don't even need this class I'm just using it so I don't have to make extra hooks :leditoroverload:
        internal class CamHoloImage : OverseerImage.HoloImage
        {
            public CamHologram cams;

            public CamHoloImage(CamHologram hologram, int firstSprite, IOwnAHoloImage imageOwner) : base(hologram, firstSprite, imageOwner)
            {
                cams = hologram;

                int numImages = (13 << hologram.game.AIs.Length) / 25;
                for (int i = 0; i <= numImages; i++)
                {
                    LoadFile("FCAP_PROJ" + i);
                }
            }

            public override void Update()
            {
                base.Update();
                randomImage = cams.CurrImageIndex; // no random flashy, that would actually be a bad thing here
            }
        }
    }
}
