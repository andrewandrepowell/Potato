using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using MonoGame.Extended;

namespace Potato
{
    internal class CollisionManager : IUpdateable
    {
        private List<ICollidable> managedCollidables;
        public List<ICollidable> Collidables { get => managedCollidables; }

        public CollisionManager()
        {
            managedCollidables = new List<ICollidable>();
        }

        public void Update(GameTime gameTime)
        {
            managedCollidables.Where((x) => x.Destroyed).ToList().ForEach((x) => managedCollidables.Remove(x));
            foreach (var combination in managedCollidables.GetPermutations(count: 2))
            {
                Vector2 collisionPoint;
                ICollidable[] pair = combination.ToArray();
                ICollidable collidable0 = pair[0], collidable1 = pair[1];
                if (!collidable0.Collidable && !collidable1.Collidable)
                    continue;
                if (!CheckForCollision(collidable0: collidable0, collidable1: collidable1, out collisionPoint))
                    continue;
                if (collidable0.Collidable)
                {
                    Vector2 collisionNormal = DetermineCollisionNormal(collidable: collidable0, colllisionPoint: collisionPoint);
                    collidable1.ServiceCollision(new ICollidable.Info(
                        other: collidable0,
                        collisionPoint: collisionPoint,
                        collisionNormal: collisionNormal));
                }
                if (collidable1.Collidable)
                {
                    Vector2 collisionNormal = DetermineCollisionNormal(collidable: collidable1, colllisionPoint: collisionPoint);
                    collidable0.ServiceCollision(new ICollidable.Info(
                        other: collidable1,
                        collisionPoint: collisionPoint,
                        collisionNormal: collisionNormal));
                }
            }
        }

        private static bool CheckForCollision(ICollidable collidable0, ICollidable collidable1, out Vector2 colllisionPoint)
        {
            colllisionPoint = Vector2.Zero;
            RectangleF collidableBounds0 = new RectangleF(position: collidable0.Position, size: collidable0.Size);
            RectangleF collidableBounds1 = new RectangleF(position: collidable1.Position, size: collidable1.Size);

            if (!collidableBounds0.Intersects(collidableBounds1))  
                return false;

            RectangleF intersection = collidableBounds0.Intersection(collidableBounds1);
            RectangleF collidable0Intersection = new RectangleF(
                x: intersection.X - collidableBounds0.X,
                y: intersection.Y - collidableBounds0.Y,
                width: intersection.Width,
                height: intersection.Height);
            RectangleF collidable1Intersection = new RectangleF(
                x: intersection.X - collidableBounds1.X,
                y: intersection.Y - collidableBounds1.Y,
                width: intersection.Width,
                height: intersection.Height);
            
            int colorLength = (int)collidable0Intersection.Width * (int)collidable0Intersection.Height;
            Color[] colorData0 = new Color[colorLength];
            Color[] colorData1 = new Color[colorLength];

            collidable0.CollisionMask.GetData(
                level: 0,
                rect: collidable0Intersection.ToRectangle(),
                data: colorData0,
                startIndex: 0,
                elementCount: colorLength);
            collidable1.CollisionMask.GetData(
                level: 0,
                rect: collidable1Intersection.ToRectangle(),
                data: colorData1,
                startIndex: 0,
                elementCount: colorLength);

            bool[] collisionMask = colorData0.Zip(colorData1, (c0, c1) => c0.A != 0 && c1.A != 0).ToArray();
            if (!collisionMask.Contains(true))
                return false;
            
            

            return true;
        }

        private static Vector2 DetermineCollisionNormal(ICollidable collidable, Vector2 colllisionPoint)
        {
            return Vector2.Zero;
        }
    }
}
