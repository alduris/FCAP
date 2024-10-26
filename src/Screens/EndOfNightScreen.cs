using System;
using Menu;

namespace FCAP.Screens
{
    internal class EndOfNightScreen : KarmaLadderScreen
    {
        public override bool LadderInCenter => true;

        public EndOfNightScreen(ProcessManager manager) : base(manager, Constants.NightOverScreen)
        {
            AddContinueButton(false);
            mySoundLoopID = SoundID.MENU_Sleep_Screen_LOOP;
            PlaySound(SoundID.MENU_Enter_Sleep_Screen);
        }

        public override void GetDataFromGame(SleepDeathScreenDataPackage package)
        {
            package.karma.y = 4;
            package.karma.x = Math.Min(4, package.saveState.cycleNumber);
            if (package.saveState.cycleNumber <= 5)
            {
                base.GetDataFromGame(package);
            }
            else
            {
                AddContinueButton(true);
            }
        }
    }
}
