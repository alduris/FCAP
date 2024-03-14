using System;
using HUD;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FCAP.Hooks
{
    internal static class RainMeterHooks
    {
        public static void Apply()
        {
            On.HUD.RainMeter.ctor += SkipHalftimeBlink;
            On.HUD.RainMeter.Update += ForceTimerVisible;
            IL.HUD.RainMeter.Update += BypassNoTimerMMF;
        }

        private static void SkipHalftimeBlink(On.HUD.RainMeter.orig_ctor orig, HUD.RainMeter self, HUD.HUD hud, FContainer fContainer)
        {
            orig(self, hud, fContainer);
            if ((self.hud.owner as Player).SlugCatClass == Constants.Nightguard)
            {
                self.halfTimeShown = true;
            }
        }

        private static void ForceTimerVisible(On.HUD.RainMeter.orig_Update orig, HUD.RainMeter self)
        {
            if ((self.hud.owner as Player).SlugCatClass == Constants.Nightguard)
            {
                self.fade = 1f;
                self.lastFade = 1f;
                self.remainVisibleCounter = int.MaxValue; // something really high so it will stay
            }

            orig(self);
        }

        private static void BypassNoTimerMMF(ILContext il)
        {
            var c = new ILCursor(il);

            c.GotoNext(MoveType.After, x => x.MatchStloc(2));

            // ANDs the variable that checks for RW Remix disable rain timer in safe regions with the inverse of whether or not we're playing as the nightguard.
            // By doing this, the variable will only be true for other scugs, not ours. That way, we ensure that despite the game taking place within Five Pebbles,
            // the rain timer will still show so the player still can figure out if it's close to the end of their shift. It also means it's always visible.

            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<RainMeter, bool>>(self => (self.hud.owner as Player).SlugCatClass != Constants.Nightguard);
            c.Emit(OpCodes.Ldloc_2);
            c.Emit(OpCodes.And);
            c.Emit(OpCodes.Stloc_2);
        }
    }
}
