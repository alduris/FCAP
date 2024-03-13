using FCAP.AI;
using UnityEngine;
using static FCAP.Global;

namespace FCAP
{
    internal class GameController : UpdatableAndDeletable
    {
        public static GameController Instance;

        public int Power = Global.MaxPower;

        public PowerStage OOPstage;
        public int OOPTimer = 0;

        public Map.Location CamViewing = Map.Location.ShowStage;
        public Map.Location CamSelected = Map.Location.ShowStage;
        public bool InCams = false;
        public int CamViewTimer = 0;

        public bool LeftDoorLight = false;
        public bool LeftDoorShut = false;
        public bool RightDoorLight = false;
        public bool RightDoorShut = false;

        public Animatronic CurrentJumpscare = Animatronic.None;
        public int JumpscareTimer = 0;

        public bool OutOfPower => Power <= 0;

        public BaseAI[] AIs;
        public GameController(Room room)
        {
            Plugin.Logger.LogDebug("Activated :)");
            this.room = room;
            Instance = this;
            AIs = [];
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            // Power
            Power -= 1 + (LeftDoorLight ? 1 : 0) + (LeftDoorShut ? 1 : 0) + (RightDoorLight ? 1 : 0) + (RightDoorShut ? 1 : 0) + (InCams ? 1 : 0);

            if (OutOfPower)
            {
                InCams = false;
                LeftDoorLight = false;
                LeftDoorShut = false;
                RightDoorLight = false;
                RightDoorShut = false;
            }

            // Update timer thing
            if (InCams)
            {
                CamViewTimer++;
            }
            else
            {
                CamViewTimer = 0;
            }

            // Update jumpscare timer
            if (CurrentJumpscare != Animatronic.None)
            {
                JumpscareTimer++;
                if (JumpscareTimer > 30)
                {
                    // die
                    Application.Quit(); // temporary thing lol
                }
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            Instance = null;
        }
        
        public void ToggleCams()
        {
            InCams = !InCams;
            CamViewTimer = 0;
            if (InCams)
            {
                //
            }
        }
        public void SwitchCamViewing()
        {
            CamViewing = CamSelected;
            CamViewTimer = 0;
        }
        public void SwitchCamSelecting(Map.Direction dir)
        {
            var cons = Map.CameraConnections[CamSelected];
            Map.Location loc = dir switch
            {
                Map.Direction.Up => cons.Up,
                Map.Direction.Down => cons.Down,
                Map.Direction.Left => cons.Left,
                Map.Direction.Right => cons.Right,
                _ => throw new System.NotImplementedException()
            };

            if (loc != Map.Location.NOWHERE)
            {
                CamSelected = loc;
            }
        }

        public void ToggleDoor(Map.Direction side)
        {
            if (side == Map.Direction.Left)
            {
                LeftDoorShut = !LeftDoorShut;
            }
            else if (side == Map.Direction.Right)
            {
                RightDoorShut = !RightDoorShut;
            }
        }
    }
}
