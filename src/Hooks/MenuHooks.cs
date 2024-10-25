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
        private static readonly ConditionalWeakTable<InteractiveMenuScene, NightguardMenuController> menuCWT = new();

        public static void Apply()
        {
            On.Menu.MenuScene.BuildScene += MenuScene_BuildScene;
            On.Menu.InteractiveMenuScene.Update += InteractiveMenuScene_Update;
            On.Menu.MenuScene.UnloadImages += MenuScene_UnloadImages;
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

            Plugin.Logger.LogDebug(self.sceneID?.value);
            if (self.sceneID == Constants.NightguardMenu)
            {
                self.sceneFolder = "scenes/slugcat_nightguard";

                if (self.flatMode)
                {
                    self.AddIllustration(new MenuIllustration(self.menu, self, self.sceneFolder, "nightguard flat", new Vector2(683f, 384f), false, true));
                }
                else
                {
                    var menuController = new NightguardMenuController(self as InteractiveMenuScene);
                    self.AddIllustration(menuController.Outside = new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard 9", new Vector2(-291f, -190f), 4f, MenuDepthIllustration.MenuShader.Basic));
                    self.AddIllustration(menuController.AIBody =  new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard 8.5", new Vector2(-291f, -190f), 3.5f, MenuDepthIllustration.MenuShader.Basic));
                    self.AddIllustration(menuController.AIEyes =  new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard 8", new Vector2(-291f, -190f), 3.45f, MenuDepthIllustration.MenuShader.Basic));
                    self.AddIllustration(menuController.Inside =  new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard 7", new Vector2(-291f, -190f), 3f, MenuDepthIllustration.MenuShader.Basic));
                    self.AddIllustration(menuController.Door =    new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard 6", new Vector2(-291f, -190f), 3.1f, MenuDepthIllustration.MenuShader.Basic));
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard 5", new Vector2(-291f, -190f), 2.6f, MenuDepthIllustration.MenuShader.Basic));
                    self.AddIllustration(menuController.Slugcat = new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard 4", new Vector2(-291f, -190f), 2.1f, MenuDepthIllustration.MenuShader.Basic));
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard 3", new Vector2(-291f, -190f), 1.8f, MenuDepthIllustration.MenuShader.Basic));
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard 2", new Vector2(-291f, -190f), 1.6f, MenuDepthIllustration.MenuShader.Basic));
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "nightguard 1", new Vector2(-291f, -190f), 1.5f, MenuDepthIllustration.MenuShader.Basic));

                    menuCWT.Remove(self as InteractiveMenuScene);
                    menuCWT.Add(self as InteractiveMenuScene, menuController);
                }
            }
        }
    }
}
