using Menu;
using UnityEngine;

namespace FCAP.Screens
{
    internal class NightguardMenuController(InteractiveMenuScene owner)
    {
        public InteractiveMenuScene owner = owner;

        public MenuDepthIllustration Slugcat;
        public MenuDepthIllustration Inside;
        public MenuDepthIllustration Door;
        public MenuDepthIllustration Outside;
        public MenuDepthIllustration AIBody;
        public MenuDepthIllustration AIEyes;

        private int innerFlickerCooldown = Random.Range(10, 40);
        private bool innerFlickering = false;
        private bool needToUpdateInnerFlicker = false;

        private int doorFlickerCooldown = Random.Range(10, 40);
        private int doorFlickerCounter = 0;

        private int outerToggleCooldown = Random.Range(10, 40);
        private bool outerToggled = true;
        private bool needToUpdateOuterToggle = false;


        public void Update()
        {
            // Update state variables
            if (innerFlickerCooldown > 0)
            {
                innerFlickering = false;
                innerFlickerCooldown--;
                if (innerFlickerCooldown <= 1)
                {
                    needToUpdateInnerFlicker = true;
                    innerFlickering = true;
                }
            }
            else
            {
                needToUpdateInnerFlicker = true;
                innerFlickering = false;
                innerFlickerCooldown = Random.Range(10, 120);
            }

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

            if (outerToggleCooldown > 0)
            {
                outerToggleCooldown--;
            }
            else
            {
                needToUpdateOuterToggle = true;
                outerToggled = !outerToggled;
                if (outerToggled)
                {
                    outerToggleCooldown = Random.Range(20, 80);
                }
                else
                {
                    outerToggleCooldown = Random.Range(40, 120);
                }
            }

            // Update graphics
            if (needToUpdateInnerFlicker)
            {
                UpdateImage(Slugcat, "nightguard 4" + (innerFlickering ? " alt" : ""));
                UpdateImage(Inside, "nightguard 7" + (innerFlickering ? " alt" : ""));
                needToUpdateInnerFlicker = false;
            }

            if (doorFlickerCounter > 0)
            {
                Door.alpha = Random.Range(0.25f, 0.75f);
            }
            else
            {
                Door.alpha = 1f;
            }

            if (needToUpdateOuterToggle)
            {
                UpdateImage(Outside, "nightguard 9" + (outerToggled? " alt" : ""));
                needToUpdateOuterToggle = false;
            }
        }

        private void UpdateImage(MenuDepthIllustration image, string fileName)
        {
            image.fileName = fileName;
            image.LoadFile(image.folderName);
            image.sprite.element = Futile.atlasManager.GetElementWithName(fileName);
        }
    }
}
