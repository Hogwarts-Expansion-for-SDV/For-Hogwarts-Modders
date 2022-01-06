using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hogwarts
{
    class TrainSprite
    {
        public const string CursorsSheet = Game1.mouseCursorsName;
        public static readonly Rectangle TrainFront = new Rectangle(276, 512, 8 * 16, 4 * 16);
        public static readonly Rectangle PassengerCar1 = new Rectangle(448, 512, 8 * 16, 4 * 16);
        public static readonly Rectangle PassengerCar2 = new Rectangle(448, 448, 8 * 16, 4 * 16);
        public static readonly Rectangle OpenCargoCar = new Rectangle(320, 512, 8 * 16, 4 * 16);
        public static readonly Rectangle ClosedCargoCar = new Rectangle(192, 512, 8 * 16, 4 * 16);
        public static readonly uint TrainID = uint.Parse(ModEntry.Instance.ModManifest.UpdateKeys.Where(s => s.Contains("Nexus")).ToArray()[0].Split(':')[1]);
        public static readonly Random Random = Game1.random;
        public Vector2 StartingPosition;
        public string Type;
        public bool FacingLeft;
        public uint AmountOfTrainCars = 4;
        public List<TemporaryAnimatedSprite> TrainCars = new();
        public static List<TrainSprite> Trains = new();
        public uint NextCarPos;
        public List<Rectangle> CollisionBoxes = new();
        public GameLocation Location;
        public static readonly Rectangle Bridge = new(67 * Game1.tileSize, 8 * Game1.tileSize, 5, 9);
        public TrainSprite(IMonitor monitor, GameLocation location)
        {
            Location = location;
            monitor.Log("Creating new train sprite.");
            uint randNum = (uint)Random.Next(1);
            if (randNum == 0)
                Type = "Passenger";
            else
                Type = "Cargo";
            randNum = (uint)Random.Next(1);
            if (randNum == 0)
            {
                FacingLeft = true;
                StartingPosition = new(-8, 11);
                NextCarPos = 8;
            }
            else
            {
                FacingLeft = false;
                StartingPosition = new(71, 9);
                NextCarPos = 63;
            }
            TrainCars.Add(new TemporaryAnimatedSprite(CursorsSheet, TrainFront, StartingPosition, !FacingLeft, 0, Color.White)
            {
                motion = new(FacingLeft ? (float)Random.NextDouble() * 5 : (float)Random.NextDouble() * -5, 0),
                acceleration = new(FacingLeft ? ((float)Random.NextDouble() / 10) * 5 : ((float)Random.NextDouble() / 10) * -5, 0)
            }
            );
            AmountOfTrainCars = (uint)Random.Next(3, 7);
            for (int i = 1; i <= AmountOfTrainCars; i++)
            {
                uint position = (uint)(8 * i);
                uint randomNum = (uint)Random.Next(1);
                Rectangle src = new();
                if (Type == "Passenger")
                    switch (randomNum)
                    {
                        case 0:
                            src = PassengerCar1;
                            break;
                        case 1:
                            src = PassengerCar2;
                            break;
                    }
                if (Type == "Cargo")
                    switch (randomNum)
                    {
                        case 0:
                            src = OpenCargoCar;
                            break;
                        case 1:
                            src = ClosedCargoCar;
                            break;
                    }
                TrainCars.Add(new TemporaryAnimatedSprite(CursorsSheet, src, new(StartingPosition.X - (position * Game1.tileSize), StartingPosition.Y), !FacingLeft, 0, Color.White)
                {
                    motion = TrainCars[0].motion,
                    acceleration = TrainCars[0].motion
                }
            );
            }
            foreach (TemporaryAnimatedSprite tas in TrainCars)
            {
                Location.temporarySprites.Add(tas);
                CollisionBoxes.Add(new((int)Math.Round(tas.Position.X), (int)Math.Round(tas.Position.Y + 128), 8, 2));
            }
            Trains.Add(this);
        }
        public void Update()
        {
            foreach (TemporaryAnimatedSprite tas in TrainCars)
            {
                if (tas.Position.X > 100 || tas.Position.X < -15)
                {
                    Location.temporarySprites.Remove(tas);
                }
            }
            foreach (Rectangle box in CollisionBoxes)
            {
                if (box.Intersects(Game1.player.GetBoundingBox()) && !Bridge.Intersects(Game1.player.GetBoundingBox()))
                {
                    Game1.player.takeDamage(40, true, null);
                    Game1.warpFarmer(Game1.player.currentLocation.Name, 13, 17, 0);
                    Game1.drawObjectDialogue("Ow! I should probably be more careful when crossing the track.");
                }
            }
        }
    }
}
