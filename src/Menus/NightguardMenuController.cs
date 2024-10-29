using Menu;
using UnityEngine;

namespace FCAP.Menus
{
    internal class NightguardMenuController(InteractiveMenuScene owner) : SceneController(owner)
    {
        public MenuDepthIllustration Slugcat;
        public MenuDepthIllustration Inside;
        public MenuDepthIllustration Door;
        public MenuDepthIllustration Outside;
        public MenuDepthIllustration AIBody;
        public MenuDepthIllustration AIEyes;
        public MenuDepthIllustration Hologram;

        private int innerFlickerCooldown = Random.Range(10, 40);
        private int innerFlickerCounter = 0;

        private int doorFlickerCooldown = Random.Range(10, 40);
        private int doorFlickerCounter = 0;

        private int outerToggleCooldown = Random.Range(10, 40);
        private bool outerToggled = true;
        private bool needToUpdateOuterToggle = false;

        private int eyeFlickerCooldown = Random.Range(80, 200);
        private int eyeFlickerCounter = 0;

        private int hologramFlickerCooldown = Random.Range(40, 160);
        private int hologramFlickerCounter = 0;
        private int hologramGraphic = 6;
        private bool needToUpdateHologramGraphic = false;


        public override void Update()
        {
            // Inside room light flickering
            if (innerFlickerCounter > 0)
            {
                innerFlickerCounter--;
                if (innerFlickerCounter == 0)
                {
                    if (Random.value < 0.1f)
                        innerFlickerCooldown = Random.Range(10, 20);
                    else
                        innerFlickerCooldown = Random.Range(120, 300);
                }
            }
            else
            {
                innerFlickerCooldown--;
                if (innerFlickerCooldown <= 0)
                {
                    innerFlickerCounter = Random.Range(4, 16);
                }
            }

            Slugcat.alpha = innerFlickerCounter > 0 ? Random.Range(0.25f, 0.75f) : 1f;
            Inside.alpha = innerFlickerCounter > 0 ? Random.Range(0.25f, 0.75f) : 1f;

            // Door flicker
            if (doorFlickerCounter > 0)
            {
                doorFlickerCounter--;
                if (doorFlickerCounter == 0)
                {
                    doorFlickerCooldown = Random.Range(15, 80);
                }
            }
            else
            {
                doorFlickerCooldown--;
                if (doorFlickerCooldown <= 0)
                {
                    doorFlickerCounter = Random.Range(2, 10);
                }
            }

            Door.alpha = doorFlickerCounter > 0 ? Random.Range(0.25f, 0.75f) : 1f;

            // Outside room light toggle
            if (outerToggleCooldown > 0)
            {
                outerToggleCooldown--;
            }
            else
            {
                needToUpdateOuterToggle = true;
                outerToggled = !outerToggled;

                if (outerToggled)
                    outerToggleCooldown = Random.Range(20, 80);
                else
                    outerToggleCooldown = Random.Range(40, 120);
            }

            if (needToUpdateOuterToggle)
            {
                UpdateImage(Outside, "nightguard 9" + (outerToggled? " alt" : ""));
                needToUpdateOuterToggle = false;
            }

            // Nightcat eye flickering
            if (eyeFlickerCounter > 0)
            {
                eyeFlickerCounter--;
                if (eyeFlickerCounter == 0)
                {
                    eyeFlickerCooldown = Random.Range(80, 200);
                }
            }
            else
            {
                eyeFlickerCooldown--;
                if (eyeFlickerCooldown <= 0)
                {
                    eyeFlickerCounter = Random.Range(2, 10);
                }
            }

            AIEyes.alpha = eyeFlickerCounter > 0 ? Random.Range(0.25f, 0.75f) : 1f;

            // Hologram flickering and graphics change
            if (hologramFlickerCounter > 0)
            {
                hologramFlickerCounter--;
                if (hologramFlickerCounter == 0)
                {
                    hologramFlickerCooldown = Random.Range(40, 160);
                    if (Random.value < 0.75f)
                    {
                        int oldGraf = hologramGraphic;
                        do
                        {
                            hologramGraphic = Random.Range(1, 7);
                        } while (hologramGraphic == oldGraf);
                        needToUpdateHologramGraphic = true;
                    }
                }
            }
            else
            {
                hologramFlickerCooldown--;
                if (hologramFlickerCooldown <= 0)
                {
                    hologramFlickerCounter = Random.Range(8, 25);
                }
            }

            Hologram.alpha = hologramFlickerCounter > 0 ? Random.Range(0.25f, 0.75f) : 1f;
            if (needToUpdateHologramGraphic)
            {
                UpdateImage(Hologram, "nightguard hologram " + hologramGraphic);
            }
        }
    }
}
