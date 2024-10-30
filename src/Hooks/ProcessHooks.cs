using System;
using FCAP.Menus;
using Menu;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RWCustom;
using UnityEngine;

namespace FCAP.Hooks
{
    internal static class ProcessHooks
    {
        public static void Apply()
        {
            On.ProcessManager.PostSwitchMainProcess += SwitchToCustomProcesses;
            IL.ProcessManager.PostSwitchMainProcess += ProcessSwitchProgressionNotReverted;
            On.RainWorldGame.CommunicateWithUpcomingProcess += RainWorldGame_CommunicateWithUpcomingProcess;
        }

        private static void RainWorldGame_CommunicateWithUpcomingProcess(On.RainWorldGame.orig_CommunicateWithUpcomingProcess orig, RainWorldGame self, MainLoopProcess nextProcess)
        {
            orig(self, nextProcess);
            if (nextProcess is WinScreen or GameOverScreen)
            {
                var data = new KarmaLadderScreen.SleepDeathScreenDataPackage(self.cameras[0].hud.textPrompt.foodInStomach, new IntVector2(4, 4), self.GetStorySession.saveState.deathPersistentSaveData.reinforcedKarma, RainWorld.roomNameToIndex["SS_FCAP"], Vector2.zero, self.cameras[0].hud.map.mapData, self.GetStorySession.saveState, self.GetStorySession.characterStats, self.GetStorySession.playerSessionRecords[0], self.GetStorySession.saveState.lastMalnourished, self.GetStorySession.saveState.malnourished);
                if (ModManager.CoopAvailable)
                {
                    for (int i = 1; i < self.GetStorySession.playerSessionRecords.Length; i++)
                    {
                        if (self.GetStorySession.playerSessionRecords[i].kills != null && self.GetStorySession.playerSessionRecords[i].kills.Count > 0)
                        {
                            data.sessionRecord.kills.AddRange(self.GetStorySession.playerSessionRecords[i].kills);
                        }
                    }
                }
                if (nextProcess is WinScreen)
                    (nextProcess as WinScreen).GetDataFromGame(data);
                else
                    (nextProcess as GameOverScreen).GetDataFromGame(data);
            }
        }

        private static void SwitchToCustomProcesses(On.ProcessManager.orig_PostSwitchMainProcess orig, ProcessManager self, ProcessManager.ProcessID ID)
        {
            if (ID == Constants.GameOverScreen)
            {
                self.currentMainLoop = new GameOverScreen(self);
            }
            else if (ID == Constants.NightOverScreen)
            {
                self.currentMainLoop = new EndOfNightScreen(self);
            }
            else if (ID == Constants.WeekOverScreen)
            {
                self.currentMainLoop = new WinScreen(self);
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
