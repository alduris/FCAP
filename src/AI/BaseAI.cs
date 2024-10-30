using FCAP.Graphics;
using static FCAP.Map;
using Random = UnityEngine.Random;

namespace FCAP.AI
{
    public abstract class BaseAI(GameController game, Enums.Animatronic me, Location startLoc, int difficulty, int counter)
    {
        protected int difficulty = difficulty == -1 ? FCAPOptions.GetCustomDifficulty(me) : difficulty;
        protected int maxCounter = counter;
        protected int counter = counter * 2; // we give a little extra delay at the start so the game can load
        public Enums.Animatronic animatronic = me;
        public Location location = startLoc;
        public GameController game = game;
        public DoorAnimatronic doorRepresentation = null;

        protected virtual int PowerDrainOnLeave => 5;

        public virtual void Update()
        {
            if (game.OutOfPower) return;
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
                        var lastLoc = location;
                        location = NextMove();
                        Plugin.Logger.LogInfo(animatronic.ToString() + " moved to " + location.ToString());
                        game.FlickerCams();

                        if (lastLoc != location && location != Location.You && (lastLoc == Location.LeftDoor || lastLoc == Location.RightDoor))
                        {
                            game.Power -= PowerDrainOnLeave;
                        }
                        else if (lastLoc != location && (location == Location.LeftDoor || location == Location.RightDoor))
                        {
                            counter *= 2; // bit of saving grace for the player
                        }
                    }

                    if (location == Location.LeftDoor || location == Location.RightDoor)
                    {
                        doorRepresentation ??= new DoorAnimatronic(game.room, game, animatronic, location == Location.LeftDoor, false);
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
            // Be finnicky with the move chances. At the door they're more likely to move in difficulty <= 3 but less likely to move > 3 which scales with the difficulty
            return Random.Range(0, 20) + (location == Location.LeftDoor || location == Location.RightDoor ? (difficulty > 3 ? difficulty : -difficulty) / 3 : 0) < difficulty;
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
            }
        }
    }
}
