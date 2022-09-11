using System;
using System.Collections.Generic;
using Potato.Element.Character;

namespace Potato.Element.Character
{
    internal static class CharacterManager
    {
        private static List<string> identifiers;
        public static ICollection<string> Identifiers => identifiers;

        static CharacterManager()
        {
            identifiers = new List<string>()
            {
                "player"
            };
        }
        public static ICharacterizable GetCharacter(string identifier)
        {
            if (!identifiers.Contains(identifier))
                throw new ArgumentException($"{identifier} not supported.");
            switch (identifier)
            {
                case "player":
                    return new Player() { Identifier = identifier };
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
