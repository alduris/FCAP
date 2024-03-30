using System;

namespace FCAP.AI
{
    internal class HunterAI : BaseAI
    {
        private static readonly int[] NightDifficulties = [0, 0, 2, 2, 3, 6];

        public HunterAI(int night) : base(Enums.Animatronic.Survivor, Map.Location.ShowStage, NightDifficulties[night], 183) { }

        public override Map.Location TryMove()
        {
            return location;
        }
    }
}
