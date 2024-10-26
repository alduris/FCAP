using System.Collections.Generic;
using UnityEngine;
using static FCAP.Map;

namespace FCAP.AI
{
    /// <summary>
    /// Equivalent of Chica. Sticks to right side of map. Shows up close and personal in cameras.
    /// </summary>
    internal class MonkAI(GameController game, int night) : BaseAI(game, Enums.Animatronic.Monk, Map.Location.ShowStage, NightDifficulties[night], 213)
    {
        private static readonly int[] NightDifficulties = [0, 1, 4, 2, 5, 8, -1];
        private static readonly Dictionary<Location, Location[]> MoveMap = new()
        {
            {
                Location.ShowStage,
                [
                    Location.DiningArea,
                    Location.DiningArea,
                    Location.PlayArea,
                    Location.PlayArea,
                    Location.PartyRoomC,
                    Location.PartyRoomD,
                ]
            },
            {
                Location.DiningArea,
                [
                    Location.ShowStage,
                    Location.PartyRoomC,
                    Location.PartyRoomD,
                    Location.Kitchen,
                    Location.PlayArea,
                    Location.PlayArea,
                ]
            },
            {
                Location.PlayArea,
                [
                    Location.ShowStage,
                    Location.DiningArea,
                    Location.Kitchen,
                    Location.Storage,
                    Location.Storage,
                    Location.Storage
                ]
            },
            {
                Location.PartyRoomC,
                [
                    Location.ShowStage,
                    Location.DiningArea,
                    Location.DiningArea,
                    Location.PartyRoomD
                ]
            },
            {
                Location.PartyRoomD,
                [
                    Location.ShowStage,
                    Location.DiningArea,
                    Location.DiningArea,
                    Location.PartyRoomC,
                    Location.Kitchen
                ]
            },
            {
                Location.Kitchen,
                [
                    Location.DiningArea,
                    Location.DiningArea,
                    Location.PlayArea,
                    Location.PlayArea,
                    Location.PlayArea,
                    Location.ShowStage,
                    Location.Storage,
                ]
            },
            {
                Location.Storage,
                [
                    Location.PlayArea,
                    Location.PlayArea,
                    Location.Kitchen,
                    Location.RightHall,
                    Location.RightHall,
                    Location.RightHall,
                ]
            },
            {
                Location.RightHall,
                [
                    Location.RightDoor,
                    Location.RightDoor,
                    Location.RightDoor,
                    Location.RightDoor,
                    Location.RightDoor,
                    Location.PlayArea,
                    Location.Storage,
                    Location.Storage
                ]
            },
            {
                Location.RightDoor,
                [
                    Location.RightHall,
                    Location.Storage,
                    Location.ShowStage,
                    Location.ShowStage
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
                if (lastHour == 3 || lastHour == 4)
                {
                    difficulty++;
                }
            }
        }

        public override Map.Location NextMove()
        {
            if (location == Location.RightDoor && !game.RightDoorShut)
            {
                return Location.You;
            }
            return MoveMap.TryGetValue(location, out var next) ? next[Random.Range(0, next.Length)] : location;
        }
    }
}
