﻿using System.Linq;
using Menu;
using UnityEngine;

namespace FCAP.Menus
{
    internal class GameOverScreen : Menu.Menu
    {
        private readonly SimpleButton continueButton;
        private readonly SimpleButton exitButton;
        private int greyOutTicker = 0;
        private bool ButtonsGreyedOut => greyOutTicker < 80; // 2 seconds

        public float ContinueAndExitButtonsXPos
        {
            get
            {
                return manager.rainWorld.options.ScreenSize.x + (1366f - manager.rainWorld.options.ScreenSize.x) / 2f;
            }
        }

        public GameOverScreen(ProcessManager manager) : base(manager, Constants.GameOverScreen)
        {
            // Init menu and background
            pages.Add(new Page(this, null, "main", 0));
            scene = new InteractiveMenuScene(this, pages[0], Constants.NightguardDeath);
            pages[0].subObjects.Add(scene);
            selectedObject = null;

            // Init buttons
            continueButton = new SimpleButton(this, pages[0], Translate("CONTINUE"), "CONTINUE", new Vector2(ContinueAndExitButtonsXPos - 180f - manager.rainWorld.options.SafeScreenOffset.x, Mathf.Max(this.manager.rainWorld.options.SafeScreenOffset.y, 15f)), new Vector2(110f, 30f))
            {
                black = 0f
            };
            pages[0].subObjects.Add(continueButton);
            pages[0].lastSelectedObject = continueButton;

            exitButton = new SimpleButton(this, pages[0], Translate("EXIT"), "EXIT", new Vector2(ContinueAndExitButtonsXPos - 320f - manager.rainWorld.options.SafeScreenOffset.x, Mathf.Max(manager.rainWorld.options.SafeScreenOffset.y, 15f)), new Vector2(110f, 30f));
            pages[0].subObjects.Add(exitButton);
            infoLabel.y = manager.rainWorld.options.ScreenSize.y - 30f;

            // Sound loop
            mySoundLoopID = SoundID.MENU_Death_Screen_LOOP;
        }

        public override void Update()
        {
            base.Update();
            continueButton.buttonBehav.greyedOut = ButtonsGreyedOut;
            exitButton.buttonBehav.greyedOut = ButtonsGreyedOut;
            greyOutTicker++;
        }

        public void GetDataFromGame(KarmaLadderScreen.SleepDeathScreenDataPackage package)
        {
            if (package.saveState.cycleNumber > 5)
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
                MutualHorizontalButtonBind(pages[0].subObjects.Last(), exitButton);
            }
            MutualHorizontalButtonBind(exitButton, continueButton);
        }

        public override void Singal(MenuObject sender, string message)
        {
            base.Singal(sender, message);
            switch (message)
            {
                case "CONTINUE":
                    {
                        manager.menuSetup.startGameCondition = ProcessManager.MenuSetup.StoryGameInitCondition.Load;
                        manager.RequestMainProcessSwitch(ProcessManager.ProcessID.Game);
                        PlaySound(SoundID.MENU_Continue_From_Sleep_Death_Screen);
                        break;
                    }
                case "EXIT":
                    {
                        manager.RequestMainProcessSwitch(ProcessManager.ProcessID.MainMenu);
                        PlaySound(SoundID.MENU_Switch_Page_Out);
                        break;
                    }
            }
        }

        public override string UpdateInfoText()
        {
            if (selectedObject is SimpleButton button)
            {
                string text = button.signalText switch
                {
                    "CONTINUE" => "Continue to the next cycle",
                    "EXIT" => "Exit to title screen",
                    _ => null
                };
                if (text != null)
                    return Translate(text);
            }
            return base.UpdateInfoText();
        }

        public override void ShutDownProcess()
        {
            base.ShutDownProcess();
        }
    }
}
