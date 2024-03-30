using System;

namespace FCAP.AI
{
    internal class MonkAI : BaseAI
    {
        private static readonly int[] NightDifficulties = [0, 1, 4, 2, 5, 8, 12];

        public MonkAI(int night) : base(Enums.Animatronic.Survivor, Map.Location.ShowStage, NightDifficulties[night], 213) { }

        public override Map.Location TryMove()
        {
            return location;
        }
    }
}
