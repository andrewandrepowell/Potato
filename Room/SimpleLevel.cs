using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using Potato.Room.Wall;

namespace Potato.Room
{
    internal struct SimpleLevelSave
    {
        public List<(string, Vector2)> Walls { get; set; }
    }
    internal class SimpleLevel : ILevel, ISavable<SimpleLevelSave>
    {
        private RoomStateChanger roomStateChanger;
        private List<IWallable> walls;
        public ICollection<IWallable> Walls { get => walls; set => throw new NotImplementedException(); }
        public IController Controller { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IOpenable.OpenStates OpenState => roomStateChanger.OpenState;

        public SimpleLevel()
        {
            roomStateChanger = new RoomStateChanger();
            walls = new List<IWallable>();
        }

        public void Close()
        {
            roomStateChanger.Close();
        }

        public void Draw(Matrix? transformMatrix = null)
        {
            foreach (IDrawable drawable in Walls)
                drawable.Draw(transformMatrix: transformMatrix);
            roomStateChanger.Draw(transformMatrix: transformMatrix);
        }

        public void HardReset()
        {
            roomStateChanger.HardReset();
        }

        public void Open()
        {
            roomStateChanger.Open();
        }

        public void SoftReset()
        {
            roomStateChanger.SoftReset();
        }

        public void Update(GameTime gameTime)
        {
            roomStateChanger.Update(gameTime: gameTime);
        }

        public SimpleLevelSave Save()
        {
            SimpleLevelSave save = new SimpleLevelSave();
            save.Walls = walls
                .Select((x) => (x, x.Position))
                .Where((x) => x.x is IIdentifiable)
                .Select((x) => (((IIdentifiable)x.x).Identifier, x.Position))
                .ToList();
            return save;
        }

        public void Load(SimpleLevelSave save)
        {
            foreach ((string identifier, Vector2 position) in save.Walls)
            {
                IWallable wall = WallManager.GetWall(identifier: identifier);
                wall.Position = position;
                walls.Add(wall);
            }
        }
    }
}
