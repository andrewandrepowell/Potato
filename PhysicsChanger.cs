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
        private const float touchedTimerThreshold = 0.1f;
        private const float squashBounceThreshold = 50;
        private float mass;
        private float maxSpeed;
        private float friction;
        private float otherFriction;
        private float groundedTimer;
        private float touchedTimer;
        private float bounce;
        private Vector2 position;
        private Vector2 velocity;
        private Vector2 acceleration;
        private Vector2 force;
        private Vector2 orientation;
        private Vector2 defaultOrientation;
        private Vector2 gravity;
        private Vector2 touchedOrientation;
        private bool destroyed;
        private bool hardPaused;
        private bool grounded => groundedTimer > 0;
        private bool touched => touchedTimer > 0;
        public bool Grounded
        {
            get => grounded;
            set => groundedTimer = (value) ? groundedTimerThreshold : 0;
        }
        public float Mass { get => mass; set => mass = value; }
        public float Friction { get => friction; set => friction = value; }
        public float Bounce { get => bounce; set => bounce = value; }
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
        public bool Destroyed => destroyed;
        public bool SoftPaused => false;
        public bool HardPaused => hardPaused;

        public PhysicsChanger()
        {
            mass = 0;
            maxSpeed = 0;
            friction = 0;
            otherFriction = 0;
            groundedTimer = 0;
            touchedTimer = 0;
            bounce = 0;
            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
            force = Vector2.Zero;
            orientation = Vector2.Zero;
            defaultOrientation = Vector2.Zero;
            gravity = Vector2.Zero;
            touchedOrientation = Vector2.Zero;
            destroyed = false;
        }

        public void Dispose() => destroyed = true;

        public void ServiceCollision(ICollidable.Info info)
        {
            // Don't update the state of the physical if destroyed or paused.
            if (destroyed || hardPaused)
                return;

            // Only check for touched and grounded states if the current physical has an orientation,
            // dictated by gravity, and if the other collidable has a normal collision vector, implying it has a surfacec.
            if (orientation != Vector2.Zero && info.Normal != Vector2.Zero)
            {
                // The current physical is considered in a touched state upon collision with an other collidable.
                {
                    // A timer is implemented so that the touched state will last for a split second longer,
                    // even after the current physical isn't collided with another physical.
                    touchedTimer = touchedTimerThreshold;

                    // The touched orientation is recorded as the collision normal vector.
                    touchedOrientation = info.Normal;

                    // Further operations are needed if the other collidable is a physical.
                    if (info.Other is IPhysical otherPhysical)
                    {
                        // The velocity of the current physical is updated to account for the bounce mechanic.
                        Vector2 orthogonal = touchedOrientation.GetPerpendicular();
                        float touchedOrientationScalar = Vector2.Dot(touchedOrientation, velocity);
                        float orthognalScalar = Vector2.Dot(orthogonal, velocity);
                        float bounceScalar = -(otherPhysical.Bounce + bounce) * touchedOrientationScalar;
                        if (Math.Abs(bounceScalar) < squashBounceThreshold)
                            bounceScalar = 0;
                        velocity = bounceScalar * touchedOrientation + orthognalScalar * orthogonal;
                    }
                }

                // If the orientation of the current physical is similar enough to the collision normal vector,
                // then grounded state is reached.
                float dotProduct = Vector2.Dot(orientation, info.Normal);
                if (dotProduct > orientationGroundedThreshold)
                {
                    // Similar to the touched state, a timer is used to keep
                    // the current physical in grounded state a bit longer.
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
            // Don't update the state of the physical if destroyed or paused.
            if (destroyed || hardPaused)
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

            // Apply touched corrections to velocity. 
            if (touched)
            {
                Vector2 orthogonal = touchedOrientation.GetPerpendicular();
                float touchedOrientationScalar = Vector2.Dot(touchedOrientation, velocity);
                float orthognalScalar = Vector2.Dot(orthogonal, velocity);
                velocity = Math.Max(0, touchedOrientationScalar) * touchedOrientation + orthognalScalar * orthogonal;
            }
            
            // Determine the new position based on the current velocity.
            position += velocity * elapsedTime;

            // Touched state is dependent on a timer. 
            if (touched)
                touchedTimer -= elapsedTime;
            // Reset associated information when touched state isn't active.
            else
            {
                touchedOrientation = Vector2.Zero;
            }

            // Grounded state is dependent on a timer.
            if (grounded)
                groundedTimer -= elapsedTime;
            // Reset associated information when grounded state isn't active.
            else
            {
                otherFriction = 0;
                orientation = defaultOrientation;
            }
        }
        public void SoftPause() { }

        public void SoftResume() { }

        public void HardPause() => hardPaused = true;

        public void HardResume() => hardPaused = false;
    }
}
