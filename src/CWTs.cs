using System.Runtime.CompilerServices;
using RWCustom;
using static FCAP.Enums;

namespace FCAP
{
    internal static class CWTs
    {
        private class MutableBox<T>(T value)
        {
            public T Value { get; set; } = value;
        }
        private static readonly ConditionalWeakTable<Player, MutableBox<Player.InputPackage>> lastInputCWT = new();
        private static readonly ConditionalWeakTable<Overseer, StrongBox<OverseerTask>> overseerCWT = new();

        public static Player.InputPackage LastInput(Player player) => lastInputCWT.GetValue(player, _ => new(RWInput.PlayerInput(0))).Value;
        public static void UpdateLastInput(Player player, Player.InputPackage input)
        {
            lastInputCWT.GetValue(player, _ => new(RWInput.PlayerInput(0))).Value = input;
        }

        public static void SetOverseerTask(Overseer overseer, OverseerTask behavior) => overseerCWT.Add(overseer, new(behavior));
        public static OverseerTask GetTask(Overseer overseer) => overseerCWT.TryGetValue(overseer, out var box) ? box.Value : OverseerTask.None;
        public static bool HasTask(Overseer overseer) => GetTask(overseer) != OverseerTask.None;
    }
}
