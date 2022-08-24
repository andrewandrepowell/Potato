using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato
{
    internal class PhysicsChanger : IPhysical, IUpdateable
    {
        private const float orientationGroundedThreshold = 0.25f;
        private const float groundedTimerThreshold = 0.1f;
        private float mass;
        private float maxSpeed;
        private float friction;
        private float otherFriction;
        private float groundedTimer;
        private Vector2 position;
        private Vector2 velocity;
        private Vector2 acceleration;
        private Vector2 force;
        private Vector2 orientation;
        private Vector2 defaultOrientation;
        private Vector2 gravity;
        private bool destroyed;
        private bool grounded => groundedTimer > 0;
        public bool Grounded
        {
            get => grounded;
            set => groundedTimer = (value) ? groundedTimerThreshold : 0;
        }
        public float Mass { get => mass; set => mass = value; }
        public Vector2 Velocity { get => velocity; set => velocity = value; }
        public float Friction { get => friction; set => friction = value; }
        public Vector2 Acceleration 
        { 
            get => acceleration; 
            set
            {
                acceleration = value;
                force = value * mass;
            }
        }
        public Vector2 Gravity
        {
            get => gravity;
            set
            {
                if (value != Vector2.Zero)
                    defaultOrientation = -Vector2.Normalize(value);
                else
                    defaultOrientation = Vector2.Zero;
                orientation = defaultOrientation;
                gravity = value;
            }
        }
        public Vector2 Force 
        { 
            get => force; 
            set
            {
                force = value;
                acceleration = value / mass;
            }
        }
        public float MaxSpeed
        {
            get => maxSpeed;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Cannot have negative speed.");
                maxSpeed = value;
            }
        }
        public bool Collidable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Texture2D CollisionMask { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IList<Vector2> CollisionVertices { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Vector2 Position { get => position; set => position = value; }

        public bool Destroyed => throw new NotImplementedException();



        public PhysicsChanger()
        {
            mass = 0;
            maxSpeed = 0;
            friction = 0;
            otherFriction = 0;
            groundedTimer = 0;
            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
            force = Vector2.Zero;
            orientation = Vector2.Zero;
            defaultOrientation = Vector2.Zero;
            gravity = Vector2.Zero;
            destroyed = false;
        }

        public void Dispose() => destroyed = true;

        public void ServiceCollision(ICollidable.Info info)
        {
            if (destroyed)
                return;

            if (orientation != Vector2.Zero && info.Normal != Vector2.Zero)
            {
                float dotProduct = Vector2.Dot(orientation, info.Normal);
                if (dotProduct > orientationGroundedThreshold)
                {
                    groundedTimer = groundedTimerThreshold;
                    orientation = info.Normal;
                    if (info.Other is IPhysical otherPhysical)
                    {
                        otherFriction = otherPhysical.Friction;
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (destroyed)
                return;

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Determine the velocity. Total acceleration is both acceleration and gravity.
            velocity += (acceleration + gravity) * elapsedTime;

            // Apply corrections to speed.
            float speed = velocity.Length();
            if (speed > 0)
            {
                Vector2 direction = Vector2.Normalize(velocity);
                speed -= (friction + otherFriction) * elapsedTime;
                speed = Math.Clamp(speed, 0, maxSpeed);
                velocity = speed * direction;
            }

            // Apply grounded corrections.
            if (grounded)
            {
                Vector2 orthogonal = new Vector2(x: -orientation.Y, y: orientation.X);
                float orientationScalar = Vector2.Dot(orientation, velocity);
                float orthognalScalar = Vector2.Dot(orthogonal, velocity);
                velocity = orthognalScalar * orthogonal;
            }
            
            // Determine the new position based on the current velocity.
            position += velocity * elapsedTime;

            // Configure grounded state.
            if (grounded)
                groundedTimer -= elapsedTime;
            else
            {
                otherFriction = 0;
                orientation = defaultOrientation;
            }
        }
    }
}
