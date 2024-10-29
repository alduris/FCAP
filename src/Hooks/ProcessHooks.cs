using System;
using FCAP.Menus;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FCAP.Hooks
{
    internal static class ProcessHooks
    {
        public static void Apply()
        {
            On.ProcessManager.PostSwitchMainProcess += SwitchToCustomProcesses;
            IL.ProcessManager.PostSwitchMainProcess += ProcessSwitchProgressionNotReverted;
        }

        private static void SwitchToCustomProcesses(On.ProcessManager.orig_PostSwitchMainProcess orig, ProcessManager self, ProcessManager.ProcessID ID)
        {
            if (ID == Constants.GameOverScreen)
            {
                self.currentMainLoop = new GameOverScreen(self);
            }
            else if (ID == Constants.NightOverScreen)
            {
                self.currentMainLoop = null;
                self.currentMainLoop = new EndOfNightScreen(self);
            }
            else if (ID == Constants.WeekOverScreen)
            {
                self.currentMainLoop = null;
#warning Make this
                throw new NotImplementedException();
            }
            orig(self, ID);
        }

        private static void ProcessSwitchProgressionNotReverted(ILContext il)
        {
            // Prevents PostSwitchMainProcess from calling rainWorld.progression.Revert() if it's one of our things
            var c = new ILCursor(il);
            ILLabel breakLoc = null;

            // Find location
            c.GotoNext(x => x.MatchCallOrCallvirt<PlayerProgression>(nameof(PlayerProgression.Revert)));
            c.GotoPrev(MoveType.After, x => x.MatchBrfalse(out breakLoc));

            // Insert our additional logic
            c.Emit(OpCodes.Ldarg_1);
            c.EmitDelegate((ProcessManager.ProcessID ID) => ID != Constants.GameOverScreen && ID != Constants.NightOverScreen); // not week over screen because it acts like credits
            c.Emit(OpCodes.Brfalse, breakLoc);
        }
    }
}
