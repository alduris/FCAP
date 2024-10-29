using System;
using Menu;
using UnityEngine;

namespace FCAP.Menus
{
    internal class EndOfNightScreen : KarmaLadderScreen
    {
        public override bool LadderInCenter => true;
        private int moveWait = 85;
        private readonly SimpleButton exitButton;

        public EndOfNightScreen(ProcessManager manager) : base(manager, Constants.NightOverScreen)
        {
            AddContinueButton(false);
            exitButton = new SimpleButton(this, pages[0], Translate("EXIT"), "EXIT", new Vector2(ContinueAndExitButtonsXPos - 320f - manager.rainWorld.options.SafeScreenOffset.x, Mathf.Max(manager.rainWorld.options.SafeScreenOffset.y, 15f)), new Vector2(110f, 30f));
            pages[0].subObjects.Add(exitButton);
            mySoundLoopID = SoundID.MENU_Sleep_Screen_LOOP;
            PlaySound(SoundID.MENU_Enter_Sleep_Screen);

            if (myGamePackage.saveState.cycleNumber > 5 || true)
            {
                Enums.Animatronic[] animatronics =
                [
                    Enums.Animatronic.Survivor,
                    Enums.Animatronic.Monk,
                    Enums.Animatronic.Hunter,
                    Enums.Animatronic.Nightcat
                ];
                var startPosition = new Vector2(683f - 12f * (animatronics.Length * 2 - 1), 372f);
                for (int i = 0; i < animatronics.Length; i++)
                {
                    var dragger = new FCAPDragger(this, pages[0], startPosition, Vector2.right * 48f * i, animatronics[i]);
                    pages[0].subObjects.Add(dragger);
                }
            }
        }

        public override void Update()
        {
            base.Update();
            if (karmaLadder != null && moveWait-- == 0)
            {
                karmaLadder.GoToKarma(karma.x + 1, false);
            }
        }

        public override void GetDataFromGame(SleepDeathScreenDataPackage package)
        {
            package.karma.y = 4;
            package.karma.x = Math.Min(4, package.saveState.cycleNumber - 1);
            if (package.saveState.cycleNumber <= 5)
            {
                base.GetDataFromGame(package);
            }
            else
            {
                AddContinueButton(true);
            }
        }

        public override string UpdateInfoText()
        {
            if (selectedObject is SimpleButton)
            {
                SimpleButton simpleButton = selectedObject as SimpleButton;
                if (simpleButton.signalText == "EXIT")
                {
                    return Translate("Exit to title screen");
                }
                if (simpleButton.signalText == "CONTINUE")
                {
                    return Translate("Continue to the next cycle");
                }
            }
            return base.UpdateInfoText();
        }
    }
}
