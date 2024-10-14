using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCAP
{
    public static class Map
    {
        /*
         * MAP LAYOUT THINGY (sort of, it's a bit inaccurate in scale):
         * ────────────────────────────────────────────────────────────
         *   ┌─────┐      ┌──┐        │ GUIDE TO SYMBOLS
         *   │     │ ┌──┐ │4 │        │ ───────────────────────
         * ┌─┤    3└─┘  └─┴─ ┴─┬─┐    │ CAM 1A -> Show stage
         * │2A        1C       2C│    │ CAM 1B -> Dining area
         * └─┤              1A ├─┘    │ CAM 1C -> Pirate cove
         * ┌─┤                 ├─┐    │ CAM 1D -> Play area
         * │2B       1D    1B  2D│    │ CAM 2A -> Party room A
         * └─┤          ┌┬ ──┬─┴─┘    │ CAM 2B -> Party room B
         *   │          ││5  │        │ CAM 2C -> Party room C
         *   │          │└───┘        │ CAM 2D -> Party room D
         * ┌─┼ ┬┬──── ┬─┘             │ CAM 3  -> Main entrance
         * │   ││    7│               │ CAM 4  -> Backstage
         * ├─┤ │└──┬ ┬┘               │ CAM 5  -> Kitchen
         * │   │   │ │                │ CAM 6  -> Restroom hall/left hall
         * └─┤ ├───┤ │                │ CAM 7  -> Storage
         *   │6  ○  8│                │ CAM 8  -> Back hall/right hall
         *   │ ├───┴─┘                │ ○      -> You
         *   └─┘
         */

        public enum Location
        {
            NOWHERE,
            ShowStage,      // cam 1a
            DiningArea,     // cam 1b
            SecondaryStage, // cam 1c
            PlayArea,       // cam 1d
            PartyRoomA,     // cam 2a
            PartyRoomB,     // cam 2b
            PartyRoomC,     // cam 2c
            PartyRoomD,     // cam 2d
            MainEntrance,   // cam 3
            Backstage,      // cam 4
            Kitchen,        // cam 5
            LeftHall,       // cam 6
            Storage,        // cam 7
            RightHall,      // cam 8
            LeftDoor,       // left door
            RightDoor,      // right door
            You             // you
        }

        public struct Connection
        {
            public Location Up;
            public Location Down;
            public Location Left;
            public Location Right;
        }
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        public static readonly Dictionary<Location, Connection> CameraConnections = new()
        {
            {
                Location.ShowStage,
                new Connection
                {
                    Up = Location.Backstage,
                    Down = Location.DiningArea,
                    Left = Location.SecondaryStage,
                    Right = Location.PartyRoomC
                }
            },
            {
                Location.DiningArea,
                new Connection
                {
                    Up = Location.ShowStage,
                    Down = Location.Kitchen,
                    Left = Location.PlayArea,
                    Right = Location.PartyRoomD
                }
            },
            {
                Location.SecondaryStage,
                new Connection
                {
                    Up = Location.NOWHERE,
                    Down = Location.PlayArea,
                    Left = Location.MainEntrance,
                    Right = Location.ShowStage
                }
            },
            {
                Location.PlayArea,
                new Connection
                {
                    Up = Location.SecondaryStage,
                    Down = Location.Storage,
                    Left = Location.PartyRoomB,
                    Right = Location.DiningArea
                }
            },
            {
                Location.PartyRoomA,
                new Connection
                {
                    Up = Location.NOWHERE,
                    Down = Location.PartyRoomB,
                    Left = Location.NOWHERE,
                    Right = Location.MainEntrance
                }
            },
            {
                Location.PartyRoomB,
                new Connection
                {
                    Up = Location.PartyRoomA,
                    Down = Location.LeftHall,
                    Left = Location.NOWHERE,
                    Right = Location.PlayArea
                }
            },
            {
                Location.PartyRoomC,
                new Connection
                {
                    Up = Location.NOWHERE,
                    Down = Location.PartyRoomD,
                    Left = Location.ShowStage,
                    Right = Location.NOWHERE
                }
            },
            {
                Location.PartyRoomD,
                new Connection
                {
                    Up = Location.PartyRoomC,
                    Down = Location.NOWHERE,
                    Left = Location.DiningArea,
                    Right = Location.NOWHERE
                }
            },
            {
                Location.MainEntrance,
                new Connection
                {
                    Up = Location.NOWHERE,
                    Down = Location.PlayArea,
                    Left = Location.PartyRoomA,
                    Right = Location.SecondaryStage
                }
            },
            {
                Location.Backstage,
                new Connection
                {
                    Up = Location.NOWHERE,
                    Down = Location.ShowStage,
                    Left = Location.MainEntrance,
                    Right = Location.NOWHERE
                }
            },
            {
                Location.Kitchen,
                new Connection
                {
                    Up = Location.DiningArea,
                    Down = Location.Storage,
                    Left = Location.PlayArea,
                    Right = Location.NOWHERE
                }
            },
            {
                Location.LeftHall,
                new Connection
                {
                    Up = Location.PartyRoomB,
                    Down = Location.NOWHERE,
                    Left = Location.NOWHERE,
                    Right = Location.RightHall
                }
            },
            {
                Location.Storage,
                new Connection
                {
                    Up = Location.PlayArea,
                    Down = Location.RightHall,
                    Left = Location.LeftHall,
                    Right = Location.Kitchen
                }
            },
            {
                Location.RightHall,
                new Connection
                {
                    Up = Location.Storage,
                    Down = Location.NOWHERE,
                    Left = Location.LeftHall,
                    Right = Location.NOWHERE
                }
            },
        };
    }
}
