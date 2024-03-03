using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FCAP.AI
{
    internal abstract class BaseAI(Map.Location startLoc, int difficulty, int counter)
    {
        protected int difficulty = difficulty;
        protected int maxCounter = counter;
        protected int counter = counter;
        public Map.Location location = startLoc;

        public void Update()
        {
            counter--;
            if (counter <= 0)
            {
                counter = maxCounter;
                TryMove();
            }
        }

        public bool MoveCheck()
        {
            return Random.Range(0, 20) < difficulty;
        }

        public void TryMove()
        {
            //
        }
    }
}
