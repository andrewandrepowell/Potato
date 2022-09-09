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
            identifierToSimpleWallMap.Add("mask_block_bottom", ("walls/mask_block_bottom", "walls/mask_block_bottom"));
            identifierToSimpleWallMap.Add("mask_block_left", ("walls/mask_block_left", "walls/mask_block_left"));
            identifierToSimpleWallMap.Add("mask_block_right", ("walls/mask_block_right", "walls/mask_block_right"));
            identifierToSimpleWallMap.Add("mask_block_top", ("walls/mask_block_top", "walls/mask_block_top"));
            identifierToSimpleWallMap.Add("mask_ramp_top_left_0", ("walls/mask_ramp_top_left_0", "walls/mask_ramp_top_left_0"));
            identifierToSimpleWallMap.Add("mask_ramp_top_left_1", ("walls/mask_ramp_top_left_1", "walls/mask_ramp_top_left_1"));
            identifierToSimpleWallMap.Add("mask_ramp_top_left_2", ("walls/mask_ramp_top_left_2", "walls/mask_ramp_top_left_2"));
            identifierToSimpleWallMap.Add("mask_ramp_top_right_0", ("walls/mask_ramp_top_right_0", "walls/mask_ramp_top_right_0"));
            identifierToSimpleWallMap.Add("mask_ramp_top_right_1", ("walls/mask_ramp_top_right_1", "walls/mask_ramp_top_right_1"));
            identifierToSimpleWallMap.Add("mask_ramp_top_right_2", ("walls/mask_ramp_top_right_2", "walls/mask_ramp_top_right_2"));
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
