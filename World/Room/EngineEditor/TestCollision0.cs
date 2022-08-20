using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato.World.Room.EngineEditor
{
    internal class TestCollision0 : IComponent
    {
        private class TestObject : IComponent, ICollidable
        {
            private string name;
            private Texture2D texture;
            private Vector2 position;
            private Vector2 collisionPoint;
            private bool destroyed;
            private int collideCounter;
            private bool performCorrection;
            private List<Vector2> collisionVertices;
            public bool Collidable { get => true; set => throw new NotImplementedException(); }
            public Texture2D CollisionMask => texture;
            public IList<Vector2> CollisionVertices => collisionVertices;
            public Vector2 Position { get => position; set => position = value; }
            public bool Destroyed => destroyed;

            public TestObject(string name, Texture2D texture, bool performCorrection=false)
            {
                this.name = name;
                this.texture = texture;
                position = Vector2.Zero;
                destroyed = false;
                collideCounter = 0;
                this.performCorrection = performCorrection;
                collisionPoint = Vector2.Zero;
                collisionVertices = CollisionManager.AcquireVertices(mask: texture, startColor: Color.Yellow, vertixColor: Color.Red);
            }

            public void Dispose() => destroyed = true;

            public void Draw(Matrix? transformMatrix = null)
            {
                if (destroyed)
                    return;
                SpriteBatch spriteBatch = Potato.SpriteBatch;
                spriteBatch.Begin(transformMatrix: transformMatrix);
                spriteBatch.Draw(
                    texture: texture,
                    position: position.ToPoint().ToVector2(),
                    color: Color.White);
                spriteBatch.DrawPoint(position: collisionPoint, color: Color.Red, size: 4);
                spriteBatch.End();
            }

            public void ServiceCollision(ICollidable.Info info)
            {
                if (destroyed)
                    return;

                collideCounter++;
                Console.WriteLine($"Name: {name}");
                Console.WriteLine($"Collision Occurred. Number of collisions: {collideCounter}");
                Console.WriteLine($"Correction Vector: {info.Correction}");
                Console.WriteLine($"Collision Point: {info.Point}");

                if (performCorrection)
                    position += info.Correction;
                collisionPoint = info.Point;
            }

            public void Update(GameTime gameTime)
            {
                if (destroyed)
                    return;
            }
        }
        private CollisionManager collisionManager;
        private TestObject testObject0;
        private TestObject testObject1;

        public TestCollision0()
        {
            collisionManager = new CollisionManager();
            Texture2D texture = Potato.Game.Content.Load<Texture2D>(assetName: "test0");
            testObject0 = new TestObject(name: "Test Object 0", texture: texture, performCorrection: false);
            testObject1 = new TestObject(name: "Test Object 1", texture: texture, performCorrection: true);
            testObject1.Position = new Vector2(x: 256, y: 256);
            collisionManager.ManagedCollidables.Add(testObject0);
            collisionManager.ManagedCollidables.Add(testObject1);
        }

        public void Draw(Matrix? transformMatrix = null)
        {
            testObject0.Draw(transformMatrix: transformMatrix);
            testObject1.Draw(transformMatrix: transformMatrix);
        }

        public void Update(GameTime gameTime)
        {
            MouseStateExtended mouseState = MouseExtended.GetState();
            if (mouseState.WasButtonJustDown(button: MouseButton.Left))
                testObject0.Position = mouseState.Position.ToVector2();
            if (mouseState.WasButtonJustDown(button: MouseButton.Right))
                testObject1.Position = mouseState.Position.ToVector2();

            testObject0.Update(gameTime: gameTime);
            testObject1.Update(gameTime: gameTime);
            collisionManager.Update(gameTime: gameTime);
        }
    }
}
