using System;

namespace FCAP.AI
{
    internal class SurvivorAI : BaseAI
    {
        private static readonly int[] NightDifficulties = [0, 3, 0, 2, 6, 12];

        public SurvivorAI(int night) : base(Enums.Animatronic.Survivor, Map.Location.ShowStage, NightDifficulties[night], 206) { }

        public override Map.Location TryMove()
        {
            return location;
        }
    }
}
