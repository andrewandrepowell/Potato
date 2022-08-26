using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Potato.World.Room.EngineEditor
{
    internal class TestPhysics0 : IComponent
    {
        private class TestObject : IComponent, IPhysical
        {
            private string name;
            private Texture2D texture;
            private Vector2 collisionPoint;
            private Vector2 collisionNormal;
            private bool destroyed;
            private int collideCounter;
            private bool performCorrection;
            private List<Vector2> collisionVertices;
            private PhysicsChanger physicsChanger;
            public bool Grounded { get => physicsChanger.Grounded; set => physicsChanger.Grounded = value; }
            public bool Collidable { get => true; set => throw new NotImplementedException(); }
            public Texture2D CollisionMask { get => texture; set => throw new NotImplementedException(); }
            public IList<Vector2> CollisionVertices { get => collisionVertices; set => throw new NotImplementedException(); }
            public Vector2 Position { get => physicsChanger.Position; set => physicsChanger.Position = value; }
            public bool Destroyed => destroyed;
            public float Mass { get => physicsChanger.Mass; set => physicsChanger.Mass = value; }
            public Vector2 Velocity { get => physicsChanger.Velocity; set => physicsChanger.Velocity = value; }
            public Vector2 Acceleration { get => physicsChanger.Acceleration; set => physicsChanger.Acceleration = value; }
            public Vector2 Force { get => physicsChanger.Force; set => physicsChanger.Force = value; }
            public float MaxSpeed { get => physicsChanger.MaxSpeed; set => physicsChanger.MaxSpeed = value; }
            public float Friction { get => physicsChanger.Friction; set => physicsChanger.Friction = value; }
            public float Bounce { get => physicsChanger.Bounce; set => physicsChanger.Bounce = value; }
            public Vector2 Gravity { get => physicsChanger.Gravity; set => physicsChanger.Gravity = value; }

            public TestObject(string name, Texture2D texture, bool performCorrection = false)
            {
                this.name = name;
                this.texture = texture;
                destroyed = false;
                collideCounter = 0;
                this.performCorrection = performCorrection;
                collisionPoint = Vector2.Zero;
                collisionNormal = Vector2.Zero;
                collisionVertices = CollisionManager.GetVertices(
                    mask: texture, startColor: Color.Yellow, includeColor: Color.Red, excludeColor: new Color(r: 0, g: 255, b: 0, alpha: 255));
                physicsChanger = new PhysicsChanger();
            }

            public void Dispose()
            {
                destroyed = true;
                physicsChanger.Dispose();
            }

            public void Draw(Matrix? transformMatrix = null)
            {
                if (destroyed)
                    return;
                SpriteBatch spriteBatch = Potato.SpriteBatch;
                spriteBatch.Begin(transformMatrix: transformMatrix);
                spriteBatch.Draw(
                    texture: texture,
                    position: physicsChanger.Position.ToPoint().ToVector2(),
                    color: Color.White);
                spriteBatch.DrawPoint(position: collisionPoint, color: Color.Red, size: 6);
                spriteBatch.DrawLine(
                    point1: collisionPoint,
                    point2: collisionPoint + 64 * collisionNormal,
                    color: Color.Blue, thickness: 4);
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
                Console.WriteLine($"Collision Normal: {info.Normal}");

                if (performCorrection)
                    physicsChanger.Position += info.Correction;
                collisionPoint = info.Point;
                collisionNormal = info.Normal;

                physicsChanger.ServiceCollision(info: info);
            }

            public void Update(GameTime gameTime)
            {
                if (destroyed)
                    return;

                physicsChanger.Update(gameTime: gameTime);
            }
        }
        private CollisionManager collisionManager;
        private TestObject testPlayer;
        private List<TestObject> testObjects;

        public TestPhysics0()
        {
            collisionManager = new CollisionManager();
            int gameWidth = Potato.Game.GraphicsDevice.Viewport.Width;
            int gameHeight = Potato.Game.GraphicsDevice.Viewport.Height;
            TestObject testObject;
            testObjects = new List<TestObject>();

            testPlayer = new TestObject(
                name: "player",
                texture: Potato.Game.Content.Load<Texture2D>("test0"),
                performCorrection: true);
            testPlayer.Position = new Vector2(x: (gameWidth - testPlayer.CollisionMask.Width) / 2, y: 100);
            testPlayer.Mass = 1f;
            testPlayer.MaxSpeed = 1000f;
            testPlayer.Friction = 80;
            testPlayer.Gravity = new Vector2(x: 0, y: 1500);
            testObjects.Add(testPlayer);

            float otherFriction = 80;
            float otherBounce = .75f;
            testObject = new TestObject(
                name: "wall4",
                texture: Potato.Game.Content.Load<Texture2D>("test4"),
                performCorrection: false);
            testObject.Position = new Vector2(x: (gameWidth - testObject.CollisionMask.Width) / 2 - 256, y: 256);
            testObjects.Add(testObject);
            testObject = new TestObject(
                name: "wall3",
                texture: Potato.Game.Content.Load<Texture2D>("test3"),
                performCorrection: false)
            { Position = new Vector2(x: testObject.Position.X, y: testObject.Position.Y + testObject.CollisionMask.Height) };
            testObjects.Add(testObject);
            testObject = new TestObject(
                name: "wall0",
                texture: Potato.Game.Content.Load<Texture2D>("test1"),
                performCorrection: false)
            { Position = new Vector2(x: testObject.Position.X + testObject.CollisionMask.Width, y: testObject.Position.Y + testObject.CollisionMask.Height / 2) };
            testObjects.Add(testObject);
            testObject = new TestObject(
                name: "wall1",
                texture: Potato.Game.Content.Load<Texture2D>("test2"),
                performCorrection: false)
            { Position = new Vector2(x: testObject.Position.X + testObject.CollisionMask.Width, y: testObject.Position.Y) };
            testObjects.Add(testObject);
            testObject = new TestObject(
                name: "wall2",
                texture: Potato.Game.Content.Load<Texture2D>("test1"),
                performCorrection: false)
            { Position = new Vector2(
                x: testObject.Position.X + testObject.CollisionMask.Width, 
                y: testObject.Position.Y + testObject.CollisionMask.Height / 2) };
            testObjects.Add(testObject);

            foreach (IPhysical physical in testObjects.Where((x) => x != testPlayer))
            {
                physical.Friction = otherFriction;
                physical.Bounce = otherBounce;
            }

            collisionManager.ManagedCollidables.AddRange(testObjects);
        }
        
        public void Draw(Matrix? transformMatrix = null)
        {
            foreach (IDrawable drawable in testObjects)
                drawable.Draw(transformMatrix: transformMatrix);
        }

        public void Update(GameTime gameTime)
        {
            MouseStateExtended mouseState = MouseExtended.GetState();
            if (mouseState.IsButtonDown(button: MouseButton.Left))
                testPlayer.Force = 3000 * Vector2.Normalize(mouseState.Position.ToVector2() - (testPlayer.Position + testPlayer.CollisionMask.Bounds.Center.ToVector2()));
            else
                testPlayer.Force = Vector2.Zero;
            if (mouseState.WasButtonJustDown(button: MouseButton.Right))
            {
                testPlayer.Force = Vector2.Zero;
                testPlayer.Velocity = Vector2.Zero;
                testPlayer.Position = mouseState.Position.ToVector2() - testPlayer.CollisionMask.Bounds.Center.ToVector2();
            }
             

            foreach (IUpdateable updateable in testObjects)
                updateable.Update(gameTime: gameTime);
            collisionManager.Update(gameTime: gameTime);
        }
    }
}
