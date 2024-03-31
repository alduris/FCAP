using OverseerHolograms;
using RWCustom;

namespace FCAP.Graphics
{
    internal class CamHoloFrame : OverseerHologram.HologramPart
    {
        public CamHoloFrame(CamHologram hologram, int firstSprite) : base(hologram, firstSprite)
        {
            float radius = 100f;
            for (int i = 0; i < 8; i++)
            {
                float num2 = (i + 0.5f) / 8f;
                float num3 = (i + 1.5f) / 8f;
                Add3DLine(Custom.DegToVec(num2 * 360f) * radius, Custom.DegToVec(num3 * 360f) * radius, 5f);
            }
        }
    }
}
