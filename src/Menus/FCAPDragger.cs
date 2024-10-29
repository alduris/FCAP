using System;
using Menu;
using RWCustom;
using UnityEngine;
using static Menu.Menu;

namespace FCAP.Menus
{
    /// <summary>
    /// Based on Menu.SandboxSettingsInterface.ScoreController.ScoreDragger
    /// </summary>
    internal class FCAPDragger : ButtonTemplate
    {
        private readonly RoundedRect roundedRect;
        private readonly MenuLabel label;
        private readonly MenuLabel animLabel;
        public readonly Enums.Animatronic animatronic;
        public readonly Vector2 offset;

        private float lastGreyFade = 0f;
        private float greyFade = 0f;
        private float lastFlash = 0f;
        private float flash = 0f;

        private int lastY;
        private int yHeldCounter;

        private int forgetClicked = 0;
        public bool held = false;
        private int savScore;
        private float savMouse;

        private float lastAlpha = 1f;
        public float alpha = 1f;
        public bool allowSelect = true;

        public override bool CurrentlySelectableMouse => base.CurrentlySelectableMouse && allowSelect;
        public override bool CurrentlySelectableNonMouse => base.CurrentlySelectableNonMouse && allowSelect;

        public FCAPDragger(Menu.Menu menu, MenuObject owner, Vector2 pos, Vector2 offset, Enums.Animatronic animatronic) : base(menu, owner, pos + offset, new Vector2(24f, 24f))
        {
            this.offset = offset;
            this.animatronic = animatronic;
            savScore = FCAPOptions.GetCustomDifficulty(animatronic);
            roundedRect = new RoundedRect(menu, this, new Vector2(0f, 0f), size, true);
            subObjects.Add(roundedRect);
            label = new MenuLabel(menu, this, "", new Vector2(0f, 2f), new Vector2(24f, 20f), false, null);
            subObjects.Add(label);

            animLabel = new MenuLabel(menu, this, animatronic.ToString()[0].ToString(), new Vector2(0f, 30f), new Vector2(24f, 20f), false, null);
            subObjects.Add(animLabel);

            UpdateLabelText();
        }

        public override Color MyColor(float timeStacker)
        {
            if (buttonBehav.greyedOut)
            {
                return HSLColor.Lerp(MenuColor(MenuColors.VeryDarkGrey), MenuColor(MenuColors.Black), black).rgb;
            }
            float num = Mathf.Lerp(buttonBehav.lastCol, buttonBehav.col, timeStacker);
            num = Mathf.Max(num, Mathf.Lerp(buttonBehav.lastFlash, buttonBehav.flash, timeStacker));
            HSLColor hslcolor = HSLColor.Lerp(MenuColor(MenuColors.DarkGrey), MenuColor(MenuColors.MediumGrey), num);
            hslcolor = HSLColor.Lerp(hslcolor, MenuColor(MenuColors.Black), black);
            return HSLColor.Lerp(hslcolor, MenuColor(MenuColors.VeryDarkGrey), Mathf.Lerp(lastGreyFade, greyFade, timeStacker)).rgb;
        }

        public void UpdateLabelText()
        {
            var diff = FCAPOptions.GetCustomDifficulty(animatronic);
            label.text = diff == 0 ? "-" : diff.ToString();
        }

        public override void Update()
        {
            base.Update();
            buttonBehav.Update();
            lastAlpha = alpha;
            lastFlash = flash;
            lastGreyFade = greyFade;
            flash = Mathf.Max(0f, flash - 0.14285715f);
            greyFade = Custom.LerpAndTick(greyFade, (menu.FreezeMenuFunctions && !held) ? 1f : 0f, 0.05f, 0.025f);
            if (buttonBehav.clicked)
            {
                forgetClicked++;
            }
            else
            {
                forgetClicked = 0;
            }
            roundedRect.fillAlpha = Mathf.Lerp(0.3f, 0.6f, buttonBehav.col);
            roundedRect.addSize = new Vector2(4f, 4f) * (buttonBehav.sizeBump + 0.5f * Mathf.Sin(buttonBehav.extraSizeBump * 3.1415927f)) * (buttonBehav.clicked ? Mathf.InverseLerp(7f, 14f, (float)forgetClicked) : 1f);
            
            int scoreCache = FCAPOptions.GetCustomDifficulty(animatronic);
            bool changed = false;

            // Dragging or doing stuff with controller
            if (held)
            {
                int dragScoreSav = scoreCache;
                if (menu.manager.menuesMouseMode)
                {
                    // Dragging with mouse
                    scoreCache = savScore + (int)((Futile.mousePosition.y - savMouse) / 4f);
                    scoreCache = Custom.IntClamp(scoreCache, 0, 20);
                }
                else
                {
                    // Controller
                    int y = menu.NonMouseInputDisregardingFreeze.y;
                    if (y != lastY || (yHeldCounter > 20 && yHeldCounter % ((yHeldCounter > 60) ? 2 : 4) == 0))
                    {
                        scoreCache += y * ((yHeldCounter > 60) ? 2 : 1);
                    }
                    if (y != 0)
                    {
                        yHeldCounter++;
                    }
                    else
                    {
                        yHeldCounter = 0;
                    }
                    scoreCache = Custom.IntClamp(scoreCache, 0, 20);
                    lastY = y;
                }

                // Check if it changed
                if (scoreCache != dragScoreSav)
                {
                    flash = 1f;
                    menu.PlaySound(SoundID.MENU_Scroll_Tick);
                    buttonBehav.sizeBump = Mathf.Min(2.5f, buttonBehav.sizeBump + 1f);
                    changed = true;
                }
            }
            else
            {
                lastY = 0;
                yHeldCounter = 0;
            }

            // Scrolling with mouse wheel
            if (menu.manager.menuesMouseMode && MouseOver)
            {
                int scrollScoreSav = scoreCache;
                scoreCache -= menu.mouseScrollWheelMovement;
                scoreCache = Custom.IntClamp(scoreCache, 0, 20);
                if (scoreCache != scrollScoreSav)
                {
                    flash = 1f;
                    menu.PlaySound(SoundID.MENU_Scroll_Tick);
                    buttonBehav.sizeBump = Mathf.Min(2.5f, buttonBehav.sizeBump + 1f);
                    savScore = scoreCache;
                    changed = true;
                }
            }

            // Held state tracking
            if (held && !menu.HoldButtonDisregardingFreeze)
            {
                held = false;
            }
            else if (!held && Selected && menu.pressButton)
            {
                savMouse = Futile.mousePosition.y;
                savScore = FCAPOptions.GetCustomDifficulty(animatronic);
                held = true;
            }

            // If it changed, save the new score to the config
            if (changed)
            {
                FCAPOptions.GetConfigurableDifficulty(animatronic).Value = scoreCache;
                FCAPOptions.Instance?.config.Save();
                UpdateLabelText();
            }
        }

        public override void GrafUpdate(float timeStacker)
        {
            base.GrafUpdate(timeStacker);
            float alpha = Mathf.Lerp(lastAlpha, this.alpha, timeStacker);
            float num = 0.5f - 0.5f * Mathf.Sin(Mathf.Lerp(buttonBehav.lastSin, buttonBehav.sin, timeStacker) / 30f * 3.1415927f * 2f);
            num *= buttonBehav.sizeBump;
            Color color = Color.Lerp(MenuRGB(MenuColors.Black), MenuRGB(MenuColors.White), Mathf.Lerp(buttonBehav.lastFlash, buttonBehav.flash, timeStacker));
            for (int i = 0; i < 9; i++)
            {
                roundedRect.sprites[i].color = color;
                roundedRect.sprites[i].alpha = alpha;
            }
            if (held)
            {
                color = Color.Lerp(Color.Lerp(MenuRGB(MenuColors.DarkGrey), MenuRGB(MenuColors.MediumGrey), num), MenuRGB(MenuColors.White), Mathf.Lerp(lastFlash, flash, timeStacker));
            }
            else
            {
                color = Color.Lerp(base.MyColor(timeStacker), MenuRGB(MenuColors.VeryDarkGrey), Mathf.Max(num, Mathf.Lerp(lastGreyFade, greyFade, timeStacker)));
            }
            label.label.color = color;
            label.label.alpha = alpha;
            animLabel.label.color = color;
            animLabel.label.alpha = alpha;
            if (held)
            {
                color = Color.Lerp(MenuRGB(MenuColors.VeryDarkGrey), MenuRGB(MenuColors.DarkGrey), Mathf.Lerp(lastFlash, flash, timeStacker));
            }
            else
            {
                color = MyColor(timeStacker);
            }
            for (int j = 9; j < 17; j++)
            {
                roundedRect.sprites[j].color = color;
                roundedRect.sprites[j].alpha = alpha;
            }
        }

        public override void Clicked()
        {
        }
    }
}
