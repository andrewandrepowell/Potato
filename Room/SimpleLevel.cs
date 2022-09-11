using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using MonoGame.Extended;
using Potato.Element.Character;
using Potato.Element.Wall;
using Potato.Element;

namespace Potato.Room
{
    internal struct SimpleLevelSave
    {
        public struct VectorNode
        {
            public float X { get; set; }
            public float Y { get; set; }
        }
        public struct ElementNode
        {
            public string Identifier { get; set; }
            public VectorNode Position { get; set; }
        }
        public struct CameraNode
        {
            public VectorNode Position { get; set; }
        }
        public List<ElementNode> Elements { get; set; }
        public CameraNode Camera { get; set; }
    }
    internal class SimpleLevel : ILevel, ISavable<SimpleLevelSave>
    {
        private OrthographicCamera camera;
        private RoomStateChanger roomStateChanger;
        private List<IElement> elements;
        public ICollection<IElement> Elements { get => elements; set => throw new NotImplementedException(); }
        public IController Controller { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IOpenable.OpenStates OpenState => roomStateChanger.OpenState;

        public OrthographicCamera Camera { get => camera; set => throw new NotImplementedException(); }

        public SimpleLevel()
        {
            camera = new OrthographicCamera(graphicsDevice: Potato.Game.GraphicsDevice);
            roomStateChanger = new RoomStateChanger();
            elements = new List<IElement>();
        }

        public void Close()
        {
            roomStateChanger.Close();
        }

        public void Draw(Matrix? transformMatrix = null)
        {
            foreach (IDrawable drawable in elements.OfType<IDrawable>())
                drawable.Draw(transformMatrix: transformMatrix);
            roomStateChanger.Draw(transformMatrix: null);
        }

        public void HardReset()
        {
            foreach (IResetable resetable in elements.OfType<IResetable>())
                resetable.HardReset();
            roomStateChanger.HardReset();
        }

        public void Open()
        {
            roomStateChanger.Open();
        }

        public void SoftReset()
        {
            foreach (IResetable resetable in elements.OfType<IResetable>())
                resetable.SoftReset();
            roomStateChanger.SoftReset();
        }

        public void Update(GameTime gameTime)
        {
            foreach (IUpdateable updateable in elements.OfType<IUpdateable>())
                updateable.Update(gameTime: gameTime);
            roomStateChanger.Update(gameTime: gameTime);
        }

        public SimpleLevelSave Save()
        {
            SimpleLevelSave save = new SimpleLevelSave();
            save.Elements = elements
                .Select((x) => (x, x.Position))
                .Where((x) => x.x is IIdentifiable)
                .Select((x) => new SimpleLevelSave.ElementNode() 
                { 
                    Identifier = ((IIdentifiable)x.x).Identifier, 
                    Position = new SimpleLevelSave.VectorNode
                    { 
                        X = x.Position.X, 
                        Y = x.Position.Y 
                    } 
                } )
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
            foreach (SimpleLevelSave.ElementNode elementNode in save.Elements)
            {
                IElement element = ElementManager.GetElement(elementNode.Identifier);
                element.Position = new Vector2(x: element.Position.X, y: element.Position.Y);
                elements.Add(element);
            }
            camera.Position = new Vector2(x: save.Camera.Position.X, y: save.Camera.Position.Y);
        }
    }
}
