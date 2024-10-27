using Menu;
using UnityEngine;

namespace FCAP.Screens
{
    internal class GameOverSceneController(InteractiveMenuScene owner) : SceneController(owner)
    {
        public MenuDepthIllustration AIEyes;

        private int cooldown = Random.Range(80, 200);
        private int counter = 0;

        public override void Update()
        {
            if (counter > 0)
            {
                counter--;
                if (counter == 0)
                {
                    cooldown = Random.Range(80, 200);
                }
            }
            else
            {
                cooldown--;
                if (cooldown <= 0)
                {
                    counter = Random.Range(2, 10);
                }
            }

            AIEyes.alpha = counter > 0 ? Random.Range(0.25f, 0.75f) : 1f;
        }
    }
}
