using System;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using BepInEx;
using BepInEx.Logging;
using FCAP.Hooks;
using MonoMod.RuntimeDetour;
using UnityEngine;
using Random = UnityEngine.Random;

#pragma warning disable CS0618
[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace FCAP
{
    [BepInPlugin("alduris.fcap", "Five Cycles at Pebbles", "0.1.0")]
    class Plugin : BaseUnityPlugin
    {
        public static new ManualLogSource Logger;

        // Add hooks
        public void OnEnable()
        {
            Logger = base.Logger;
            Logger.LogDebug("boo");
            On.RainWorld.OnModsInit += Extras.WrapInit(RainWorld_OnModsInit);
        }


        // Load any resources, such as sprites or sounds
        private void RainWorld_OnModsInit(RainWorld rainWorld)
        {
            Constants.Register();

            var bundle = AssetBundle.LoadFromFile(AssetManager.ResolveFilePath("assets/fcap", false));
            rainWorld.Shaders.Add(Constants.CamShaderName, FShader.CreateShader(Constants.CamShaderName, bundle.LoadAsset<Shader>("assets/shaders/camholoimage.shader")));

            Shader.SetGlobalFloat("_fcapThresh", Constants.CamsShaderThresh);

            Logger.LogDebug("hi");
            try
            {

                GameplayHooks.Apply();
                GraphicsHooks.Apply();
                OverseerHooks.Apply();
                ProcessHooks.Apply();
                PlacedObjectHooks.Apply();
                RainMeterHooks.Apply();

                Logger.LogDebug("yay");
            }
            catch (Exception ex)
            {
                Logger.LogError("boowomp");
                Logger.LogError(ex);
            }
        }
    }
}
