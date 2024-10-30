using System.Collections.Generic;
using Menu;
using RWCustom;
using UnityEngine;

namespace FCAP.Menus
{
    internal class WinScreen : Menu.Menu
    {
        private float ContinueAndExitButtonsXPos => manager.rainWorld.options.ScreenSize.x + (1366f - manager.rainWorld.options.ScreenSize.x) / 2f;
        private float LeftHandButtonsPosXAdd => Custom.LerpMap(manager.rainWorld.options.ScreenSize.x, 1024f, 1280f, 222f, 70f);

        private SimpleButton continueButton;
        private SimpleButton exitButton;

        private readonly List<WinLabelThingy> labelGroups = [];
        private int statCountdown = 40;

        public WinScreen(ProcessManager manager) : base(manager, Constants.WeekOverScreen)
        {
            // Page and background
            pages.Add(new Page(this, null, "main", 0));
            scene = new InteractiveMenuScene(this, pages[0], Constants.NightguardWin);
            pages[0].subObjects.Add(scene);

            // Continue button
            continueButton = new SimpleButton(this, pages[0], base.Translate("CONTINUE"), "CONTINUE", new Vector2(ContinueAndExitButtonsXPos - 180f - this.manager.rainWorld.options.SafeScreenOffset.x, Mathf.Max(this.manager.rainWorld.options.SafeScreenOffset.y, 15f)), new Vector2(110f, 30f));
            pages[0].subObjects.Add(continueButton);
            pages[0].lastSelectedObject = continueButton;

            // Exit button
            exitButton = new SimpleButton(this, pages[0], Translate("EXIT"), "EXIT", new Vector2(ContinueAndExitButtonsXPos - 320f - manager.rainWorld.options.SafeScreenOffset.x, Mathf.Max(manager.rainWorld.options.SafeScreenOffset.y, 15f)), new Vector2(110f, 30f));
            pages[0].subObjects.Add(exitButton);
            MutualHorizontalButtonBind(exitButton, continueButton);

            selectedObject = exitButton;
        }

        public override void Update()
        {
            base.Update();
            if (labelGroups.Count > 0)
            {
                if (statCountdown > 0)
                {
                    statCountdown--;
                    return;
                }

                for (int i = 0; i < labelGroups.Count; i++)
                {
                    if (!labelGroups[i].Done)
                    {
                        labelGroups[i].Update();
                        if (labelGroups[i].Done)
                        {
                            statCountdown = 20;
                        }
                        break;
                    }
                }
            }
        }
        public override void Singal(MenuObject sender, string message)
        {
            if (message != null)
            {
                if (message == "CONTINUE")
                {
                    manager.menuSetup.startGameCondition = ProcessManager.MenuSetup.StoryGameInitCondition.Load;
                    manager.RequestMainProcessSwitch(ProcessManager.ProcessID.Game);
                    PlaySound(SoundID.MENU_Continue_From_Sleep_Death_Screen);
                }
                else if (message == "EXIT")
                {
                    manager.RequestMainProcessSwitch(ProcessManager.ProcessID.MainMenu);
                    PlaySound(SoundID.MENU_Switch_Page_Out);
                }
            }
        }

        public void GetDataFromGame(KarmaLadderScreen.SleepDeathScreenDataPackage package)
        {
            float y = 535f;
            labelGroups.Add(new WinLabelTicker(this, 0, 5, "Successful cycles :", y));
            y -= 30f;
            labelGroups.Add(new WinLabelTicker(this, 0, package.saveState.deathPersistentSaveData.deaths, "Deaths :", y));
            y -= 30f;
            labelGroups.Add(new WinLabelTicker(this, 0, package.saveState.deathPersistentSaveData.quits, "Quits :", y));
            y -= 30f;
            int kills = 0;
            foreach (var kill in package.saveState.kills)
            {
                if (kill.Key.critType == CreatureTemplate.Type.Overseer)
                {
                    kills = kill.Value;
                    break;
                }
            }
            labelGroups.Add(new WinLabelTicker(this, 0, kills, "Overseers Wasted :", y));
            y -= 30f;
            labelGroups.Add(new WinLabelPopper(this, "Helped Five Pebbles", y));
            y -= 30f;
            labelGroups.Add(new WinLabelPopper(this, "120 Slugcoin Earned", y));
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

        private class WinLabelThingy
        {
            public Menu.Menu menu;
            public int curr;
            public int end;
            public string name;
            public MenuLabel label;
            public bool isShowing = false;
            public bool Done => isShowing && curr == end;

            public WinLabelThingy(WinScreen menu, int start, int end, string name, float y)
            {
                this.menu = menu;
                curr = start;
                this.end = end;
                this.name = menu.Translate(name);
                label = new MenuLabel(menu, menu.pages[0], this.name + " " + start, new Vector2(menu.LeftHandButtonsPosXAdd, y), new Vector2(200f, 30f), true, null);
                menu.pages[0].subObjects.Add(label);
                label.label.alignment = FLabelAlignment.Left;
                label.label.alpha = 0f;
            }

            public virtual void Update()
            {
                if (!isShowing)
                {
                    isShowing = true;
                }
                else if (!Done)
                {
                    if (curr < end) curr++;
                    else if (curr > end) curr--;
                    PlaySound();
                }
                label.label.alpha = 1f;
            }

            protected virtual void PlaySound()
            {
                menu.PlaySound(SoundID.UI_Multiplayer_Player_Result_Box_Number_Tick);
            }
        }

        private class WinLabelTicker : WinLabelThingy
        {
            public WinLabelTicker(WinScreen menu, int start, int end, string name, float y) : base(menu, start, end + name.Length, name, y)
            {
                label.text = "";
            }

            public override void Update()
            {
                base.Update();
                label.text = curr < name.Length ? name.Substring(curr) : name + " " + curr;
            }
        }

        private class WinLabelPopper : WinLabelThingy
        {
            public WinLabelPopper(WinScreen menu, string name, float y) : base(menu, 0, name.Length, name, y)
            {
                label.text = "<  >";
            }

            public override void Update()
            {
                base.Update();
                label.text = "< " + name.Substring(0, curr) + " >";
            }
        }
    }
}
