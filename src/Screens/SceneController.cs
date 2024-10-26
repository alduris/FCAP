using Menu;

namespace FCAP.Screens
{
    internal abstract class SceneController(InteractiveMenuScene owner)
    {
        public InteractiveMenuScene owner = owner;

        public abstract void Update();

        protected void UpdateImage(MenuIllustration image, string fileName)
        {
            image.fileName = fileName;
            image.LoadFile(image.folderName);
            image.sprite.element = Futile.atlasManager.GetElementWithName(fileName);
        }
    }
}
