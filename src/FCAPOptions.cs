using System;

namespace FCAP
{
    public class FCAPOptions : OptionInterface
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

        public static Configurable<int> GetConfigurableDifficulty(Enums.Animatronic anim) => anim switch
        {
            Enums.Animatronic.Survivor => SurvDifficulty,
            Enums.Animatronic.Monk => MonkDifficulty,
            Enums.Animatronic.Hunter => HunterDifficulty,
            Enums.Animatronic.Nightcat => NightDifficulty,
            _ => throw new NotImplementedException()
        };

        private static Configurable<int> SurvDifficulty;
        private static Configurable<int> MonkDifficulty;
        private static Configurable<int> HunterDifficulty;
        private static Configurable<int> NightDifficulty;

        public static FCAPOptions? Instance;

        public FCAPOptions()
        {
            Instance = this;
            SurvDifficulty = config.Bind(nameof(SurvDifficulty), 10, new ConfigAcceptableRange<int>(0, 20));
            MonkDifficulty = config.Bind(nameof(MonkDifficulty), 10, new ConfigAcceptableRange<int>(0, 20));
            HunterDifficulty = config.Bind(nameof(HunterDifficulty), 10, new ConfigAcceptableRange<int>(0, 20));
            NightDifficulty = config.Bind(nameof(NightDifficulty), 10, new ConfigAcceptableRange<int>(0, 20));
        }
    }
}
