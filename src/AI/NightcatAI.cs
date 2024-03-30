using System;
using static FCAP.Map;

namespace FCAP.AI
{
    internal class NightcatAI : BaseAI
    {
        private static readonly int CamAdditionalWait = 700;
        private static readonly int[] NightDifficulties = [0, 0, 1, 2, 4, 6];

        private bool WaitingForCamDown = false;
        private bool ForceMove = false;

        public NightcatAI(int night) : base(Enums.Animatronic.Nightcat, Location.SecondaryStage, NightDifficulties[night], 368) { }

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
            // temp, add out of power stuff
            return base.CanJumpscare();
        }

        public override Location TryMove()
        {
            return location;
        }
    }
}
