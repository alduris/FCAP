using System;
using System.Runtime.CompilerServices;
using FCAP.Screens;
using Menu;
using Mono.Cecil.Cil;
using MonoMod.Cil;
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
            On.Menu.SlugcatSelectMenu.SlugcatPageContinue.Update += SlugcatPageContinue_Update;
            On.Menu.SlugcatSelectMenu.SlugcatPageContinue.ctor += SlugcatPageContinue_ctor;
        }

        private static void SlugcatPageContinue_ctor(On.Menu.SlugcatSelectMenu.SlugcatPageContinue.orig_ctor orig, SlugcatSelectMenu.SlugcatPageContinue self, Menu.Menu menu, MenuObject owner, int pageIndex, SlugcatStats.Name slugcatNumber)
        {
            orig(self, menu, owner, pageIndex, slugcatNumber);
            if (slugcatNumber == Constants.Nightguard)
            {
                self.regionLabel.text = string.Concat(menu.Translate("Five Pebbles Pizzeria"), " - ", menu.Translate("Cycle"), " ", (self.saveGameData.cycle + 1).ToString());
            }
        }

        private static void SlugcatPageContinue_Update(On.Menu.SlugcatSelectMenu.SlugcatPageContinue.orig_Update orig, SlugcatSelectMenu.SlugcatPageContinue self)
        {
            orig(self);
            if (self.slugcatNumber == Constants.Nightguard)
            {
                self.hud.karmaMeter.fade = 0f;
                self.hud.foodMeter.fade = 0f;
                self.regionLabel.pos = new Vector2(self.regionLabel.pos.x, self.KarmaSymbolPos.y);
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
                self.sceneFolder = "scenes/slugcat_nightguard";

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

                    menuCWT.Remove(self as InteractiveMenuScene);
                    menuCWT.Add(self as InteractiveMenuScene, menuController);
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
        }
    }
}
