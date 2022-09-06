using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using Potato.Room.Wall;

namespace Potato.Room
{
    internal struct SimpleLevelSave
    {
        public struct WallNode
        {
            public string Identifier { get; set; }
            public float X { get; set; }
            public float Y { get; set; }
        }
        public List<WallNode> WallNodes { get; set; }
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
            save.WallNodes = walls
                .Select((x) => (x, x.Position))
                .Where((x) => x.x is IIdentifiable)
                .Select((x) => new SimpleLevelSave.WallNode() { Identifier = ((IIdentifiable)x.x).Identifier, X = x.Position.X, Y = x.Position.Y } )
                .ToList();
            return save;
        }

        public void Load(SimpleLevelSave save)
        {
            foreach (SimpleLevelSave.WallNode wallNode in save.WallNodes)
            {
                IWallable wall = WallManager.GetWall(identifier: wallNode.Identifier);
                wall.Position = new Vector2(x: wallNode.X, y: wallNode.Y);
                walls.Add(wall);
            }
        }
    }
}
