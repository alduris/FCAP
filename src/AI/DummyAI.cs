namespace FCAP.AI
{
    /// <summary>
    /// Literally does nothing :3
    /// </summary>
    internal class DummyAI : BaseAI
    {
        public DummyAI() : base(GameController.Instance, Enums.Animatronic.None, Map.Location.NOWHERE, 0, int.MaxValue) { }

        public override Map.Location NextMove()
        {
            return Map.Location.NOWHERE;
        }
    }
}
