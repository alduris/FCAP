using OverseerHolograms;

namespace FCAP
{
    internal static class Constants
    {
        public const int MaxPower = 24000;
        public const float CamsShaderThresh = 0.9f;
        public const string CamShaderName = "FCAPHoloImage";

        public static void Register()
        {
            Nightguard ??= new SlugcatStats.Name("Nightguard", false);
            CamsHolo ??= new OverseerHologram.Message("FCAPCams", true);
            DoorHolo ??= new OverseerHologram.Message("FCAPDoor", true);
            CamsImageID ??= new OverseerImage.ImageID("FCAPCamImage", true);
            SunblockType ??= new PlacedObject.Type("FCAPSunblock", true);
        }

        public static void Unregister()
        {
            Nightguard = null; // we didn't register this so we don't get to unregister it

            CamsHolo?.Unregister();
            CamsHolo = null;
            DoorHolo?.Unregister();
            DoorHolo = null;
            CamsImageID?.Unregister();
            CamsImageID = null;
            SunblockType?.Unregister();
            SunblockType = null;
        }

        public static SlugcatStats.Name Nightguard;

        public static OverseerHologram.Message CamsHolo;
        public static OverseerHologram.Message DoorHolo;
        public static OverseerImage.ImageID CamsImageID;

        public static PlacedObject.Type SunblockType;
    }
}
