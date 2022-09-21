using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
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
        private bool facingRight;
        private bool tryingToMoveHorizontally;
        private float horizontalForce;
        private float verticalForce;
        private float groundStick;
        private float jumpTimer;
        private float spriteDrawRotation;
        private const float jumpTimerThreshold = 0.1f;
        private const float groundMultipler = 250f;
        private Vector2 jumpOrientation;
        private Vector2 spriteDrawScale;
        private Vector2 spriteDrawOffset;
        private AnimatedSprite runningSprite;
        private AnimatedSprite idleSprite;
        private AnimatedSprite currentSprite;
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
            jumpTimer = 0.0f;
            jumpOrientation = Vector2.Zero;
            facingRight = true;
            tryingToMoveHorizontally = false;

            groundStick = 0;
            horizontalForce = 3000;
            verticalForce = 14000f;
            physicsChanger.Mass = 1f;
            physicsChanger.MaxSpeed = 2000f;
            physicsChanger.Friction = 1000;
            physicsChanger.Gravity = new Vector2(x: 0, y: 3000);
            physicsChanger.Bounce = .75f;
            physicsChanger.Stick = groundStick;

            {
                SpriteSheet spriteSheet;
                spriteSheet = Potato.Game.Content.Load<SpriteSheet>("protagonist_running.sf", new JsonContentLoader());
                runningSprite = new AnimatedSprite(spriteSheet);
                spriteSheet = Potato.Game.Content.Load<SpriteSheet>("protagonist_idle.sf", new JsonContentLoader());
                idleSprite = new AnimatedSprite(spriteSheet);
                idleSprite.Play("idle");
                currentSprite = idleSprite;
            }

            spriteDrawScale = new Vector2(x: 0.175f, y: 0.175f);
            spriteDrawOffset = collisionMask.Bounds.Size.ToVector2() / 2;
            spriteDrawRotation = 0.0f;
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
            Vector2 drawPosition = physicsChanger.Position
                    .ToPoint()
                    .ToVector2();
            //spriteBatch.Draw(
            //    texture: collisionMask, 
            //    position: drawPosition, 
            //    color: Color.White);
            spriteBatch.Draw(
                sprite: currentSprite, 
                position: drawPosition + spriteDrawOffset, 
                rotation: spriteDrawRotation, 
                scale: spriteDrawScale);
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
            // Don't update if the player is destroyed or hard paused.
            if (destroyed || hardPaused)
                return;

            float timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Implement player movement.
            if (controller != null && !softPaused)
            {
                float upValue = controller.UpHeld();
                float leftValue = controller.LeftHeld();
                float rightValue = controller.RightHeld();
                float movementValue = rightValue - leftValue;

                // Set the direction the character is facing.
                if (movementValue > 0)
                    facingRight = true;
                else if (movementValue < 0)
                    facingRight = false;

                // Determine whether or not player is trying to move horizontally.
                if (Math.Abs(movementValue) > 0)
                    tryingToMoveHorizontally = true;
                else
                    tryingToMoveHorizontally = false;

                // The following implements the logic for the jump mechanic.
                // The idea of the jump mechanic is if the player is grounded, hasn't already jumped, and up is held on the controller,
                //   the jump state is activated for certain amount of time.
                // On activation, the current orientation is recorded to determine the direction in which the jump will occur,
                //   and stick is removed .
                // If in the jump state is activated and the up is still held, a force is applied in the direction of the recorded orientation.
                // Jump state ends if its timer runs out or up is no longer held. 
                // Jump state cannot be reentered until the player is grounded.
                // If up is not being held and the player is grounded, stick is applied once the player starts trying to move.
                {
                    if (upValue > 0)
                    {
                        if (jumpTimer > 0)
                        {
                            jumpTimer -= timeElapsed;
                            physicsChanger.Force += verticalForce * jumpOrientation * upValue;
                        }
                        else if (physicsChanger.Grounded && !jumpLock)
                        {
                            physicsChanger.Stick = 0;
                            jumpTimer = jumpTimerThreshold;
                            jumpOrientation = physicsChanger.Orientation;
                            jumpLock = true;
                        }
                    }
                    else
                    {
                        jumpTimer = 0;
                        if (physicsChanger.Grounded)
                        {
                            jumpLock = false;
                            physicsChanger.Stick = Math.Abs(movementValue) * groundMultipler;
                        }
                    }
                }

                // If the player tries to move, a force is applied in the horizontal direction in accordance to the
                //   players movement.
                {
                    Vector2 orthogonalOrientation = physicsChanger.Orientation.GetPerpendicular();
                    physicsChanger.Force += horizontalForce * movementValue * orthogonalOrientation;
                }
            }

            // Update the media.
            {
                currentSprite.Effect = (facingRight) ? SpriteEffects .None : SpriteEffects.FlipHorizontally;

                spriteDrawRotation = (float)Math.Atan2(y: physicsChanger.Orientation.Y, x: physicsChanger.Orientation.X) + MathHelper.PiOver2;

                if (tryingToMoveHorizontally)
                {
                    if (currentSprite != runningSprite)
                    {
                        runningSprite.Play("running");
                        currentSprite = runningSprite;
                    }
                }
                else 
                {
                    if (currentSprite != idleSprite)
                    {
                        idleSprite.Play("idle");
                        currentSprite = idleSprite;
                    }
                }

                currentSprite.Update(gameTime: gameTime);
            }

            // Update the other updateables associated with the player.
            physicsChanger.Update(gameTime: gameTime);
        }
    }
}
