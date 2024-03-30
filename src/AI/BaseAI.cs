using static FCAP.Map;
using Random = UnityEngine.Random;

namespace FCAP.AI
{
    internal abstract class BaseAI(Enums.Animatronic me, Location startLoc, int difficulty, int counter)
    {
        protected int difficulty = difficulty;
        protected int maxCounter = counter;
        protected int counter = counter;
        public Enums.Animatronic animatronic = me;
        public Location location = startLoc;

        public virtual void Update()
        {
            counter--;
            if (counter <= 0)
            {
                counter = maxCounter;
                if (MoveCheck())
                {
                    if (location == Map.Location.You && CanJumpscare())
                    {
                        GameController.Instance.CurrentJumpscare = animatronic;
                    }
                    location = TryMove();
                }
            }
        }

        public bool MoveCheck()
        {
            return Random.Range(0, 20) < difficulty;
        }

        public abstract Location TryMove();

        public virtual bool CanJumpscare()
        {
            if (GameController.Instance.OutOfPower)
            {
                return false;
            }
            else
            {
                return location == Location.LeftDoor || location == Location.RightDoor;
            }
        }
    }
}
