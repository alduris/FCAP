using System;
using HUD;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace FCAP.Hooks
{
    internal static class GameHUDHooks
    {
        public static void Apply()
        {
            On.HUD.HUD.Update += HideBadParts;
            On.HUD.RainMeter.ctor += SkipHalftimeBlink;
            On.HUD.RainMeter.Update += ForceTimerVisible;
            IL.HUD.RainMeter.Update += BypassNoTimerMMF;
            _ = new Hook(
                typeof(RainCycle).GetProperty(nameof(RainCycle.RegionHidesTimer)).GetGetMethod(),
                (Func<RainCycle, bool> orig, RainCycle self) => orig(self) && self.world?.game?.StoryCharacter != Constants.Nightguard);
        }

        private static void HideBadParts(On.HUD.HUD.orig_Update orig, HUD.HUD self)
        {
            orig(self);
            if (self.owner is Player p && p.room?.game?.StoryCharacter == Constants.Nightguard)
            {
                self.foodMeter.fade = 0f;
                self.karmaMeter.fade = 0f;
            }
        }

        private static void SkipHalftimeBlink(On.HUD.RainMeter.orig_ctor orig, RainMeter self, HUD.HUD hud, FContainer fContainer)
        {
            orig(self, hud, fContainer);
            if (self.hud.owner is Player p && p.SlugCatClass == Constants.Nightguard)
            {
                self.halfTimeShown = true;
            }
        }

        private static void ForceTimerVisible(On.HUD.RainMeter.orig_Update orig, RainMeter self)
        {
            if (self.hud.owner is Player p && p.SlugCatClass == Constants.Nightguard)
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

            c.GotoNext(MoveType.After, x => x.MatchStloc(1));

            // ANDs the variable that checks for RW Remix disable rain timer in safe regions with the inverse of whether or not we're playing as the nightguard.
            // By doing this, the variable will only be true for other scugs, not ours. That way, we ensure that despite the game taking place within Five Pebbles,
            // the rain timer will still show so the player still can figure out if it's close to the end of their shift. It also means it's always visible.

            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<RainMeter, bool>>(self => self.hud.owner is not Player p || p.SlugCatClass != Constants.Nightguard);
            c.Emit(OpCodes.Ldloc_1);
            c.Emit(OpCodes.And);
            c.Emit(OpCodes.Stloc_1);
        }
    }
}
