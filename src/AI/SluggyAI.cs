using System;
using static FCAP.Map;

namespace FCAP.AI
{
    internal class SluggyAI : BaseAI
    {
        private static int CamAdditionalWait = 700;
        private static int[] NightDifficulties = [0, 0, 1, 2, 4, 6];

        private bool WaitingForCamDown = false;
        private bool ForceMove = false;

        public SluggyAI(int night) : base(Global.Animatronic.Sluggy, Map.Location.ShowStage, NightDifficulties[night], 2235) { }

        public override void Update()
        {
            if (ForceMove)
            {
                if (GameController.Instance.InCams)
                {
                    counter = 2;
                }
                else if (WaitingForCamDown)
                {
                    WaitingForCamDown = false;
                    counter = CamAdditionalWait;
                }
            }
            else if (counter == 1 && GameController.Instance.InCams)
            {
                counter = 2;
                WaitingForCamDown = true;
                ForceMove = true;
            }
            base.Update();
        }

        public override bool CanJumpscare()
        {
            if (GameController.Instance.OutOfPower)
            {
                //
            }
            else
            {
                return location == Location.LeftDoor || location == Location.RightDoor;
            }
        }

        public override Location TryMove()
        {
            throw new NotImplementedException();
        }
    }
}
