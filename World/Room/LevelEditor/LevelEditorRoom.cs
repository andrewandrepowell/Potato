using Microsoft.Xna.Framework;
using MonoGame.Extended.Input;
using Potato.Room;
using Potato.World.Menu;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Potato.Element;

namespace Potato.World.Room.LevelEditor
{
    internal class LevelEditorRoom : IRoom
    {
        private const int blockWidth = 128;
        private const string saveExtension = "level";
        private const float dragCameraSpeed = 32f;
        private RoomStateChanger roomStateChanger;
        private LevelEditorMenu levelEditorMenu;
        private SimpleLevel simpleLevel;
        private IElement elementToPlace;
        private string elementToPlaceIdentifier;
        private Vector2 dragCameraStart;
        public IController Controller { get => levelEditorMenu.Controller; set => levelEditorMenu.Controller = value; }
        public IOpenable.OpenStates OpenState => roomStateChanger.OpenState;
        public ISelectable TitleSelect => levelEditorMenu.TitleSelect;

        public LevelEditorRoom()
        {
            dragCameraStart = Vector2.Zero;
            elementToPlaceIdentifier = "";
            elementToPlace = null;
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
            transformMatrix = simpleLevel.Camera.GetViewMatrix();
            if (elementToPlace != null && elementToPlace is IDrawable drawable)
                drawable.Draw(transformMatrix: transformMatrix);
            simpleLevel.Draw(transformMatrix: transformMatrix);
            levelEditorMenu.Draw(transformMatrix: null);
            roomStateChanger.Draw(transformMatrix: null);
        }

        private void Reset()
        {
            elementToPlaceIdentifier = "";
            if (elementToPlace != null)
            {
                if (elementToPlace is IDestroyable destroyable)
                    destroyable.Dispose();
                elementToPlace = null;
            }
            simpleLevel.Elements.Clear();
            simpleLevel.Camera.Position = Vector2.Zero;
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
            float timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Operations if there's no element-to-place. 
            if (elementToPlace == null)
            {
                // Detect if element-to-place identifier changes.
                if (elementToPlaceIdentifier != levelEditorMenu.ElementToPlaceIdentifier)
                {
                    elementToPlaceIdentifier = levelEditorMenu.ElementToPlaceIdentifier;

                    // If the identifier exists, set the element-to-place to the new element.
                    elementToPlace = ElementManager.GetElement(elementToPlaceIdentifier);
                }

                // Try to pick up an element.
                if (mouseState.WasButtonJustDown(MouseButton.Left))
                {
                    Point mousePosition = simpleLevel.Camera.Position.ToPoint() + mouseState.Position;
                    Rectangle mouseBounds = new Rectangle(location: mousePosition, size: new Point(x: 1, y: 1));
                    try
                    {
                        elementToPlace = simpleLevel.Elements
                            .Select((x) =>
                            new
                            {
                                Bounds = new Rectangle(
                                    location: x.Position.ToPoint(),
                                    size: (Point)x.Size),
                                Element = x
                            })
                            .Where((x) => x.Bounds.Intersects(mouseBounds))
                            .First()
                            .Element;
                        simpleLevel.Elements.Remove(elementToPlace);
                    }
                    catch (InvalidOperationException)
                    {
                    }
                }
            }
            // Operations if there's NO element-to-place. 
            else
            {
                // Detect if element-to-place identifier changes.
                if (elementToPlaceIdentifier != levelEditorMenu.ElementToPlaceIdentifier)
                {
                    elementToPlaceIdentifier = levelEditorMenu.ElementToPlaceIdentifier;

                    // If the identifier exists, set the element-to-place to the new element.
                    if (ElementManager.Identifiers.Contains(elementToPlaceIdentifier))
                    {
                        if (elementToPlace is IDestroyable destroyable)
                            destroyable.Dispose();
                        elementToPlace = ElementManager.GetElement(elementToPlaceIdentifier);
                    }
                }

                // Set the position of the element-to-place.
                {
                    int blockX = (mouseState.Position.X + (int)simpleLevel.Camera.Position.X) / blockWidth;
                    int blockY = (mouseState.Position.Y + (int)simpleLevel.Camera.Position.Y) / blockWidth;
                    elementToPlace.Position = new Vector2(
                        x: blockX * blockWidth,
                        y: blockY * blockWidth);
                }

                // Try to place the element if the left mouse button is pressed.
                if (mouseState.WasButtonJustDown(MouseButton.Left))
                {
                    // Only place the element-to-place if it doesn't intersect within the bounding rectangle of all placed elements.
                    Point mousePosition = simpleLevel.Camera.Position.ToPoint() + mouseState.Position;
                    Rectangle mouseBounds = new Rectangle(location: mousePosition, size: new Point(x: 1, y: 1));
                    if (simpleLevel.Elements
                        .Select((x) => new Rectangle(
                            location: x.Position.ToPoint(),
                            size: (Point)x.Size))
                        .All((x) => !x.Intersects(mouseBounds)))
                    {
                        simpleLevel.Elements.Add(elementToPlace);

                        // Replace element-to-place with a new element if one's available.
                        // If not, make sure the element-to-place isn't referring to the placed element.
                        if (elementToPlaceIdentifier.Length > 0)
                            elementToPlace = ElementManager.GetElement(elementToPlaceIdentifier);
                        else
                            elementToPlace = null;
                    }
                }
                // If the drop element menu option is selected, dispose of the element-to-place.
                else if (levelEditorMenu.DropElementSelect.Selected)
                {
                    if (elementToPlace is IDestroyable destroyable)
                        destroyable.Dispose();
                    elementToPlace = null;
                }
            }

            // Dragging the mouse with the right mouse button held moves the
            // the camera in the direction of the drag.
            {
                if (mouseState.WasButtonJustDown(MouseButton.Right))
                {
                    dragCameraStart = mouseState.Position.ToVector2();
                }
                if (mouseState.IsButtonDown(MouseButton.Right))
                {
                    Vector2 moveVector = mouseState.Position.ToVector2() - dragCameraStart;
                    simpleLevel.Camera.Move(direction: moveVector * timeElapsed * dragCameraSpeed);
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
                try
                {
                    SimpleLevelSave save = Saver.Load<SimpleLevelSave>($"{levelEditorMenu.LoadString}.{saveExtension}");
                    simpleLevel.Load(save);
                }
                catch (FileNotFoundException)
                {
                }
            }

            // Update the other objects.
            simpleLevel.Update(gameTime: gameTime);
            levelEditorMenu.Update(gameTime: gameTime);
            roomStateChanger.Update(gameTime: gameTime);
        }
    }
}
