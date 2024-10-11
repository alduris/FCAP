using FCAP.Graphics;
using UnityEngine;
using static FCAP.Map;
using Random = UnityEngine.Random;

namespace FCAP.AI
{
    internal abstract class BaseAI(GameController game, Enums.Animatronic me, Location startLoc, int difficulty, int counter)
    {
        protected int difficulty = difficulty;
        protected int maxCounter = counter;
        protected int counter = counter;
        public Enums.Animatronic animatronic = me;
        public Location location = startLoc;
        public GameController game = game;
        public DoorAnimatronic doorRepresentation = null;

        public virtual void Update()
        {
            counter--;
            if (counter <= 0)
            {
                counter = maxCounter;
                if (MoveCheck())
                {
                    if (CanJumpscare())
                    {
                        game.CurrentJumpscare = animatronic;
                    }
                    else
                    {
                        location = NextMove();
                        Plugin.Logger.LogInfo(animatronic.ToString() + " moved to " + location.ToString());
                        game.FlickerCams();
                    }

                    if (location == Location.LeftDoor || location == Location.RightDoor)
                    {
                        doorRepresentation ??= new DoorAnimatronic(game.room, game, animatronic, location == Location.LeftDoor);
                        game.room.AddObject(doorRepresentation);
                    }
                    else
                    {
                        doorRepresentation?.Destroy();
                        doorRepresentation = null;
                    }
                }
            }
        }

        public virtual bool MoveCheck()
        {
            return Random.Range(0, 20) < difficulty;
        }

        public abstract Location NextMove();

        public virtual bool CanJumpscare()
        {
            if (GameController.Instance.OutOfPower)
            {
                return false;
            }
            else
            {
                return location == Location.You;
                // return (location == Location.LeftDoor && !game.LeftDoorShut) || (location == Location.RightDoor && !game.RightDoorShut) || location == Location.You;
            }
        }
    }
}
