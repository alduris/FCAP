using System.Collections.Generic;
using Menu;
using RWCustom;
using UnityEngine;

namespace FCAP.Menus
{
    internal class Stars : PositionedMenuObject
    {
        private readonly List<FSprite> sprites = [];

        private float lastAlpha;
        public float alpha;

        public Stars(Menu.Menu menu, MenuObject owner, Vector2 pos, int count) : base(menu, owner, pos)
        {
            for (int i = 0; i < count; i++)
            {
                var sprite = new FSprite("Multiplayer_Star", true)
                {
                    shader = Custom.rainWorld.Shaders[count >= 3 ? "MenuTextGold" : "MenuText"]
                };
                Container.AddChild(sprite);
                sprites.Add(sprite);
            }
        }

        public override void Update()
        {
            base.Update();
            lastAlpha = alpha;
        }

        public override void GrafUpdate(float timeStacker)
        {
            base.GrafUpdate(timeStacker);
            for (int i = 0; i < sprites.Count; i++)
            {
                var sprite = sprites[i];
                sprite.alpha = alpha;
                sprite.SetPosition(Vector2.Lerp(lastPos, pos, timeStacker) + Vector2.right * 60f * (i - sprites.Count / 2f + 0.5f));
            }
        }

        public override void RemoveSprites()
        {
            base.RemoveSprites();
            foreach (var sprite in sprites)
            {
                sprite.RemoveFromContainer();
            }
        }
    }
}
