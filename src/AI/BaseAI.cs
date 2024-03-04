using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FCAP.AI
{
    internal abstract class BaseAI(Global.Animatronic me, Map.Location startLoc, int difficulty, int counter)
    {
        protected int difficulty = difficulty;
        protected int maxCounter = counter;
        protected int counter = counter;
        public Global.Animatronic animatronic = me;
        public Map.Location location = startLoc;

        public void Update()
        {
            counter--;
            if (counter <= 0)
            {
                counter = maxCounter;
                if (MoveCheck())
                {
                    if (location == Map.Location.You && CanJumpscare())
                    {
                        Global.CurrentJumpscare = animatronic;
                    }
                    location = TryMove();
                }
            }
        }

        public bool MoveCheck()
        {
            return Random.Range(0, 20) < difficulty;
        }

        public abstract Map.Location TryMove();

        public abstract bool CanJumpscare();
    }
}
