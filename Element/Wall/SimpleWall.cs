using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato.Element.Wall
{
    internal class SimpleWall : IWallable, IIdentifiable
    {
        private static Color startColor = Color.Yellow;
        private static Color includeColor = Color.Red;
        private static Color excludeColor = new Color(r: 0, g: 255, b: 0, alpha: 255);
        private bool destroyed;
        private bool collidable;
        private Texture2D collisionMask;
        private Texture2D displayTexture;
        private List<Vector2> collisionVertices;
        private Vector2 position;
        private string identifier;
        public Texture2D DisplayTexture { get => displayTexture; set => throw new NotImplementedException(); }
        public bool Collidable { get => collidable; set => collidable = value; }
        public Texture2D CollisionMask { get => collisionMask; set => throw new NotImplementedException(); }
        public IList<Vector2> CollisionVertices { get => collisionVertices; set => throw new NotImplementedException(); }
        public Size2 Size { get => collisionMask.Bounds.Size; set => throw new NotImplementedException(); }
        public Vector2 Position { get => position; set => position = value; }
        public bool Destroyed => destroyed;
        public string Identifier { get => identifier; set => throw new NotImplementedException(); }

        public SimpleWall(Texture2D collisionMask, Texture2D displayTexture, string identifier)
        {
            destroyed = false;
            collidable = true;
            this.collisionMask = collisionMask;
            this.displayTexture = displayTexture;
            this.identifier = identifier;
            collisionVertices = CollisionManager.GetVertices(
                mask: collisionMask,
                startColor: startColor,
                includeColor: includeColor,
                excludeColor: excludeColor);
        }

        public void Dispose() => destroyed = true;

        public void Draw(Matrix? transformMatrix = null)
        {
            if (destroyed)
                return;
            SpriteBatch spriteBatch = Potato.SpriteBatch;
            spriteBatch.Begin(transformMatrix: transformMatrix);
            spriteBatch.Draw(
                texture: displayTexture,
                position: position,
                color: Color.White);
            spriteBatch.End();
        }

        public void ServiceCollision(ICollidable.Info info)
        {
            if (destroyed)
                return;
        }
    }
}
