using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OverseerHolograms;

namespace FCAP
{
    internal static class Constants
    {
        public static void Register()
        {
            Nightguard = new SlugcatStats.Name("Nightguard", false);
            CamsHologram = new OverseerHologram.Message("FCAPCams", true);
        }

        public static void Unregister()
        {
            Nightguard = null;
            CamsHologram = null;
        }

        public static SlugcatStats.Name Nightguard;

        public static OverseerHologram.Message CamsHologram;
    }
}
