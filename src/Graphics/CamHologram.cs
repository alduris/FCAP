using System;
using System.Collections.Generic;
using System.Linq;
using MoreSlugcats;
using OverseerHolograms;
using static OverseerHolograms.OverseerImage;

namespace FCAP.Graphics
{
    internal partial class CamHologram : OverseerHologram, IOwnAHoloImage
    {
        public GameController game;

        public HoloImage image;
        public CamHoloFrame frame;

        public CamHologram(GameController game, Overseer overseer, Message message, Creature communicateWith, float importance) : base(overseer, message, communicateWith, importance)
        {
            this.game = game;

            image = new CamHoloImage(this, totalSprites, this);
            AddPart(image);
            frame = new CamHoloFrame(this, totalSprites);
            AddPart(frame);
        }

        public CamHologram(AncientBot robo, Message message, Creature communicateWith, float importance) : base(robo, message, communicateWith, importance)
        {
            throw new NotImplementedException();
        }

        private int CamPhotoNum
        {
            get
            {
                int camnum = game.CamViewing switch
                {
                    Map.Location.ShowStage => 0,
                    Map.Location.DiningArea => 1,
                    Map.Location.SecondaryStage => 2,
                    Map.Location.PlayArea => 3,
                    Map.Location.PartyRoomA => 4,
                    Map.Location.PartyRoomB => 5,
                    Map.Location.PartyRoomC => 6,
                    Map.Location.PartyRoomD => 7,
                    Map.Location.MainEntrance => 8,
                    Map.Location.Backstage => 9,
                    Map.Location.Kitchen => 10,
                    Map.Location.RestroomHall => 11,
                    Map.Location.Storage => 12,
                    Map.Location.BackHall => 13,
                    _ => throw new NotImplementedException(),
                };

                // Binary operators my behated
                int bits = 0;
                for (int i = 0; i < game.AIs.Length; i++)
                {
                    if (game.AIs[i].location == game.CamViewing)
                    {
                        bits |= 1 << i;
                    }
                }
                return (camnum << game.AIs.Length) + bits;
            }
        }

        public int CurrImageIndex => CamPhotoNum % 25;
        public int CurrFileIndex => CamPhotoNum / 25;

        public int ShowTime => int.MaxValue;

        public ImageID CurrImage => Constants.CamsImageID;

        public float ImmediatelyToContent => 0.5f;
    }
}
