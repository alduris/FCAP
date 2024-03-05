using System.Runtime.CompilerServices;
using RWCustom;

namespace FCAP
{
    internal static class CWTs
    {
        private class MutableBox<T>(T value)
        {
            public T Value { get; set; } = value;
        }
        private static readonly ConditionalWeakTable<Player, MutableBox<Player.InputPackage>> lastInputCWT = new();

        public static Player.InputPackage LastInput(Player player) => lastInputCWT.GetValue(player, _ => new(RWInput.PlayerInput(0, Custom.rainWorld))).Value;
        public static void UpdateLastInput(Player player, Player.InputPackage input)
        {
            lastInputCWT.GetValue(player, _ => new(RWInput.PlayerInput(0, Custom.rainWorld))).Value = input;
        }
    }
}
