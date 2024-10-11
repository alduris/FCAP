using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FCAP
{
    public class Sunblock : UpdatableAndDeletable, IDrawable
    {
        private readonly Vector2[] quad;
        public PlacedObject placedObject;

        public int ID => (placedObject.data as SunblockData).ID;
        public bool visible = true;

        public Sunblock(PlacedObject placedObject)
        {
            this.placedObject = placedObject;
            quad = new Vector2[4];
            quad[0] = placedObject.pos;
            quad[1] = placedObject.pos + (placedObject.data as PlacedObject.QuadObjectData).handles[0];
            quad[2] = placedObject.pos + (placedObject.data as PlacedObject.QuadObjectData).handles[1];
            quad[3] = placedObject.pos + (placedObject.data as PlacedObject.QuadObjectData).handles[2];
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            quad[0] = placedObject.pos;
            quad[1] = placedObject.pos + (placedObject.data as PlacedObject.QuadObjectData).handles[0];
            quad[2] = placedObject.pos + (placedObject.data as PlacedObject.QuadObjectData).handles[1];
            quad[3] = placedObject.pos + (placedObject.data as PlacedObject.QuadObjectData).handles[2];
        }

        public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[1];
            sLeaser.sprites[0] = new TriangleMesh("Futile_White", [new(0, 1, 2), new(0, 2, 3)], false)
            {
                color = new Color(1f / 255f, 0f, 0f)
            };
            AddToContainer(sLeaser, rCam, null);
        }

        public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            rCam.ReturnFContainer("Shadows").AddChild(sLeaser.sprites[0]);
        }

        public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
        }

        public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            var mesh = (TriangleMesh)sLeaser.sprites[0];
            for (int i = 0; i < 4; i++)
            {
                mesh.MoveVertice(i, quad[i] - camPos);
            }
            mesh.isVisible = visible;
        }

        public class SunblockData(PlacedObject owner) : PlacedObject.QuadObjectData(owner)
        {
            public Vector2 panelPos;
            public int ID;

            public override void FromString(string s)
            {
                base.FromString(s);
                string[] array = Regex.Split(s, "~");
                panelPos.x = float.Parse(array[6], NumberStyles.Any, CultureInfo.InvariantCulture);
                panelPos.y = float.Parse(array[7], NumberStyles.Any, CultureInfo.InvariantCulture);
                ID = int.Parse(array[8], NumberStyles.Any, CultureInfo.InvariantCulture);
                unrecognizedAttributes = SaveUtils.PopulateUnrecognizedStringAttrs(array, 9);
            }

            public override string ToString()
            {
                var text = string.Join("~", base.ToString(), panelPos.x, panelPos.y, ID);
                return SaveUtils.AppendUnrecognizedStringAttrs(text, "~", unrecognizedAttributes);
            }
        }
    }
}
