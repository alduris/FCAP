using Menu;
using OverseerHolograms;

namespace FCAP
{
    internal static class Constants
    {
        public const int MaxPower = 24000;
        public const float CamsShaderThresh = 0.9f;
        public const string CamShaderName = "FCAPHoloImage";
        public const string RoomName = "SS_FCAP";

        public static void Register()
        {
            Nightguard ??= new SlugcatStats.Name("Nightguard", false);

            CamsHolo ??= new OverseerHologram.Message("FCAPCams", true);
            DoorHolo ??= new OverseerHologram.Message("FCAPDoor", true);
            CamsImageID ??= new OverseerImage.ImageID("FCAPCamImage", true);

            SunblockType ??= new PlacedObject.Type("FCAPSunblock", true);
            
            GameOverScreen ??= new ProcessManager.ProcessID("FCAPGameOver", true);
            NightOverScreen ??= new ProcessManager.ProcessID("FCAPNightOver", true);
            WeekOverScreen ??= new ProcessManager.ProcessID("FCAPWeekOver", true);

            JumpscareSound ??= new SoundID("FCAPJumpscare", true);

            NightguardMenu ??= new MenuScene.SceneID("Slugcat_Nightguard", false);
            NightguardDeath ??= new MenuScene.SceneID("Death_Nightguard", false);
        }

        public static void Unregister()
        {
            Nightguard = null; // we didn't register these so we don't get to unregister them
            NightguardMenu = null;

            CamsHolo?.Unregister();
            CamsHolo = null;
            DoorHolo?.Unregister();
            DoorHolo = null;
            CamsImageID?.Unregister();
            CamsImageID = null;
            
            SunblockType?.Unregister();
            SunblockType = null;

            GameOverScreen?.Unregister();
            GameOverScreen = null;
            NightOverScreen?.Unregister();
            NightOverScreen = null;
            WeekOverScreen?.Unregister();
            WeekOverScreen = null;

            JumpscareSound?.Unregister();
            JumpscareSound = null;
        }

        public static SlugcatStats.Name Nightguard;

        public static OverseerHologram.Message CamsHolo;
        public static OverseerHologram.Message DoorHolo;
        public static OverseerImage.ImageID CamsImageID;

        public static PlacedObject.Type SunblockType;

        public static ProcessManager.ProcessID GameOverScreen;   // you died screen
        public static ProcessManager.ProcessID NightOverScreen;  // equivalent of the 6am clock in fnaf games
        public static ProcessManager.ProcessID WeekOverScreen;   // equivalent of the paycheck in fnaf games

        public static SoundID JumpscareSound;

        public static MenuScene.SceneID NightguardMenu;
        public static MenuScene.SceneID NightguardDeath;
    }
}
