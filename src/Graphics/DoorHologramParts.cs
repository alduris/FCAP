using System.Collections.Generic;
using OverseerHolograms;
using UnityEngine;

namespace FCAP.Graphics
{
    internal partial class DoorHologram
    {
        public class DoorFramePart : HologramPart
        {
            public const float DEPTH = 5f;

            public DoorFramePart(Vector2 tl, Vector2 tr, Vector2 bl, Vector2 br, OverseerHologram hologram, int firstSprite) : base(hologram, firstSprite)
            {
                Vector2 depthVec = new(DEPTH, 0);

                // Corners -> themselves in 3d
                AddLine(tl - depthVec, tl + depthVec);
                AddLine(tr - depthVec, tr + depthVec);
                AddLine(bl - depthVec, bl + depthVec);
                AddLine(br - depthVec, br + depthVec);

                // Corners to corners
                AddLine(tl - depthVec, tr - depthVec);
                AddLine(tl + depthVec, tr + depthVec);

                AddLine(tl - depthVec, bl - depthVec);
                AddLine(tl + depthVec, bl + depthVec);

                AddLine(bl - depthVec, br - depthVec);
                AddLine(bl + depthVec, br + depthVec);

                AddLine(tr - depthVec, br - depthVec);
                AddLine(tr + depthVec, br + depthVec);

                // Other stuff
                hologram.totalSprites += this.totalSprites;
            }
        }

        public class DoorBarrierParts : HologramPart
        {
            public const float GAP = 0.05f;
            public const float DESIRED_HEIGHT = 20f;

            public int[] flicker;
            public float[,] flickerAmt;
            public int numBarriers;

            public DoorBarrierParts(Vector2 tl, Vector2 tr, Vector2 bl, Vector2 br, OverseerHologram hologram, int firstSprite) : base(hologram, firstSprite)
            {
                // Height is calculated from midpoint
                float midpointLength = (Vector2.Distance(tl, tr) + Vector2.Distance(bl, br)) / 2f;
                numBarriers = Mathf.RoundToInt(midpointLength * (1f - GAP) / (DESIRED_HEIGHT + midpointLength * GAP));
                var barrierLength = (midpointLength - midpointLength * GAP * (numBarriers + 1)) / numBarriers;
                var barrierLerpLen = barrierLength / (midpointLength * (1f - GAP));

                // Calculate lerp segment positions
                var rangeTL = Vector2.Lerp(tl, tr, GAP);
                var rangeTR = Vector2.Lerp(tr, tl, GAP);
                var rangeBL = Vector2.Lerp(bl, br, GAP);
                var rangeBR = Vector2.Lerp(br, bl, GAP);

                // Account for top padding
                rangeTL = Vector2.Lerp(rangeTL, rangeBL, GAP);
                rangeTR = Vector2.Lerp(rangeTR, rangeBR, GAP);

                for (int i = 0; i < numBarriers; i++)
                {
                    var top = (float)i / numBarriers;
                    var bottom = top + barrierLerpLen;

                    var segTL = Vector2.Lerp(rangeTL, rangeBL, top);
                    var segTR = Vector2.Lerp(rangeTR, rangeBR, top);
                    var segBL = Vector2.Lerp(rangeTL, rangeBL, bottom);
                    var segBR = Vector2.Lerp(rangeTR, rangeBR, bottom);

                    AddLine(segTL, segTR);
                    AddLine(segTR, segBR);
                    AddLine(segBR, segBL);
                    AddLine(segBL, segTL);
                }

                // Do other stuff
                hologram.totalSprites += this.totalSprites;
                flicker = new int[numBarriers];
                flickerAmt = new float[numBarriers, 2];
            }

            public override void Update()
            {
                base.Update();

                for (int i = 0; i < numBarriers; i++)
                {
                    flickerAmt[i, 1] = flickerAmt[i, 0];
                    if (flicker[i] > 0)
                    {
                        flicker[i]--;
                        flickerAmt[i, 0] = Random.value;
                    }
                    else if (Random.value < 0.005f)
                    {
                        flicker[i] = Random.Range(3, 8);
                        flickerAmt[i, 0] = Random.value;
                    }
                    else
                    {
                        flickerAmt[i, 0] = 0f;
                    }
                }
            }

            public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos, Vector2 partPos, Vector2 headPos, float useFade, float popOut, Color useColor)
            {
                base.DrawSprites(sLeaser, rCam, timeStacker, camPos, partPos, headPos, useFade, popOut, useColor);

                // Flicker barriers
                for (int i = 0; i < numBarriers; i++)
                {
                    sLeaser.sprites[i * 4 + 0].alpha = 1 - Mathf.Lerp(flickerAmt[i, 1], flickerAmt[i, 0], timeStacker);
                }
            }
        }
    }
}
