using DevInterface;
using RWCustom;
using UnityEngine;

namespace FCAP
{
    public class SunblockRepresentation : QuadObjectRepresentation
    {
        SunblockControlPanel controlPanel;
        int lineSprite;
        Sunblock obj;

        public SunblockRepresentation(DevUI owner, string IDstring, DevUINode parentNode, PlacedObject pObj) : base(owner, IDstring, parentNode, pObj, "FCAPSunblocker")
        {
            controlPanel = new SunblockControlPanel(owner, IDstring, this, new Vector2(100f, 0f));
            subNodes.Add(controlPanel);
            controlPanel.pos = (pObj.data as Sunblock.SunblockData).panelPos;
            
            fSprites.Add(new FSprite("pixel", true));
            lineSprite = fSprites.Count - 1;
            owner.placedObjectsContainer.AddChild(fSprites[lineSprite]);
            fSprites[lineSprite].anchorY = 0f;
        }

        public override void Refresh()
        {
            base.Refresh();
            MoveSprite(lineSprite, absPos);
            fSprites[lineSprite].scaleY = controlPanel.pos.magnitude;
            fSprites[lineSprite].rotation = Custom.AimFromOneVectorToAnother(absPos, controlPanel.absPos);
            (pObj.data as Sunblock.SunblockData).panelPos = controlPanel.pos;
        }

        public override void Update()
        {
            base.Update();
            obj ??= new Sunblock(pObj);
        }

        public class SunblockControlPanel : Panel
        {
            //
            public SunblockControlPanel(DevUI owner, string IDstring, DevUINode parentNode, Vector2 pos) : base(owner, IDstring, parentNode, pos, new Vector2(250f, 25f), "FCAP Sunblock")
            {
                subNodes.Add(new IDControl(owner, "ID", this, new Vector2(5f, 5f)));
            }

            public class IDControl : IntegerControl
            {
                public IDControl(DevUI owner, string IDstring, DevUINode parentNode, Vector2 pos) : base(owner, IDstring, parentNode, pos, "ID")
                {
                }

                public override void Refresh()
                {
                    NumberLabelText = ((parentNode as SunblockRepresentation).pObj.data as Sunblock.SunblockData).ID.ToString();
                    base.Refresh();
                }

                public override void Increment(int change)
                {
                    var data = ((parentNode as SunblockRepresentation).pObj.data as Sunblock.SunblockData);
                    data.ID += change;
                    base.Increment(change);
                    Refresh();
                    parentNode.Refresh();
                }
            }
        }
    }
}
