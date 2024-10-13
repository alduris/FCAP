using System.IO;
using UnityEngine;

namespace FCAP.Graphics
{
    public class Jumpscare(Enums.Animatronic source) : CosmeticSprite
    {
        public Enums.Animatronic animatronic = source;

        public string JumpscareElement
        {
            get
            {
                string name = animatronic switch
                {
                    Enums.Animatronic.Survivor => "jumpscareSurvivor",
                    Enums.Animatronic.Monk => "jumpscareMonk",
                    Enums.Animatronic.Hunter => "jumpscareHunter",
                    Enums.Animatronic.Nightcat => "jumpscareNightcat",
                    Enums.Animatronic.Golden => "jumpscareGolden",
                    _ => throw new System.NotImplementedException()
                };
                if (Futile.atlasManager.GetAtlasWithName(name) == null)
                {
                    Texture2D texture2D = new(1, 1, TextureFormat.ARGB32, false);
                    texture2D.LoadImage(File.ReadAllBytes(AssetManager.ResolveFilePath(Path.Combine("jumpscares", name + ".png"))));
                    Futile.atlasManager.LoadAtlasFromTexture(name, texture2D, false);
                }
                return name;
            }
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            lastPos = pos;
            pos = Random.insideUnitCircle * 16f;
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = [new FSprite(JumpscareElement)];
            rCam.ReturnFContainer("HUD2").AddChild(sLeaser.sprites[0]);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            var sprite = sLeaser.sprites[0];
            sprite.MoveToFront();
            sprite.SetPosition(Vector2.Lerp(lastPos, pos, timeStacker));
        }
    }
}
