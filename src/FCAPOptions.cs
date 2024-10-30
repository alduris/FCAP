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

        private static Configurable<bool> N5;
        private static Configurable<bool> N6;
        private static Configurable<bool> M20;

        public static FCAPOptions Instance;

        public static void Flag(int i)
        {
            switch (i)
            {
                case 0: N5.Value = true; break;
                case 1: N6.Value = true; break;
                case 2: M20.Value = true; break;
            }
            Instance?.config.Save();
        }

        public FCAPOptions()
        {
            Instance = this;
            SurvDifficulty = config.Bind(nameof(SurvDifficulty), 10, new ConfigAcceptableRange<int>(0, 20));
            MonkDifficulty = config.Bind(nameof(MonkDifficulty), 10, new ConfigAcceptableRange<int>(0, 20));
            HunterDifficulty = config.Bind(nameof(HunterDifficulty), 10, new ConfigAcceptableRange<int>(0, 20));
            NightDifficulty = config.Bind(nameof(NightDifficulty), 10, new ConfigAcceptableRange<int>(0, 20));

            N5 = config.Bind(nameof(N5), false);
            N6 = config.Bind(nameof(N6), false);
            M20 = config.Bind(nameof(M20), false);
        }
    }
}
