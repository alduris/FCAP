using Menu;
using UnityEngine;

namespace FCAP.Menus
{
    internal class WinSceneController(InteractiveMenuScene owner) : SceneController(owner)
    {
        // just need to flicker the sign
        public MenuDepthIllustration Sign;


        private int cooldown = Random.Range(80, 200);
        private int counter = 0;

        public override void Update()
        {
            if (counter > 0)
            {
                counter--;
                if (counter == 0)
                {
                    cooldown = Random.Range(80, 320);
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

            Sign.alpha = counter > 0 ? Random.Range(0.25f, 0.75f) : Random.Range(0.95f, 1f);
        }
    }
}
