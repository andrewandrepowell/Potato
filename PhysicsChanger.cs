using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato
{
    internal class PhysicsChanger : IPhysical, IUpdateable
    {
        private float mass;
        private float maxSpeed;
        private float friction;
        private float otherFriction;
        private Vector2 position;
        private Vector2 velocity;
        private Vector2 acceleration;
        private Vector2 force;
        private Vector2 orientation;
        private Vector2 defaultOrientation;
        private Vector2 gravity;
        private bool destroyed;
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
        }

        public void Update(GameTime gameTime)
        {
            if (destroyed)
                return;

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            velocity += (acceleration + gravity) * elapsedTime;

            float speed = velocity.Length();
            if (speed > 0)
            {
                Vector2 direction = Vector2.Normalize(velocity);
                speed -= (friction + otherFriction) * elapsedTime;
                speed = Math.Clamp(speed, 0, maxSpeed);

                velocity = speed * direction;
            }
            

            position += velocity * elapsedTime;
        }
    }
}
