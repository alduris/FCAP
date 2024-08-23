using RWCustom;
using UnityEngine;

namespace FCAP.Hooks
{
    internal static class PlacedObjectHooks
    {
        public static void Apply()
        {
            On.PlacedObject.GenerateEmptyData += PlacedObject_GenerateEmptyData;
            On.DevInterface.ObjectsPage.CreateObjRep += ObjectsPage_CreateObjRep;
            On.DevInterface.ObjectsPage.DevObjectGetCategoryFromPlacedType += ObjectsPage_DevObjectGetCategoryFromPlacedType;
            On.Room.Loaded += Room_Loaded;
        }

        private static void Room_Loaded(On.Room.orig_Loaded orig, Room self)
        {
            bool firstTimeRealized = self.abstractRoom.firstTimeRealized;
            orig(self);
            if (self.game == null) return;

            try
            {
                self.abstractRoom.firstTimeRealized = firstTimeRealized;
                foreach (var item in self.roomSettings.placedObjects)
                {
                    if (item.active && item.type == Constants.SunblockType)
                    {
                        self.AddObject(new Sunblock(item));
                    }
                }
            }
            finally
            {
                self.abstractRoom.firstTimeRealized = false;
            }
        }

        private static DevInterface.ObjectsPage.DevObjectCategories ObjectsPage_DevObjectGetCategoryFromPlacedType(On.DevInterface.ObjectsPage.orig_DevObjectGetCategoryFromPlacedType orig, DevInterface.ObjectsPage self, PlacedObject.Type type)
        {
            return type == Constants.SunblockType ? DevInterface.ObjectsPage.DevObjectCategories.Unsorted : orig(self, type);
        }

        private static void ObjectsPage_CreateObjRep(On.DevInterface.ObjectsPage.orig_CreateObjRep orig, DevInterface.ObjectsPage self, PlacedObject.Type tp, PlacedObject pObj)
        {
            if (tp == Constants.SunblockType)
            {
                if (pObj == null)
                {
                    pObj = new PlacedObject(tp, null)
                    {
                        pos = self.owner.room.game.cameras[0].pos + Vector2.Lerp(self.owner.mousePos, new Vector2(-683f, 384f), 0.25f) + Custom.DegToVec(Random.value * 360f) * 0.2f
                    };
                    self.RoomSettings.placedObjects.Add(pObj);
                }
                var rep = new SunblockRepresentation(self.owner, tp.ToString() + "_Rep", self, pObj);
                self.tempNodes.Add(rep);
                self.subNodes.Add(rep);
            }
            else
            {
                orig(self, tp, pObj);
            }
        }

        private static void PlacedObject_GenerateEmptyData(On.PlacedObject.orig_GenerateEmptyData orig, PlacedObject self)
        {
            if (self.type == Constants.SunblockType)
            {
                self.data = new Sunblock.SunblockData(self);
            }
            else
            {
                orig(self);
            }
        }
    }
}
