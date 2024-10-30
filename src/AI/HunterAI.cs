using UnityEngine;
using static FCAP.Map;

namespace FCAP.AI
{
    /// <summary>
    /// Sort of a less threatening version of Freddy because it can go back to stage, but at the same time it also has two paths and they are both short.
    /// Once at a door, it has a much smaller chance to go back to stage, instead opting to stay at the door.
    /// In cameras, stays to the sides/partially hidden. Harder to see, though not as hard as nightcat.
    /// </summary>
    internal class HunterAI(GameController game, int night) : BaseAI(game, Enums.Animatronic.Hunter, Map.Location.ShowStage, NightDifficulties[night], 203)
    {
        private static readonly int[] NightDifficulties = [0, 1, 3, 4, 7, 7, -1];
        private const float ReturnHomeChance = 0.65f;

        protected override int PowerDrainOnLeave => 30;

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
                Location.Kitchen => Location.RightHall,
                Location.RightHall => Location.RightDoor,
                Location.RightDoor => Random.value < ReturnHomeChance ? Location.ShowStage : location,
                _ => location
            };
        }

        public override bool CanJumpscare()
        {
            return (location == Location.LeftDoor && !game.LeftDoorShut) || (location == Location.RightDoor && !game.RightDoorShut) || base.CanJumpscare();
        }

        public override bool MoveCheck()
        {
            return base.MoveCheck() && (!game.InCams || location == Location.LeftDoor || location == Location.RightDoor);
        }
    }
}
