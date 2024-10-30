using System.Collections.Generic;
using UnityEngine;
using static FCAP.Map;

namespace FCAP.AI
{
    /// <summary>
    /// Equivalent of Chica. Sticks to right side of map. Shows up close and personal in cameras.
    /// </summary>
    internal class MonkAI(GameController game, int night) : BaseAI(game, Enums.Animatronic.Monk, Map.Location.ShowStage, NightDifficulties[night], 247)
    {
        private const float LurkChance = 0.35f;
        private static readonly int[] NightDifficulties = [0, 2, 5, 3, 6, 9, -1];
        private static readonly Dictionary<Location, Location[]> MoveMap = new()
        {
            {
                Location.ShowStage,
                [
                    Location.DiningArea,
                    Location.PlayArea,
                    Location.PartyRoomC,
                    Location.PartyRoomD,
                    Location.Kitchen,
                ]
            },
            {
                Location.DiningArea,
                [
                    Location.PlayArea,
                    Location.PlayArea,
                    Location.ShowStage,
                    Location.Kitchen,
                    Location.PartyRoomC,
                    Location.PartyRoomD,
                ]
            },
            {
                Location.PlayArea,
                [
                    Location.Storage,
                    Location.Storage,
                    Location.Storage,
                    Location.DiningArea,
                    Location.Kitchen,
                    Location.ShowStage,
                ]
            },
            {
                Location.PartyRoomC,
                [
                    Location.DiningArea,
                    Location.DiningArea,
                    Location.ShowStage,
                    Location.PartyRoomD
                ]
            },
            {
                Location.PartyRoomD,
                [
                    Location.DiningArea,
                    Location.DiningArea,
                    Location.Kitchen,
                    Location.ShowStage,
                    Location.PartyRoomC,
                ]
            },
            {
                Location.Kitchen,
                [
                    Location.PlayArea,
                    Location.PlayArea,
                    Location.Storage,
                    Location.DiningArea,
                    Location.DiningArea,
                    Location.ShowStage,
                ]
            },
            {
                Location.Storage,
                [
                    Location.RightHall,
                    Location.RightHall,
                    Location.PlayArea,
                    Location.PlayArea,
                    Location.Kitchen,
                ]
            },
            {
                Location.RightHall,
                [
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
                    Location.ShowStage,
                    Location.ShowStage,
                    Location.RightHall,
                    Location.Storage,
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
            if (location == Location.RightDoor)
            {
                if (!game.RightDoorShut)
                {
                    return Location.You;
                }
                else if (Random.value < LurkChance)
                {
                    return Location.RightDoor;
                }
            }
            // Bias towards lower values so earlier in list gets preference
            return MoveMap.TryGetValue(location, out var next) ? next[(int)(Mathf.Pow(Random.value, 1.8f) * next.Length)] : location;
        }
    }
}
