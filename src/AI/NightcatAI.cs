using UnityEngine;
using static FCAP.Map;

namespace FCAP.AI
{
    /// <summary>
    /// A sort of combination between Freddy and Foxy. Quick to move towards you, then you hope that your door isn't open.
    /// In cameras, likes the dark.
    /// </summary>
    internal class NightcatAI(GameController game, int night) : BaseAI(game, Enums.Animatronic.Nightcat, Location.SecondaryStage, NightDifficulties[night], 368)
    {
        private static readonly int CamAdditionalWait = 700;
        private static readonly int[] NightDifficulties = [0, 0, 1, 2, 4, 6];

        private bool WaitingForCamDown = false;
        private bool ForceMove = false;

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

        public override bool MoveCheck()
        {
            return (base.MoveCheck() && !WaitingForCamDown) || ForceMove;
        }

        public override bool CanJumpscare()
        {
            // temp, add out of power stuff
            return ((location == Location.LeftDoor && !game.LeftDoorShut) || (location == Location.RightDoor && !game.RightDoorShut)) &&
                (!game.InCams || game.InCams && game.CamViewing != location);
        }

        public override Location NextMove()
        {
            return location switch
            {
                Location.SecondaryStage => Random.value < 0.5f ? Location.LeftHall : Location.Storage,

                Location.LeftHall => Location.LeftDoor,
                Location.LeftDoor => Location.SecondaryStage,

                Location.Storage => Location.RightDoor,
                Location.RightDoor => Location.SecondaryStage,
                _ => location
            };
        }
    }
}
