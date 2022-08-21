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
        private Vector2 velocity;
        private Vector2 acceleration;
        private Vector2 force;
        private bool collidable;
        private Texture2D collisionMask;
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
        public bool Collidable { get => collidable; set => collidable = value; }

        public Texture2D CollisionMask { get => collisionMask; set => collisionMask = value; }
        public IList<Vector2> CollisionVertices { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Vector2 Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool Destroyed => throw new NotImplementedException();



        public PhysicsChanger()
        {
            
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void ServiceCollision(ICollidable.Info info)
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
