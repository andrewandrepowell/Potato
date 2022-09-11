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
        private CollisionManager collisionManager;
        private List<IElement> elements;
        private bool softPaused;
        private bool hardPaused;
        public ICollection<IElement> Elements { get => elements; set => throw new NotImplementedException(); }
        public IController Controller { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IOpenable.OpenStates OpenState => roomStateChanger.OpenState;
        public OrthographicCamera Camera { get => camera; set => throw new NotImplementedException(); }
        public CollisionManager Collision { get => collisionManager; set => throw new NotImplementedException(); }
        public bool SoftPaused => softPaused;
        public bool HardPaused => hardPaused;

        public SimpleLevel()
        {
            camera = new OrthographicCamera(graphicsDevice: Potato.Game.GraphicsDevice);
            roomStateChanger = new RoomStateChanger();
            collisionManager = new CollisionManager();
            elements = new List<IElement>();
            softPaused = false;
            hardPaused = false;
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
            // Remove any destroyed elements from the element list.
            // This might get changed such that list is updated on a timer basis.
            elements
                .OfType<IDestroyable>()
                .Where((x) => x.Destroyed)
                .ToList().ForEach((x) => elements.Remove((IElement)x));

            // For each updateable element, update the element.
            foreach (IUpdateable updateable in elements.OfType<IUpdateable>())
                updateable.Update(gameTime: gameTime);

            // Update the other updateables related to the level.
            roomStateChanger.Update(gameTime: gameTime);
            collisionManager.Update(gameTime: gameTime);
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
                element.Position = new Vector2(x: elementNode.Position.X, y: elementNode.Position.Y);
                elements.Add(element);
            }
            camera.Position = new Vector2(x: save.Camera.Position.X, y: save.Camera.Position.Y);
        }

        public void SoftPause()
        {
            if (softPaused)
                return;
            softPaused = true;
            foreach (IPausible pausible in elements.OfType<IPausible>())
                pausible.SoftPause();
        }

        public void SoftResume()
        {
            if (!softPaused)
                return;
            softPaused = false;
            foreach (IPausible pausible in elements.OfType<IPausible>())
                pausible.SoftResume();
        }

        public void HardPause()
        {
            if (hardPaused)
                return;
            hardPaused = true;
            foreach (IPausible pausible in elements.OfType<IPausible>())
                pausible.HardPause();
        }

        public void HardResume()
        {
            if (!hardPaused)
                return;
            hardPaused = false;
            foreach (IPausible pausible in elements.OfType<IPausible>())
                pausible.HardResume();
        }
    }
}
