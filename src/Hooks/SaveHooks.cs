namespace FCAP.Hooks
{
    internal static class SaveHooks
    {
        public static void Apply()
        {
            On.RainWorldGame.ForceSaveNewDenLocation += RainWorldGame_ForceSaveNewDenLocation;
        }

        private static void RainWorldGame_ForceSaveNewDenLocation(On.RainWorldGame.orig_ForceSaveNewDenLocation orig, RainWorldGame game, string roomName, bool saveWorldStates)
        {
            if (game.StoryCharacter == Constants.Nightguard)
            {
                roomName = Constants.RoomName;
            }
            orig(game, roomName, saveWorldStates);
        }
    }
}
