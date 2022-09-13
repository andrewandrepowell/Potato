using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using Potato.Element.Character;
using Potato.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Potato.World.Room.EngineEditor
{
    internal class TestPhysics0 : IComponent, IPausible, IControllable
    {
        private SimpleLevel simpleLevel;
        private Player playerPhysical;
        private bool softPaused;
        private bool hardPaused;
        private IController controller;
        public bool SoftPaused => softPaused;
        public bool HardPaused => hardPaused;
        public IController Controller 
        { 
            get => controller; 
            set
            {
                controller = value;
                playerPhysical.Controller = value;
            }
        }

        public TestPhysics0()
        {
            controller = null;
            softPaused = false;
            hardPaused = false;
            simpleLevel = new SimpleLevel();
            simpleLevel.Load(Saver.Load<SimpleLevelSave>("test_level_0.level"));
            simpleLevel.Open();
            foreach (ICollidable collidable in simpleLevel.Elements.OfType<ICollidable>())
                simpleLevel.Collision.ManagedCollidables.Add(collidable);
            playerPhysical = simpleLevel.Elements.OfType<Player>().First();
            playerPhysical.Mass = 1f;
            playerPhysical.MaxSpeed = 1000f;
            playerPhysical.Friction = 80;
            playerPhysical.Gravity = new Vector2(x: 0, y: 1500);
            playerPhysical.Bounce = .75f;
            playerPhysical.VerticalForce = 4000;
        }
        
        public void Draw(Matrix? transformMatrix = null)
        {
            simpleLevel.Draw(transformMatrix: transformMatrix);
        }

        public void Update(GameTime gameTime)
        {
            MouseStateExtended mouseState = MouseExtended.GetState();

            if (mouseState.IsButtonDown(button: MouseButton.Left))
                playerPhysical.Force = 3000 * Vector2.Normalize(mouseState.Position.ToVector2() - (playerPhysical.Position + playerPhysical.CollisionMask.Bounds.Center.ToVector2()));
            else
                playerPhysical.Force = Vector2.Zero;
            if (mouseState.WasButtonJustDown(button: MouseButton.Right))
            {
                playerPhysical.Force = Vector2.Zero;
                playerPhysical.Velocity = Vector2.Zero;
                playerPhysical.Position = mouseState.Position.ToVector2() - playerPhysical.CollisionMask.Bounds.Center.ToVector2();
            }

            simpleLevel.Update(gameTime: gameTime);
        }

        public void SoftPause()
        {
            if (softPaused)
                return;
            softPaused = true;
            simpleLevel.SoftPause();
        }

        public void SoftResume()
        {
            if (!softPaused)
                return;
            softPaused = false;
            simpleLevel.SoftResume();
        }

        public void HardPause()
        {
            if (hardPaused)
                return;
            hardPaused = true;
            simpleLevel.HardPause();
        }

        public void HardResume()
        {
            if (!hardPaused)
                return;
            hardPaused = false;
            simpleLevel.HardResume();
        }
    }
}
