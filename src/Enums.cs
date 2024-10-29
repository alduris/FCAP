using System;

namespace FCAP
{
    public static class Enums
    {

        public enum Animatronic
        {
            Survivor,
            Monk,
            Hunter,
            Nightcat,
            Golden,
            None
        }

        public static SlugcatStats.Name GetFakeSlug(Animatronic animatronic)
        {
            return animatronic switch
            {
                Animatronic.Survivor => SlugcatStats.Name.White,
                Animatronic.Monk => SlugcatStats.Name.Yellow,
                Animatronic.Hunter => SlugcatStats.Name.Red,
                Animatronic.Nightcat => SlugcatStats.Name.Night,
                _ => throw new NotImplementedException()
            };
        }

        public enum OverseerTask
        {
            LeftDoor,
            RightDoor,
            Cameras,
            None
        }
    }
}
