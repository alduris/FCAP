using UnityEngine;
using static FCAP.Map;

namespace FCAP.AI
{
    /// <summary>
    /// Sort of a less threatening version of Freddy because it can go back to stage, but at the same time it also has two paths and they are both short.
    /// Once at a door, it has a much smaller chance to go back to stage, instead opting to stay at the door.
    /// In cameras, stays to the sides/partially hidden. Harder to see, though not as hard as nightcat.
    /// </summary>
    internal class HunterAI(GameController game, int night) : BaseAI(game, Enums.Animatronic.Hunter, Map.Location.ShowStage, NightDifficulties[night], 183)
    {
        private static readonly int[] NightDifficulties = [0, 1, 3, 3, 5, 8, -1];
        private const float ReturnHomeChance = 1f / 3f;

        public override Location NextMove()
        {
            return location switch
            {
                Location.ShowStage => Random.value < 0.5f ? Location.MainEntrance : Location.PartyRoomD,

                Location.MainEntrance => Location.PartyRoomB,
                Location.PartyRoomB => Location.LeftHall,
                Location.LeftHall => Location.LeftDoor,
                Location.LeftDoor => Random.value < ReturnHomeChance ? Location.ShowStage : location,

                Location.PartyRoomD => Location.Kitchen,
                Location.Kitchen => Location.Storage,
                Location.Storage => Location.RightDoor,
                Location.RightDoor => Random.value < ReturnHomeChance ? Location.ShowStage : location,
                _ => location
            };
        }
    }
}
