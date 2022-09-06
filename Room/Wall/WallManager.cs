using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato.Room.Wall
{
    internal static class WallManager
    {
        private static Dictionary<string, (string, string)> identifierToSimpleWallMap;
        public static ICollection<string> Identifiers => identifierToSimpleWallMap.Keys;

        static WallManager()
        {
            identifierToSimpleWallMap = new Dictionary<string, (string, string)>();
            identifierToSimpleWallMap.Add("test1", ("test1", "test1"));
            identifierToSimpleWallMap.Add("test2", ("test2", "test2"));
            identifierToSimpleWallMap.Add("test3", ("test3", "test3"));
            identifierToSimpleWallMap.Add("test4", ("test4", "test4"));
        }
        public static IWallable GetWall(string identifier)
        {
            ContentManager content = Potato.Game.Content;
            (string collisionMaskName, string displayTextureName) = identifierToSimpleWallMap[identifier];
            return new SimpleWall(
                collisionMask: content.Load<Texture2D>(collisionMaskName),
                displayTexture: content.Load<Texture2D>(displayTextureName),
                identifier: identifier);
        }
    }
}
