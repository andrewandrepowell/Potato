using Potato.Element.Character;
using Potato.Element.Wall;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato.Element
{
    internal static class ElementManager
    {
        private static List<string> identifiers;
        public static ICollection<string> Identifiers => identifiers;

        static ElementManager()
        {
            identifiers = new List<string>();
            identifiers.AddRange(CharacterManager.Identifiers);
            identifiers.AddRange(WallManager.Identifiers);
        }

        public static IElement GetElement(string identifier)
        {
            if (WallManager.Identifiers.Contains(identifier))
                return WallManager.GetWall(identifier);
            else if (CharacterManager.Identifiers.Contains(identifier))
                return CharacterManager.GetCharacter(identifier);
            else throw new ArgumentException($"{identifier} not a supported identifier.");
        }
    }
}
