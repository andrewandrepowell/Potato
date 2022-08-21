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
        private Vector2 position;
        private Vector2 velocity;
        private Vector2 acceleration;
        private Vector2 force;
        private bool destroyed;
        public float Mass { get => mass; set => mass = value; }
        public Vector2 Velocity { get => velocity; set => velocity = value; }
        public Vector2 Acceleration 
        { 
            get => acceleration; 
            set
            {
                acceleration = value;
                force = value * mass;
            }
        }
        public Vector2 Force 
        { 
            get => force; 
            set
            {
                if (mass == 0)
                    throw new DivideByZeroException("Mass cannot be zero when setting force");
                force = value;
                acceleration = value / mass;
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
            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
            force = Vector2.Zero;
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
            velocity += acceleration * elapsedTime;
            position += velocity * elapsedTime;
        }
    }
}
