using Microsoft.Xna.Framework;
using MonoGame.Extended.Input;
using Potato.Room;
using Potato.Room.Wall;
using Potato.World.Menu;
using System;
using System.Collections.Generic;
using System.Text;


namespace Potato.World.Room.LevelEditor
{
    internal class LevelEditorRoom : IRoom
    {
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
            {
                wallToPlace.Draw(transformMatrix: transformMatrix);
            }
            simpleLevel.Draw(transformMatrix: transformMatrix);
            levelEditorMenu.Draw(transformMatrix: transformMatrix);
            roomStateChanger.Draw(transformMatrix: transformMatrix);
        }

        public void HardReset()
        {
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
            wallToPlaceIdentifier = "";
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
                wallToPlace.Position = mouseState.Position.ToVector2();
            }

            // Update the other objects.
            simpleLevel.Update(gameTime: gameTime);
            levelEditorMenu.Update(gameTime: gameTime);
            roomStateChanger.Update(gameTime: gameTime);
        }
    }
}
