using System;

namespace FCAP
{
    public class Options : OptionInterface
    {
        public static int GetCustomDifficulty(Enums.Animatronic anim) => anim switch
        {
            Enums.Animatronic.Survivor => SurvDifficulty.Value,
            Enums.Animatronic.Monk => MonkDifficulty.Value,
            Enums.Animatronic.Hunter => HunterDifficulty.Value,
            Enums.Animatronic.Nightcat => NightDifficulty.Value,
            Enums.Animatronic.Golden => 20,
            Enums.Animatronic.None => 0,
            _ => throw new NotImplementedException(),
        };

        private static Configurable<int> SurvDifficulty;
        private static Configurable<int> MonkDifficulty;
        private static Configurable<int> HunterDifficulty;
        private static Configurable<int> NightDifficulty;

        public Options()
        {
            SurvDifficulty = new(10, new ConfigAcceptableRange<int>(0, 20));
            MonkDifficulty = new(10, new ConfigAcceptableRange<int>(0, 20));
            HunterDifficulty = new(10, new ConfigAcceptableRange<int>(0, 20));
            NightDifficulty = new(10, new ConfigAcceptableRange<int>(0, 20));
        }
    }
}
