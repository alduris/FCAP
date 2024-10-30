using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FCAP.Hooks;
using HUD;
using Menu.Remix.MixedUI;
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
                    lines = File.ReadAllLines(path).Select(AddLineBreaks).ToArray();
                }
                else
                {
                    // Try to get English
                    path = AssetManager.ResolveFilePath(Path.Combine("phoneguy", "eng", cycle + ".txt"));
                    if (File.Exists(path))
                    {
                        lines = File.ReadAllLines(path).Select(AddLineBreaks).ToArray();
                    }
                }
            }
            catch (System.Exception e)
            {
                Plugin.Logger.LogError(e);
            }

            static string AddLineBreaks(string text)
            {
                const float THRESH = 1366f * 3f / 5f;
                text = text.Trim();
                if (LabelTest.GetWidth(text) > THRESH)
                {
                    var split = new LinkedList<string>(text.Split(' '));
                    var output = new StringBuilder();
                    var currLine = "";
                    while (split.Count > 0)
                    {
                        output.Append(split.First());
                        currLine = string.Concat(currLine, split.First(), " ");
                        split.RemoveFirst();
                        if (LabelTest.GetWidth(currLine) > THRESH)
                        {
                            output.Append("<LINE>");
                            currLine = "";
                        }
                        else
                        {
                            output.Append(' ');
                        }
                    }
                    return output.ToString();
                }
                return text;
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
            if (dialogBox != null && dialogBox.messages.Count > 0)
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
