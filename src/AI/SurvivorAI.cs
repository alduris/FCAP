using System.Collections.Generic;
using UnityEngine;
using Location = FCAP.Map.Location;

namespace FCAP.AI
{
    /// <summary>
    /// Equivalent of Bonnie. Sticks to left side. Sticks to the middle/back of cameras.
    /// </summary>
    internal class SurvivorAI(GameController game, int night) : BaseAI(game, Enums.Animatronic.Survivor, Location.ShowStage, NightDifficulties[night], 226)
    {
        private const float LurkChance = 0.35f;
        private static readonly int[] NightDifficulties = [0, 5, 2, 4, 7, 10, -1];
        private static readonly Dictionary<Location, Location[]> MoveMap = new()
        {
            {
                Location.ShowStage,
                [
                    Location.Backstage,
                    Location.DiningArea,
                    Location.PlayArea,
                    Location.MainEntrance,
                ]
            },
            {
                Location.DiningArea,
                [
                    Location.PlayArea,
                    Location.PlayArea,
                    Location.MainEntrance,
                    Location.ShowStage,
                ]
            },
            {
                Location.PlayArea,
                [
                    Location.LeftHall,
                    Location.LeftHall,
                    Location.LeftHall,
                    Location.ShowStage,
                    Location.PartyRoomA,
                    Location.PartyRoomB,
                    Location.DiningArea,
                    Location.MainEntrance,
                ]
            },
            {
                Location.PartyRoomA,
                [
                    Location.PlayArea,
                    Location.PlayArea,
                    Location.LeftHall,
                    Location.MainEntrance,
                    Location.PartyRoomB,
                ]
            },
            {
                Location.PartyRoomB,
                [
                    Location.LeftHall,
                    Location.LeftHall,
                    Location.PlayArea,
                    Location.PlayArea,
                    Location.MainEntrance,
                    Location.PartyRoomA,
                ]
            },
            {
                Location.MainEntrance,
                [
                    Location.PartyRoomA,
                    Location.PartyRoomB,
                    Location.PlayArea,
                    Location.PlayArea,
                    Location.ShowStage,
                    Location.LeftHall,
                ]
            },
            {
                Location.Backstage,
                [
                    Location.DiningArea,
                    Location.ShowStage,
                    Location.PlayArea,
                ]
            },
            {
                Location.LeftHall,
                [
                    Location.LeftDoor,
                    Location.LeftDoor,
                    Location.LeftDoor,
                    Location.MainEntrance,
                    Location.PlayArea,
                    Location.PlayArea
                ]
            },
            {
                Location.LeftDoor,
                [
                    Location.ShowStage,
                    Location.ShowStage,
                    Location.LeftHall,
                ]
            },
        };

        private int lastHour = 0;

        public override void Update()
        {
            base.Update();
            
            if (lastHour != game.Hour)
            {
                lastHour = game.Hour;
                if (lastHour == 2 || lastHour == 3 || lastHour == 4)
                {
                    difficulty++;
                }
            }
        }

        public override Location NextMove()
        {
            if (location == Location.LeftDoor)
            {
                if (!game.LeftDoorShut)
                {
                    return Location.You;
                }
                else if (Random.value < LurkChance)
                {
                    return Location.LeftDoor;
                }
            }
            // Bias towards lower values so earlier in list gets preference
            return MoveMap.TryGetValue(location, out var next) ? next[(int)(Mathf.Pow(Random.value, 2f) * next.Length)] : location;
        }
    }
}
