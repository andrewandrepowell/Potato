using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato.Element.Character
{
    internal class Player : ICharacterizable, IControllable, IIdentifiable
    {
        private Bag<IProjectile> projectiles;
        private PhysicsChanger physicsChanger;
        private Texture2D collisionMask;
        private List<Vector2> collisionVertices;
        private IController controller;
        private string identifier;
        private bool collidable;
        private bool destroyed;
        private bool softPaused;
        private bool hardPaused;
        private bool jumpLock;
        private float horizontalForce;
        private float verticalForce;
        private const float jumpTimerThreshold = 0.1f;
        private float jumpTimer;
        public float HorizontalForce { get => horizontalForce; set => horizontalForce = value; }
        public float VerticalForce { get => verticalForce; set => verticalForce = value; }
        public Bag<IProjectile> Projectiles { get => projectiles; set => throw new NotImplementedException(); }
        public float Mass { get => physicsChanger.Mass; set => physicsChanger.Mass = value; }
        public float MaxSpeed { get => physicsChanger.MaxSpeed; set => physicsChanger.MaxSpeed = value; }
        public float Friction { get => physicsChanger.Friction; set => physicsChanger.Friction = value; }
        public float Bounce { get => physicsChanger.Bounce; set => physicsChanger.Bounce = value; }
        public bool Grounded { get => physicsChanger.Grounded; set => physicsChanger.Grounded = value; }
        public Vector2 Velocity { get => physicsChanger.Velocity; set => physicsChanger.Velocity = value; }
        public Vector2 Acceleration { get => physicsChanger.Acceleration; set => physicsChanger.Acceleration = value; }
        public Vector2 Force { get => physicsChanger.Force; set => physicsChanger.Force = value; }
        public Vector2 Gravity { get => physicsChanger.Gravity; set => physicsChanger.Gravity = value; }
        public Vector2 Orientation { get => physicsChanger.Orientation; set => physicsChanger.Orientation = value; }
        public bool Collidable { get => collidable; set => collidable = value; }
        public Texture2D CollisionMask { get => collisionMask; set => throw new NotImplementedException(); }
        public IList<Vector2> CollisionVertices { get => collisionVertices; set => throw new NotImplementedException(); }
        public Size2 Size { get => collisionMask.Bounds.Size; set => throw new NotImplementedException(); }
        public Vector2 Position { get => physicsChanger.Position; set => physicsChanger.Position = value; }
        public bool Destroyed => destroyed;
        public bool SoftPaused => softPaused;
        public bool HardPaused => hardPaused;
        public IController Controller { get => controller; set => controller = value; }
        public string Identifier { get => identifier; set => identifier = value; }

        public Player()
        {
            projectiles = new Bag<IProjectile>();
            physicsChanger = new PhysicsChanger();
            collisionMask = Potato.Game.Content.Load<Texture2D>("characters/mask_player");
            collisionVertices = new List<Vector2>();
            controller = null;
            identifier = "";
            collidable = false;
            destroyed = false;
            softPaused = false;
            hardPaused = false;
            jumpLock = false;
            horizontalForce = 0.0f;
            verticalForce = 0.0f;
            jumpTimer = 0.0f;
        }

        public void Dispose()
        {
            destroyed = true;
            foreach (IDestroyable destroyable in projectiles)
                destroyable.Dispose();
            projectiles.Clear();
            physicsChanger.Dispose();
        }

        public void Draw(Matrix? transformMatrix = null)
        {
            SpriteBatch spriteBatch = Potato.SpriteBatch;
            spriteBatch.Begin(transformMatrix: transformMatrix);
            spriteBatch.Draw(
                texture: collisionMask, 
                position: physicsChanger.Position
                    .ToPoint()
                    .ToVector2(), 
                color: Color.White);
            spriteBatch.End();
        }

        public void HardPause()
        {
            hardPaused = true;
            physicsChanger.HardPause();
        }

        public void HardResume()
        {
            hardPaused = false;
            physicsChanger.HardResume();
        }

        public void ServiceCollision(ICollidable.Info info)
        {
            if (destroyed)
                return;
            physicsChanger.Position += info.Correction;
            physicsChanger.ServiceCollision(info: info);
        }

        public void SoftPause()
        {
            softPaused = true;
            physicsChanger.SoftPause();
        }

        public void SoftResume()
        {
            softPaused = false;
            physicsChanger.SoftResume();
        }

        public void Update(GameTime gameTime)
        {
            if (destroyed || hardPaused)
                return;

            float timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;


            if (controller != null && !softPaused)
            {
                float upValue = controller.UpHeld();
                float leftValue = controller.LeftHeld();
                float rightValue = controller.RightHeld();

                {
                    if (upValue > 0)
                    {
                        if (jumpTimer > 0)
                        {
                            jumpTimer -= timeElapsed;
                            physicsChanger.Force += verticalForce * physicsChanger.Orientation * upValue;
                        }
                        else if (physicsChanger.Grounded && !jumpLock)
                        {
                            jumpTimer = jumpTimerThreshold;
                            jumpLock = true;
                        }
                    }
                    else
                    {
                        jumpTimer = 0;
                        if (physicsChanger.Grounded)
                        {
                            jumpLock = false;
                        }
                    }
                }

                {
                    Vector2 orthogonalOrientation = physicsChanger.Orientation.GetPerpendicular();
                    physicsChanger.Force += horizontalForce * (rightValue - leftValue) * orthogonalOrientation;
                }
            }

            physicsChanger.Update(gameTime: gameTime);
        }
    }
}
