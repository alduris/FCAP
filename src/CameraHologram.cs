using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoreSlugcats;
using OverseerHolograms;
using UnityEngine;

namespace FCAP
{
    internal class CameraHologram : OverseerHologram
    {
        public Vector2 showPos;
        public CameraHologram(Overseer overseer, Message message, Creature communicateWith, float importance) : base(overseer, message, communicateWith, importance)
        {
            // Get potential positions and pick the farthest one from the player
            List<Vector2> potentialPositions = [];
            for (int i = 0; i < overseer.room.roomSettings.placedObjects.Count; i++)
            {
                if (overseer.room.roomSettings.placedObjects[i].type == PlacedObject.Type.ProjectedImagePosition)
                {
                    potentialPositions.Add(overseer.room.roomSettings.placedObjects[i].pos);
                }
            }

            if (potentialPositions.Count == 0)
            {
                showPos = communicateWith.firstChunk.pos;
                // throw new NotSupportedException("Room must have at least one ProjectedImagePosition!");
            }
            else
            {
                showPos = potentialPositions.Aggregate((x, y) =>
                {
                    var a = Vector2.Distance(x, communicateWith.firstChunk.pos);
                    var b = Vector2.Distance(y, communicateWith.firstChunk.pos);

                    return a > b ? x : y;
                });
            }
        }

        public CameraHologram(AncientBot robo, Message message, Creature communicateWith, float importance) : base(robo, message, communicateWith, importance)
        {
            // You want to use a robot to display your camera? No thanks, we're not artificer here :3
            throw new NotImplementedException();
        }

        public class CameraImage : HologramPart // projections are 200x200
        {
            public CameraHologram owner;
            public PositionedSoundEmitter sound;

            public CameraImage(OverseerHologram hologram, int firstSprite) : base(hologram, firstSprite)
            {
                owner = hologram as CameraHologram;
                totalSprites = 2;
                LoadFile("");
            }

            public void LoadFile(string fileName)
            {
                if (Futile.atlasManager.GetAtlasWithName(fileName) != null)
                {
                    return;
                }
                string str = AssetManager.ResolveFilePath(Path.Combine("Projections", fileName + ".png"));
                Texture2D texture = new(1, 1, TextureFormat.ARGB32, false);
                AssetManager.SafeWWWLoadTexture(ref texture, "file:///" + str, true, true);
                Futile.atlasManager.LoadAtlasFromTexture(fileName, texture, false);
            }

            public override void Update()
            {
                base.Update();

                // Sound
                if (sound == null)
                {
                    sound = new PositionedSoundEmitter(hologram.pos, 0f, 1f);
                    hologram.room.PlaySound(SoundID.Overseer_Image_LOOP, sound, true, 0f, 1f, false);
                    sound.requireActiveUpkeep = true;
                }
                else
                {
                    sound.alive = true;
                    sound.pos = hologram.pos;
                    sound.volume = Mathf.Pow(partFade * hologram.fade, 0.25f) * myAlpha;
                    if (sound.slatedForDeletetion && !sound.soundStillPlaying)
                    {
                        sound = null;
                    }
                }
            }
        }
    }
}
