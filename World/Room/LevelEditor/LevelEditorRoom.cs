using Microsoft.Xna.Framework;
using MonoGame.Extended.Input;
using Potato.Room;
using Potato.Room.Wall;
using Potato.World.Menu;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;


namespace Potato.World.Room.LevelEditor
{
    internal class LevelEditorRoom : IRoom
    {
        private const int blockWidth = 127;
        private const string saveExtension = "level";
        private RoomStateChanger roomStateChanger;
        private LevelEditorMenu levelEditorMenu;
        private SimpleLevel simpleLevel;
        private IWallable wallToPlace;
        private string wallToPlaceIdentifier;
        public IController Controller { get => levelEditorMenu.Controller; set => levelEditorMenu.Controller = value; }
        public IOpenable.OpenStates OpenState => roomStateChanger.OpenState;
        public ISelectable TitleSelect => levelEditorMenu.TitleSelect;

        public LevelEditorRoom()
        {
            wallToPlaceIdentifier = "";
            wallToPlace = null;
            simpleLevel = new SimpleLevel();
            roomStateChanger = new RoomStateChanger();
            levelEditorMenu = new LevelEditorMenu();
        }

        public void Close()
        {
            simpleLevel.Close();
            levelEditorMenu.Close();
            roomStateChanger.Close();
        }

        public void Draw(Matrix? transformMatrix = null)
        {
            if (wallToPlace != null)
                wallToPlace.Draw(transformMatrix: transformMatrix);
            simpleLevel.Draw(transformMatrix: transformMatrix);
            levelEditorMenu.Draw(transformMatrix: transformMatrix);
            roomStateChanger.Draw(transformMatrix: transformMatrix);
        }

        private void Reset()
        {
            wallToPlaceIdentifier = "";
            if (wallToPlace != null)
            {
                wallToPlace.Dispose();
                wallToPlace = null;
            }
            simpleLevel.Walls.Clear();
        }

        public void HardReset()
        {
            Reset();
            simpleLevel.HardReset();
            levelEditorMenu.HardReset();
            roomStateChanger.HardReset();
        }

        public void Open()
        {
            simpleLevel.Open();
            levelEditorMenu.Open();
            roomStateChanger.Open();
        }

        public void SoftReset()
        {
            Reset();
            simpleLevel.SoftReset();
            levelEditorMenu.SoftReset();
            roomStateChanger.SoftReset();
        }

        public void Update(GameTime gameTime)
        {
            MouseStateExtended mouseState = MouseExtended.GetState();

            // Detect if wall-to-place identifier changes.
            if (wallToPlaceIdentifier != levelEditorMenu.WallToPlaceIdentifier)
            {
                wallToPlaceIdentifier = levelEditorMenu.WallToPlaceIdentifier;

                // If the identifier exists, set the wall-to-place to the new wall.
                if (WallManager.Identifiers.Contains(wallToPlaceIdentifier))
                {
                    if (wallToPlace != null)
                        wallToPlace.Dispose();
                    wallToPlace = WallManager.GetWall(identifier: wallToPlaceIdentifier);
                }
                else
                {
                    wallToPlace.Dispose();
                    wallToPlace = null;
                }
            }
            
            // Perform operataions if there's a wall-to-place.
            if (wallToPlace != null)
            {
                // Set the position of the block-to-place.
                int blockX = mouseState.Position.X / blockWidth;
                int blockY = mouseState.Position.Y / blockWidth;
                wallToPlace.Position = new Vector2(
                    x: blockX * blockWidth,
                    y: blockY * blockWidth);

                // Try to place the block if left mouse button is pressed.
                if (mouseState.WasButtonJustDown(MouseButton.Left))
                {
                    // Only place the wall-to-place if it doesn't intersect within the bounding rectangle of all placed walls.
                    Rectangle mouseBounds = new Rectangle(location: mouseState.Position, size: new Point(x: 1, y: 1));
                    if (simpleLevel.Walls
                        .Select((x)=> new Rectangle(
                            location: x.Position.ToPoint(), 
                            size: x.CollisionMask.Bounds.Size))
                        .All((x)=> !x.Intersects(mouseBounds)))
                    {
                        simpleLevel.Walls.Add(wallToPlace);
                        wallToPlace = WallManager.GetWall(identifier: wallToPlaceIdentifier);
                    }
                }
            }

            // Save the level if the save menu is active, the level editor menu has the controller, 
            // the activate button is pressed, and the save string is greater than 0 length.
            if (levelEditorMenu.SaveMenuActive && levelEditorMenu.Controller != null &&
                levelEditorMenu.Controller.ActivatePressed() && levelEditorMenu.SaveString.Length > 0)
            {
                SimpleLevelSave save = simpleLevel.Save();
                Saver.Save(fileName: $"{levelEditorMenu.SaveString}.{saveExtension}", obj: save);
            }

            // Loading the level works in a similar way to saving, but the reverse operation.
            if (levelEditorMenu.LoadMenuActive && levelEditorMenu.Controller != null &&
                levelEditorMenu.Controller.ActivatePressed() && levelEditorMenu.LoadString.Length > 0)
            {
                SimpleLevelSave save = Saver.Load<SimpleLevelSave>($"{levelEditorMenu.LoadString}.{saveExtension}");
                simpleLevel.Load(save);
            }

            // Update the other objects.
            simpleLevel.Update(gameTime: gameTime);
            levelEditorMenu.Update(gameTime: gameTime);
            roomStateChanger.Update(gameTime: gameTime);
        }
    }
}
