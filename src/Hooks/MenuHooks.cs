using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FCAP.Menus;
using Menu;
using MonoMod.RuntimeDetour;
using RWCustom;
using UnityEngine;

namespace FCAP.Hooks
{
    internal static class MenuHooks
    {
        private static readonly ConditionalWeakTable<InteractiveMenuScene, SceneController> menuCWT = new();

        public static void Apply()
        {
            On.Menu.MenuScene.BuildScene += MenuScene_BuildScene;
            On.Menu.InteractiveMenuScene.Update += InteractiveMenuScene_Update;
            On.Menu.MenuScene.UnloadImages += MenuScene_UnloadImages;
            On.Menu.SlugcatSelectMenu.SlugcatPageContinue.ctor += SlugcatPageContinue_ctor;
            On.Menu.SlugcatSelectMenu.SlugcatPageContinue.Update += SlugcatPageContinue_Update;
            On.Menu.Menu.SelectCandidate += Menu_SelectCandidate;
            _ = new Hook(typeof(MenuObject).GetProperty(nameof(MenuObject.Selected)).GetGetMethod(), MenuObject_Selected_get);
        }

        private static bool MenuObject_Selected_get(Func<MenuObject, bool> orig, MenuObject self)
        {
            if (self.menu is SlugcatSelectMenu && self.menu.pages.Any(p => p.subObjects.Any(x => x is FCAPDragger dragger && dragger.held && self != dragger))) return false;
            return orig(self);
        }

        private static MenuObject Menu_SelectCandidate(On.Menu.Menu.orig_SelectCandidate orig, Menu.Menu self, RWCustom.IntVector2 direction)
        {
            if (self.selectedObject is FCAPDragger dragger && dragger.held)
            {
                return self.selectedObject;
            }
            return orig(self, direction);
        }

        private static void SlugcatPageContinue_ctor(On.Menu.SlugcatSelectMenu.SlugcatPageContinue.orig_ctor orig, SlugcatSelectMenu.SlugcatPageContinue self, Menu.Menu menu, MenuObject owner, int pageIndex, SlugcatStats.Name slugcatNumber)
        {
            orig(self, menu, owner, pageIndex, slugcatNumber);
            if (slugcatNumber == Constants.Nightguard)
            {
                self.regionLabel.text = string.Concat(menu.Translate("Five Pebbles Pizzeria"), " - ", menu.Translate("Cycle"), " ", (self.saveGameData.cycle + 1).ToString());
                if (FCAPOptions.Get(0))
                {
                    int count = 1;
                    if (FCAPOptions.Get(1)) count++;
                    if (FCAPOptions.Get(2)) count++;
                    self.subObjects.Add(new Stars(menu, self, self.regionLabel.pos + Vector2.up * 40f, count));
                }

                // Difficulty selection
                if (self.saveGameData.cycle > 5)
                {
                    Enums.Animatronic[] animatronics =
                    [
                        Enums.Animatronic.Survivor,
                        Enums.Animatronic.Monk,
                        Enums.Animatronic.Hunter,
                        Enums.Animatronic.Nightcat
                    ];
                    var startPosition = new Vector2(self.regionLabel.pos.x, self.KarmaSymbolPos.y - 18f);
                    for (int i = 0; i < animatronics.Length; i++)
                    {
                        var dragger = new FCAPDragger(self.menu, self.menu.pages[0], startPosition, Vector2.right * 48f * (i - (animatronics.Length / 2f) + 0.25f), animatronics[i]);
                        self.subObjects.Add(dragger);
                    }
                }
            }
        }

        private static void SlugcatPageContinue_Update(On.Menu.SlugcatSelectMenu.SlugcatPageContinue.orig_Update orig, SlugcatSelectMenu.SlugcatPageContinue self)
        {
            orig(self);
            if (self.slugcatNumber == Constants.Nightguard)
            {
                self.hud.karmaMeter.fade = 0f;
                self.hud.foodMeter.fade = 0f;
                if (self.saveGameData.cycle <= 5)
                {
                    self.regionLabel.pos = new Vector2(self.regionLabel.pos.x, self.KarmaSymbolPos.y);
                }
                else
                {
                    foreach (var subObj in self.subObjects)
                    {
                        if (subObj is FCAPDragger dragger)
                        {
                            dragger.pos = new Vector2(self.MidXpos + self.NextScroll(1f) * self.ScrollMagnitude, self.KarmaSymbolPos.y - 18f) + dragger.offset;
                            dragger.alpha = 1 - Mathf.Abs(self.NextScroll(1f));
                            dragger.allowSelect = self.Scroll(1f) < 0.5f;
                        }
                        else if (subObj is Stars stars)
                        {
                            stars.pos = new Vector2(self.MidXpos + self.NextScroll(1f) * self.ScrollMagnitude, self.regionLabel.pos.y + 50f);
                            stars.alpha = 1 - Mathf.Abs(self.NextScroll(1f));
                        }
                    }
                }
            }
        }

        private static void MenuScene_UnloadImages(On.Menu.MenuScene.orig_UnloadImages orig, MenuScene self)
        {
            if (self is InteractiveMenuScene ims && menuCWT.TryGetValue(ims, out _))
            {
                menuCWT.Remove(ims);
            }
            orig(self);
        }

        private static void InteractiveMenuScene_Update(On.Menu.InteractiveMenuScene.orig_Update orig, InteractiveMenuScene self)
        {
            orig(self);
            if (menuCWT.TryGetValue(self, out var controller))
            {
                controller.Update();
            }
        }

        private static void MenuScene_BuildScene(On.Menu.MenuScene.orig_BuildScene orig, MenuScene self)
        {
            orig(self);

            if (self.sceneID == Constants.NightguardMenu)
            {
                bool useExMenu = self.menu is SlugcatSelectMenu ssm
                    && ssm.saveGameData[Constants.Nightguard]?.cycle == 5;

                self.sceneFolder = useExMenu ? "scenes/therapy_nightguard" : "scenes/slugcat_nightguard";

                if (useExMenu)
                {
                    // Easter egg menu that only shows after you've beaten cycle 5 but haven't beaten cycle 6
                    if (self.flatMode)
                    {
                        self.AddIllustration(new MenuIllustration(self.menu, self, self.sceneFolder, "ex nightguard flat", new Vector2(683, 384), false, true));
                    }
                    else
                    {
                        var offset = new Vector2(-17f, -16f + 160f);
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "ex nightguard 5", offset, 2.9f, MenuDepthIllustration.MenuShader.Basic));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "ex nightguard 4", offset, 2.5f, MenuDepthIllustration.MenuShader.Basic));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "ex nightguard 3", offset, 2.3f, MenuDepthIllustration.MenuShader.Basic));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "ex nightguard 2", offset, 2.1f, MenuDepthIllustration.MenuShader.Basic));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "ex nightguard 1", offset, 2.2f, MenuDepthIllustration.MenuShader.Basic));
                        (self as InteractiveMenuScene).idleDepths.Add(2.9f);
                        (self as InteractiveMenuScene).idleDepths.Add(2.2f);
                        (self as InteractiveMenuScene).idleDepths.Add(2.4f);
                        (self as InteractiveMenuScene).idleDepths.Add(1.9f);
                    }
                }
                else
                {
                    // Normal menu
                    if (self.flatMode)
                    {
                        self.AddIllustration(new MenuIllustration(self.menu, self, self.sceneFolder, "nightguard flat", new Vector2(683f, 384f), false, true));
                    }
                    else
                    {
                        var offset = new Vector2(72f, 24f + 80f); // center and then raise 80 pixels
                        var menuController = new NightguardMenuController(self as InteractiveMenuScene);
                        self.AddIllustration(menuController.Outside = new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard 9", offset, 4f, MenuDepthIllustration.MenuShader.Basic));
                        self.AddIllustration(menuController.AIBody =  new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard 8.5", offset, 3.5f, MenuDepthIllustration.MenuShader.Basic));
                        self.AddIllustration(menuController.AIEyes =  new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard 8", offset, 3.45f, MenuDepthIllustration.MenuShader.Basic));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard 7 alt", offset, 3f, MenuDepthIllustration.MenuShader.Basic));
                        self.AddIllustration(menuController.Inside =  new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard 7", offset, 3f, MenuDepthIllustration.MenuShader.Basic));
                        self.AddIllustration(menuController.Door =    new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard 6", offset, 3.1f, MenuDepthIllustration.MenuShader.Basic));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard 5", offset, 2.6f, MenuDepthIllustration.MenuShader.Basic));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard 4 alt", offset, 2.1f, MenuDepthIllustration.MenuShader.Basic));
                        self.AddIllustration(menuController.Slugcat = new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard 4", offset, 2.1f, MenuDepthIllustration.MenuShader.Basic));
                        self.AddIllustration(menuController.Hologram = new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard hologram 6", offset, 1.95f, MenuDepthIllustration.MenuShader.Basic));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard 3", offset, 1.8f, MenuDepthIllustration.MenuShader.Basic));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard 2", offset, 1.6f, MenuDepthIllustration.MenuShader.Basic));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard 1", offset, 1.5f, MenuDepthIllustration.MenuShader.Basic));
                        (self as InteractiveMenuScene).idleDepths.Add(2.8f);
                        (self as InteractiveMenuScene).idleDepths.Add(2.1f);
                        (self as InteractiveMenuScene).idleDepths.Add(1.9f);

                        menuCWT.Remove(self as InteractiveMenuScene);
                        menuCWT.Add(self as InteractiveMenuScene, menuController);
                    }
                }
            }
            else if (self.sceneID == Constants.NightguardDeath)
            {
                self.sceneFolder = "scenes/death_nightguard";

                if (self.flatMode)
                {
                    string eyeArt = GameController.LastJumpscare == Enums.Animatronic.Nightcat ? "nightguard death flat" : "nightguard death flat alt";
                    self.AddIllustration(new MenuIllustration(self.menu, self, self.sceneFolder, eyeArt, new Vector2(683f, 384f), false, true));
                }
                else
                {
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard death 6", Vector2.zero, 3.5f, MenuDepthIllustration.MenuShader.Basic));
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard death 5", Vector2.zero, 3.0f, MenuDepthIllustration.MenuShader.Basic));
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard death 3 alt", Vector2.zero, 3.1f, MenuDepthIllustration.MenuShader.Basic));
                    if (GameController.LastJumpscare == Enums.Animatronic.Nightcat)
                    {
                        var controller = new GameOverSceneController(self as InteractiveMenuScene);
                        self.AddIllustration(controller.AIEyes = new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard death 3", Vector2.zero, 3.1f, MenuDepthIllustration.MenuShader.Basic));
                        menuCWT.Remove(self as InteractiveMenuScene);
                        menuCWT.Add(self as InteractiveMenuScene, controller);
                    }
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard death 4", Vector2.zero, 2.6f, MenuDepthIllustration.MenuShader.Basic));
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard death 2", Vector2.zero, 2.1f, MenuDepthIllustration.MenuShader.Basic));
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard death 1", Vector2.zero, 2.05f, MenuDepthIllustration.MenuShader.Basic));
                    (self as InteractiveMenuScene).idleDepths.Add(2.3f);
                    (self as InteractiveMenuScene).idleDepths.Add(2.0f);
                    (self as InteractiveMenuScene).idleDepths.Add(3.2f);
                    (self as InteractiveMenuScene).idleDepths.Add(1.6f);
                }
            }
            else if (self.sceneID == Constants.NightguardWin)
            {
                self.sceneFolder = "scenes/end_nightguard";

                if (self.flatMode)
                {
                    self.AddIllustration(new MenuIllustration(self.menu, self, self.sceneFolder, "nightguard end", new Vector2(683f, 384f), false, true));
                }
                else
                {
                    var controller = new WinSceneController(self as InteractiveMenuScene);
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard end 13", Vector2.zero, 7.0f, MenuDepthIllustration.MenuShader.Basic));
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard end 12", Vector2.zero, 6.5f, MenuDepthIllustration.MenuShader.Basic));
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard end 11", Vector2.zero, 4.5f, MenuDepthIllustration.MenuShader.Basic));
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard end 10", Vector2.zero, 2.5f, MenuDepthIllustration.MenuShader.Basic));
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard end 9", Vector2.zero, 3.8f, MenuDepthIllustration.MenuShader.Basic));
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard end 8", Vector2.zero, 3.7f, MenuDepthIllustration.MenuShader.Basic));
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard end 7", Vector2.zero, 3.5f, MenuDepthIllustration.MenuShader.Basic));
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard end 6", Vector2.zero, 3.4f, MenuDepthIllustration.MenuShader.Basic));
                    self.AddIllustration(controller.Sign = new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard end 6 alt", Vector2.zero, 3.4f, MenuDepthIllustration.MenuShader.Basic));
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard end 5", Vector2.zero, 3f, MenuDepthIllustration.MenuShader.Basic));
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard end 4", Vector2.zero, 2.4f, MenuDepthIllustration.MenuShader.Basic));
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard end 3", Vector2.zero, 2.2f, MenuDepthIllustration.MenuShader.Basic));
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard end 2", Vector2.zero, 2.3f, MenuDepthIllustration.MenuShader.Basic));
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard end 1", Vector2.zero, 2.1f, MenuDepthIllustration.MenuShader.Basic));
                    (self as InteractiveMenuScene).idleDepths.Add(2.6f);
                    (self as InteractiveMenuScene).idleDepths.Add(2.0f);
                    (self as InteractiveMenuScene).idleDepths.Add(3.2f);
                    (self as InteractiveMenuScene).idleDepths.Add(1.6f);
                    menuCWT.Remove(self as InteractiveMenuScene);
                    menuCWT.Add(self as InteractiveMenuScene, controller);
                }

                // Also play song
                self.menu.manager.musicPlayer.MenuRequestsSong(Constants.EndingSongs[UnityEngine.Random.Range(0, Constants.EndingSongs.Length)], float.MaxValue, 0f);
            }
        }
    }
}
