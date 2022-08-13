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
        public List<ICollidable> ManagedCollidables { get => managedCollidables; }

        public CollisionManager()
        {
            managedCollidables = new List<ICollidable>();
        }

        public void Update(GameTime gameTime)
        {
            // Remove any destroyed collidables from the manager.
            managedCollidables.Where((x) => x.Destroyed).ToList().ForEach((x) => managedCollidables.Remove(x));

            // For each collidable in the manager, check for collisions with other collidables in the manager.
            foreach (var combination in managedCollidables.GetPermutations(count: 2))
            {
                Vector2 point0, point1, correction0, correction1;
                ICollidable[] pair = combination.ToArray();
                ICollidable collidable0 = pair[0], collidable1 = pair[1];

                if (!CheckForCollision(
                    collidable0: collidable0, collidable1: collidable1,
                    correction0: out correction0, correction1: out correction1,
                    point0: out point0, point1: out point1))
                    continue;
                
                collidable0.ServiceCollision(info: new ICollidable.Info(other: collidable1, point: point0, correction: correction0));
                collidable1.ServiceCollision(info: new ICollidable.Info(other: collidable0, point: point1, correction: correction1));
            }
        }

        private static bool CheckForCollision(ICollidable collidable0, ICollidable collidable1, out Vector2 correction0, out Vector2 correction1, out Vector2 point0, out Vector2 point1)
        {
            correction0 = Vector2.Zero;
            correction1 = Vector2.Zero;
            point0 = Vector2.Zero;
            point1 = Vector2.Zero;

            // If one of the collidables are not collidable, then there is no collision.
            if (!collidable0.Collidable || !collidable1.Collidable)
                return false;

            // Determine the bounding rectangles for this physics and the other physics.
            Rectangle bounds0 = new Rectangle(location: collidable0.Position.ToPoint(), size: collidable0.CollisionMask.Bounds.Size);
            Rectangle bounds1 = new Rectangle(location: collidable1.Position.ToPoint(), size: collidable1.CollisionMask.Bounds.Size);

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

            int[] colCounts = new int[intersection.Width];
            int[] rowCounts = new int[intersection.Height];
            
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

                        colCounts[col]++;
                        rowCounts[row]++;
                    }

            int overlapWidth = rowCounts.Max();
            int overlapHeight = colCounts.Max(); ;
            int pointMidX = (colMax + colMin) / 2;
            int pointMidY = (rowMax + rowMin) / 2;

            float adjustX0 = (collidable0.Position.X - bounds0.X);
            float adjustY0 = (collidable0.Position.Y - bounds0.Y);
            float adjustX1 = (collidable1.Position.X - bounds1.X);
            float adjustY1 = (collidable1.Position.Y - bounds1.Y);
            float correctionOffsetY0, correctionOffsetY1;
            float correctionOffsetX0, correctionOffsetX1;
            float pointY0, pointY1;
            float pointX0, pointX1;
            if (overlapHeight < overlapWidth)
            {
                correctionOffsetX0 = 0;
                correctionOffsetX1 = 0;
                pointX0 = pointMidX + intersection1.X + collidable1.Position.X;
                pointX1 = pointMidX + intersection0.X + collidable0.Position.X;
                
                if (topSum1 > bottomSum1)
                {
                    correctionOffsetY0 = -overlapHeight - adjustY0;
                    pointY0 = rowMin + intersection1.Y + collidable1.Position.Y;
                }
                else
                {
                    correctionOffsetY0 = overlapHeight - adjustY0;
                    pointY0 = rowMax + intersection1.Y + collidable1.Position.Y;
                }
                if (topSum0 > bottomSum0)
                {
                    correctionOffsetY1 = -overlapHeight - adjustY1;
                    pointY1 = rowMin + intersection0.Y + collidable0.Position.Y;
                }
                else
                {
                    correctionOffsetY1 = overlapHeight - adjustY1;
                    pointY1 = rowMax + intersection0.Y + collidable0.Position.Y;
                }
            }
            else
            {
                correctionOffsetY0 = 0;
                correctionOffsetY1 = 0;
                pointY0 = pointMidY + intersection1.Y + collidable1.Position.Y;
                pointY1 = pointMidY + intersection0.Y + collidable0.Position.Y;

                if (leftSum1 > rightSum1)
                {
                    correctionOffsetX0 = -overlapWidth - adjustX0;
                    pointX0 = colMin + intersection1.X + collidable1.Position.X;
                }
                else
                {
                    correctionOffsetX0 = overlapWidth - adjustX0;
                    pointX0 = colMax + intersection1.X + collidable1.Position.X;
                }
                if (leftSum0 > rightSum0)
                {
                    correctionOffsetX1 = -overlapWidth - adjustX1;
                    pointX1 = colMin + intersection0.X + collidable0.Position.X;
                }
                else
                {
                    correctionOffsetX1 = overlapWidth - adjustX1;
                    pointX1 = colMax + intersection0.X + collidable0.Position.X;
                }
            }

#if DEBUG
            Console.WriteLine($"Overlap Width: {overlapWidth}. Overlap Height: {overlapHeight}");
            Console.WriteLine($"topSum1: {topSum1}. bottomSum1: {bottomSum1}. leftSum1: {leftSum1}. rightSum1: {rightSum1}");
            Console.WriteLine($"topSum0: {topSum0}. bottomSum0: {bottomSum0}. leftSum0: {leftSum0}. rightSum0: {rightSum0}");
#endif

            correction0 = new Vector2(
                x: correctionOffsetX0,
                y: correctionOffsetY0);
            point0 = new Vector2(
                x: pointX0,
                y: pointY0);
            correction1 = new Vector2(
                x: correctionOffsetX1,
                y: correctionOffsetY1);
            point1 = new Vector2(
                x: pointX1,
                y: pointY1);

            return true;
        }
    }
}
