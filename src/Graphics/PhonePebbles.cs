using System.IO;
using FCAP.Hooks;
using HUD;
using RWCustom;

namespace FCAP.Graphics
{
    public class PhonePebbles : UpdatableAndDeletable
    {
        public GameController game;
        public string[] lines = null;
        public DialogBox dialogBox = null;
        public int waitBeforeShowMessages = 40 * 5;

        public PhonePebbles(GameController game, Room room, int cycle)
        {
            this.game = game;
            this.room = room;
            room.AddObject(this);

            try
            {
                // Try to get language
                var lang = LocalizationTranslator.LangShort(Custom.rainWorld.inGameTranslator.currentLanguage);
                var path = AssetManager.ResolveFilePath(Path.Combine("phoneguy", lang, cycle + ".txt"));
                if (File.Exists(path))
                {
                    lines = File.ReadAllLines(path);
                }
                else
                {
                    // Try to get English
                    path = AssetManager.ResolveFilePath(Path.Combine("phoneguy", "eng", cycle + ".txt"));
                    if (File.Exists(path))
                    {
                        lines = File.ReadAllLines(path);
                    }
                }
            }
            catch (System.Exception e)
            {
                Plugin.Logger.LogError(e);
            }
        }

        public override void Update(bool eu)
        {
            base.Update(eu);

            if (lines == null)
            {
                Destroy();
            }
            else if (dialogBox == null && room.game?.cameras[0]?.hud != null)
            {
                dialogBox = room.game.cameras[0].hud.InitDialogBox();
            }
            else if (waitBeforeShowMessages > 0)
            {
                waitBeforeShowMessages--;
                if (waitBeforeShowMessages == 0)
                {
                    foreach (var line in lines)
                    {
                        if (line.Length == 0) continue;
                        dialogBox.AddPebblesMessage(line, 10 + line.Length / 10);
                    }
                }
            }
            else if (dialogBox.messages.Count == 0)
            {
                Destroy();
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            if (waitBeforeShowMessages <= 0 && dialogBox != null && dialogBox.messages.Count > 0)
            {
                dialogBox.Interrupt("*click*", 5);
            }
            if (game.phoneGuy == this)
            {
                if (room.game?.cameras[0] != null)
                {
                    room.game.cameras[0].virtualMicrophone.ShutUpPebbles();
                }
                game.phoneGuy = null;
            }
        }
    }
}
