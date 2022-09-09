using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato.Character
{
    internal class Player : ICharacterizable, IControllable, IIdentifiable
    {
        private List<IProjectile> projectiles;
        private PhysicsChanger physicsChanger;
        private Texture2D collisionMask;
        private List<Vector2> collisionVertices;
        private IController controller;
        private string identifier;
        private bool collidable;
        private bool destroyed;
        private bool softPaused;
        private bool hardPaused;
        public IList<IProjectile> Projectiles { get => projectiles; set => throw new NotImplementedException(); }
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
        public Vector2 Position { get => physicsChanger.Position; set => physicsChanger.Position = value; }

        public Player()
        {
            projectiles = new List<IProjectile>();
            physicsChanger = new PhysicsChanger();
            collisionMask = Potato.Game.Content.Load<Texture2D>("characters/mask_player");
            collisionVertices = new List<Vector2>();
            controller = null;
            identifier = "";
            collidable = false;
            destroyed = false;
            softPaused = false;
            hardPaused = false;
        }

        public bool Destroyed => destroyed;

        public bool SoftPaused => softPaused;

        public bool HardPaused => hardPaused;

        public IController Controller { get => controller; set => controller = value; }

        public string Identifier { get => identifier; set => identifier = value; }

        public void Dispose()
        {
            destroyed = true;
            foreach (IDestroyable destroyable in projectiles)
                destroyable.Dispose();
            physicsChanger.Dispose();
        }

        public void Draw(Matrix? transformMatrix = null)
        {

            foreach (IDrawable drawable in projectiles)
                drawable.Draw(transformMatrix: transformMatrix);
        }

        public void HardPause()
        {
            hardPaused = true;
            foreach (IPausible pausible in projectiles)
                pausible.HardPause();
            physicsChanger.HardPause();
        }

        public void HardResume()
        {
            hardPaused = false;
            foreach (IPausible pausible in projectiles)
                pausible.HardResume();
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
            foreach (IPausible pausible in projectiles)
                pausible.SoftPause();
            physicsChanger.SoftPause();
        }

        public void SoftResume()
        {
            softPaused = false;
            foreach (IPausible pausible in projectiles)
                pausible.SoftResume();
            physicsChanger.SoftResume();
        }

        public void Update(GameTime gameTime)
        {
            foreach (IUpdateable updateable in projectiles)
                updateable.Update(gameTime: gameTime);
            physicsChanger.Update(gameTime: gameTime);
        }
    }
}
