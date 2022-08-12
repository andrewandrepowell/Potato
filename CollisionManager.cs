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
            Vector2 penetration0 = Vector2.Zero;
            Vector2 penetration1 = Vector2.Zero;
            
            // Determine the bounding rectangles for this physics and the other physics.
            Rectangle bounds0 = new Rectangle(location: collidable0.Position.ToPoint(), size: (Point)collidable0.Size);
            Rectangle bounds1 = new Rectangle(location: collidable1.Position.ToPoint(), size: (Point)collidable1.Size);

            // Determine whether the bounding rectangles intersect.
            // If they don't, don't bother going further.
            if (!bounds0.Intersects(bounds1))  
                return false;

            // Find the bounding rectangle that represents the overlap between 
            // the two bounding rectangles. 
            Rectangle intersection = Rectangle.Intersect(bounds0, bounds1);

            // Find the equivalent rectangles that represent the intersection relative
            // to the two mask bounding rectangles.
            // These are needed to extract the corresponding pixel data from the masks.
            Rectangle intersection0 = new Rectangle(
                x: intersection.X - bounds0.X,
                y: intersection.Y - bounds0.Y,
                width: intersection.Width,
                height: intersection.Height);
            Rectangle intersection1 = new Rectangle(
                x: intersection.X - bounds1.X,
                y: intersection.Y - bounds1.Y,
                width: intersection.Width,
                height: intersection.Height);

            // Get the pixel data for the masks.
            int colorLength = intersection.Width * intersection.Height;
            Color[] colorData0 = new Color[colorLength];
            Color[] colorData1 = new Color[colorLength];

            collidable0.CollisionMask.GetData(
                level: 0,
                rect: intersection0,
                data: colorData0,
                startIndex: 0,
                elementCount: colorLength);
            collidable1.CollisionMask.GetData(
                level: 0,
                rect: intersection1,
                data: colorData1,
                startIndex: 0,
                elementCount: colorLength);

            // Get the mask that represents all points of intersection.
            bool[] collisionMask = colorData0.Zip(colorData1, (c0, c1) => c0.A != 0 && c1.A != 0).ToArray();
            if (!collisionMask.Contains(true))
                return false;

            int midHeight0 = bounds0.Height / 2;
            int midWidth0 = bounds0.Width / 2;
            int midHeight1 = bounds1.Height / 2;
            int midWidth1 = bounds1.Width / 2;
            int topSum0 = 0, topSum1 = 0;
            int bottomSum0 = 0, bottomSum1 = 0;
            int leftSum0 = 0, leftSum1 = 0;
            int rightSum0 = 0, rightSum1 = 0;
            
            int rowMin = intersection.Height - 1;
            int rowMax = 0;
            int colMin = intersection.Width - 1;
            int colMax = 0;
            
            for (int row = 0; row < intersection.Height; row++)
                for (int col = 0; col < intersection.Width; col++)
                    if (collisionMask[col + row * intersection.Width])
                    {
                        if (row + intersection0.Y < midHeight0)
                            topSum0++;
                        else
                            bottomSum0++;
                        if (col + intersection0.X < midWidth0)
                            leftSum0++;
                        else
                            rightSum0++;

                        if (row + intersection1.Y < midHeight1)
                            topSum1++;
                        else
                            bottomSum1++;
                        if (col + intersection1.X < midWidth1)
                            leftSum1++;
                        else
                            rightSum1++;

                        if (row < rowMin)
                            rowMin = row;
                        if (row > rowMax)
                            rowMax = row;
                        if (col < colMin)
                            colMin = col;
                        if (col > colMax)
                            colMax = col;
                    }

            int penetrateOffsetY0;
            if (topSum1 > bottomSum1)
                penetrateOffsetY0 = rowMax + 1;
            else
                penetrateOffsetY0 = rowMin - intersection1.Y;

            int penetrateOffsetX0;
            if (leftSum1 > rightSum1)
                penetrateOffsetX0 = colMax + 1;
            else
                penetrateOffsetX0 = colMin - intersection1.X;

            int penetrateOffsetY1;
            if (topSum0 > bottomSum0)
                penetrateOffsetY1 = rowMax + 1;
            else
                penetrateOffsetY1 = rowMin - intersection0.Y;

            int penetrateOffsetX1;
            if (leftSum0 > rightSum0)
                penetrateOffsetX1 = colMax + 1;
            else
                penetrateOffsetX1 = colMin - intersection0.X;

            penetration0 = new Vector2(
                x: penetrateOffsetX0 - (collidable0.Position.X - bounds0.X),
                y: penetrateOffsetY0 - (collidable0.Position.Y - bounds0.Y));
            penetration1 = new Vector2(
                x: penetrateOffsetX1 - (collidable1.Position.X - bounds1.X),
                y: penetrateOffsetY1 - (collidable1.Position.Y - bounds1.Y));


            return true;
        }

        private static Vector2 DetermineCollisionNormal(ICollidable collidable, Vector2 colllisionPoint)
        {
            return Vector2.Zero;
        }
    }
}
