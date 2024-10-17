using Menu;
using UnityEngine;

namespace FCAP.Screens
{
    internal class EndOfNightClock : PositionedMenuObject
    {
        private const int MOVE_TIME_TOTAL = 40;
        private const int BLIP_TIME_TOTAL = 15;

        private const int SPRITES_DIGITS = 3;
        private const int SPRITES_BORDER = 3 * 2;
        private const int TOTAL_SPRITES = SPRITES_DIGITS + SPRITES_BORDER + 1;

        public bool Done => false;

        private int beginWaitTimer = 80;
        private int moveTimeTimer = 0;
        private int blipCircleTimer = 0;

        public FSprite[] sprites;
        public FContainer container;

        private enum Phase
        {
            Waiting,
            Moving,
            Blipping,
            Done
        }
        private Phase phase = Phase.Waiting;

        private int DigitSprite(int n) => n; // 3 digits
        private int BorderSprite(bool right, int part) => SPRITES_DIGITS + part + (right ? 3 : 0); // 3 parts per side
        private int CircleSprite => TOTAL_SPRITES - 1;

        public EndOfNightClock(Menu.Menu menu, MenuObject owner, Vector2 pos) : base(menu, owner, pos)
        {
            container = new FContainer();
            owner.Container.AddChild(container);
            sprites = new FSprite[TOTAL_SPRITES];
        }

        public override void Update()
        {
            base.Update();

            switch (phase)
            {
                case Phase.Waiting:
                    {
                        beginWaitTimer--;
                        if (beginWaitTimer == 0)
                        {
                            phase = Phase.Moving;
                        }
                        break;
                    }
                case Phase.Moving:
                    {
                        break;
                    }
                case Phase.Blipping:
                    {
                        break;
                    }
                case Phase.Done:
                    {
                        break;
                    }
            }
        }

        public override void GrafUpdate(float timeStacker)
        {
            base.GrafUpdate(timeStacker);
        }

        public override void RemoveSprites()
        {
            base.RemoveSprites();
            foreach (var sprite in sprites)
            {
                sprite.RemoveFromContainer();
            }
            container.RemoveFromContainer();
        }
    }
}
