using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using Potato.Room.Wall;
using MonoGame.Extended;
using Potato.Character;

namespace Potato.Room
{
    internal struct SimpleLevelSave
    {
        public struct VectorNode
        {
            public float X { get; set; }
            public float Y { get; set; }
        }
        public struct WallNode
        {
            public string Identifier { get; set; }
            public VectorNode Position { get; set; }
        }
        public struct CharacterNode
        {
            public string Identifier { get; set; }
            public VectorNode Position { get; set; }
        }
        public struct CameraNode
        {
            public VectorNode Position { get; set; }
        }
        public List<WallNode> Walls { get; set; }
        public List<CharacterNode> Characters { get; set; }
        public CameraNode Camera { get; set; }
    }
    internal class SimpleLevel : ILevel, ISavable<SimpleLevelSave>
    {
        private OrthographicCamera camera;
        private RoomStateChanger roomStateChanger;
        private List<IWallable> walls;
        private List<ICharacterizable> characters;
        public ICollection<IWallable> Walls { get => walls; set => throw new NotImplementedException(); }
        public ICollection<ICharacterizable> Characters { get => characters; set => throw new NotImplementedException(); }
        public IController Controller { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IOpenable.OpenStates OpenState => roomStateChanger.OpenState;

        public OrthographicCamera Camera { get => camera; set => throw new NotImplementedException(); }

        public SimpleLevel()
        {
            camera = new OrthographicCamera(graphicsDevice: Potato.Game.GraphicsDevice);
            roomStateChanger = new RoomStateChanger();
            walls = new List<IWallable>();
            characters = new List<ICharacterizable>();
        }

        public void Close()
        {
            roomStateChanger.Close();
        }

        public void Draw(Matrix? transformMatrix = null)
        {
            foreach (IDrawable drawable in walls)
                drawable.Draw(transformMatrix: transformMatrix);
            foreach (IDrawable drawable in characters)
                drawable.Draw(transformMatrix: transformMatrix);
            roomStateChanger.Draw(transformMatrix: null);
        }

        public void HardReset()
        {
            foreach (IResetable resetable in characters)
                resetable.HardReset();
            roomStateChanger.HardReset();
        }

        public void Open()
        {
            roomStateChanger.Open();
        }

        public void SoftReset()
        {
            foreach (IResetable resetable in characters)
                resetable.SoftReset();
            roomStateChanger.SoftReset();
        }

        public void Update(GameTime gameTime)
        {
            foreach (IUpdateable updateable in characters)
                updateable.Update(gameTime: gameTime);
            roomStateChanger.Update(gameTime: gameTime);
        }

        public SimpleLevelSave Save()
        {
            SimpleLevelSave save = new SimpleLevelSave();
            save.Walls = walls
                .Select((x) => (x, x.Position))
                .Where((x) => x.x is IIdentifiable)
                .Select((x) => new SimpleLevelSave.WallNode() 
                { 
                    Identifier = ((IIdentifiable)x.x).Identifier, 
                    Position = new SimpleLevelSave.VectorNode
                    { 
                        X = x.Position.X, 
                        Y = x.Position.Y 
                    } 
                } )
                .ToList();
            save.Characters = characters
                 .Select((x) => (x, x.Position))
                 .Where((x) => x.x is IIdentifiable)
                 .Select((x) => new SimpleLevelSave.CharacterNode()
                 {
                     Identifier = ((IIdentifiable)x.x).Identifier,
                     Position = new SimpleLevelSave.VectorNode
                     {
                         X = x.Position.X,
                         Y = x.Position.Y
                     }
                 })
                 .ToList();
            save.Camera = new SimpleLevelSave.CameraNode
            {
                Position = new SimpleLevelSave.VectorNode
                {
                    X = camera.Position.X,
                    Y = camera.Position.Y
                }
            };
            return save;
        }

        public void Load(SimpleLevelSave save)
        {
            foreach (SimpleLevelSave.WallNode wallNode in save.Walls)
            {
                IWallable wall = WallManager.GetWall(identifier: wallNode.Identifier);
                wall.Position = new Vector2(x: wallNode.Position.X, y: wallNode.Position.Y);
                walls.Add(wall);
            }
            foreach (SimpleLevelSave.CharacterNode characterNode in save.Characters)
            {
                ICharacterizable character = CharacterManager.GetCharacter(identifier: characterNode.Identifier);
                character.Position = new Vector2(x: characterNode.Position.X, y: characterNode.Position.Y);
                characters.Add(character);
            }
            camera.Position = new Vector2(x: save.Camera.Position.X, y: save.Camera.Position.Y);
        }
    }
}
