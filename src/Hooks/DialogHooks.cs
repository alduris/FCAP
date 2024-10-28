using System.Collections.Generic;
using HUD;
using UnityEngine;

namespace FCAP.Hooks
{
    internal static class DialogHooks
    {
        private static readonly SoundID[] voiceSoundIDs = [
            SoundID.SS_AI_Talk_1,
            SoundID.SS_AI_Talk_2,
            SoundID.SS_AI_Talk_3,
            SoundID.SS_AI_Talk_4,
            SoundID.SS_AI_Talk_5
        ];

        public static void Apply()
        {
            On.HUD.DialogBox.InitNextMessage += DialogBox_InitNextMessage;
        }

        private static void DialogBox_InitNextMessage(On.HUD.DialogBox.orig_InitNextMessage orig, HUD.DialogBox self)
        {
            orig(self);
            if (self.CurrentMessage is PebblesMessage)
            {
                self.hud.PlaySound(voiceSoundIDs[Random.Range(0, voiceSoundIDs.Length)]);
            }
        }

        internal class PebblesMessage(string text, float xOrientation, float yPos, int extraLinger) : DialogBox.Message(text, xOrientation, yPos, extraLinger) { }

        public static void AddPebblesMessage(this DialogBox self, string text, int extraLinger)
        {
            self.messages ??= [];
            self.messages.Add(new PebblesMessage(text, self.defaultXOrientation, self.defaultYPos, extraLinger));
            if (self.messages.Count == 1)
            {
                self.InitNextMessage();
            }
        }

        public static void ShutUpPebbles(this VirtualMicrophone self)
        {
            foreach (var sound in self.soundObjects)
            {
                foreach (var id in voiceSoundIDs)
                {
                    if (sound.soundData.soundID == id)
                    {
                        sound.Destroy();
                    }
                }
            }
        }
    }
}
