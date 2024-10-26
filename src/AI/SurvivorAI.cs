using System.Collections.Generic;
using UnityEngine;
using Location = FCAP.Map.Location;

namespace FCAP.AI
{
    /// <summary>
    /// Equivalent of Bonnie. Sticks to left side. Sticks to the middle/back of cameras.
    /// </summary>
    internal class SurvivorAI(GameController game, int night) : BaseAI(game, Enums.Animatronic.Survivor, Location.ShowStage, NightDifficulties[night], 206)
    {
        private static readonly int[] NightDifficulties = [0, 3, 0, 2, 6, 12, -1];
        private static readonly Dictionary<Location, Location[]> MoveMap = new()
        {
            {
                Location.ShowStage,
                [
                    Location.Backstage,
                    Location.DiningArea,
                    Location.MainEntrance,
                    Location.PlayArea
                ]
            },
            {
                Location.DiningArea,
                [
                    Location.ShowStage,
                    Location.MainEntrance,
                    Location.PlayArea,
                    Location.PlayArea,
                ]
            },
            {
                Location.PlayArea,
                [
                    Location.ShowStage,
                    Location.DiningArea,
                    Location.MainEntrance,
                    Location.PartyRoomA,
                    Location.PartyRoomB,
                    Location.LeftHall,
                    Location.LeftHall,
                    Location.LeftHall,
                    Location.LeftHall,
                ]
            },
            {
                Location.PartyRoomA,
                [
                    Location.MainEntrance,
                    Location.PlayArea,
                    Location.PlayArea,
                    Location.PartyRoomB,
                    Location.LeftHall
                ]
            },
            {
                Location.PartyRoomB,
                [
                    Location.MainEntrance,
                    Location.PlayArea,
                    Location.PlayArea,
                    Location.PartyRoomA,
                    Location.LeftHall,
                    Location.LeftHall
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
                    Location.ShowStage,
                    Location.DiningArea,
                    Location.PlayArea
                ]
            },
            {
                Location.LeftHall,
                [
                    Location.LeftDoor,
                    Location.LeftDoor,
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
                    Location.LeftHall,
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
                if (lastHour == 2 || lastHour == 3 || lastHour == 4)
                {
                    difficulty++;
                }
            }
        }

        public override Location NextMove()
        {
            if (location == Location.LeftDoor && !game.LeftDoorShut)
            {
                return Location.You;
            }
            return MoveMap.TryGetValue(location, out var next) ? next[Random.Range(0, next.Length)] : location;
        }
    }
}
