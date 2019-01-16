using UnityEngine;
using System.Collections.Generic;
using SHProject.Ingame;

namespace SHProject.Network
{
    public class CharacterManager : Singleton<CharacterManager>
    {
        private Dictionary<int, CharacterBase> chars = new Dictionary<int, CharacterBase>();

        public void RegisterCharacter(int playerID, CharacterBase photonView)
        {
            if (!chars.ContainsKey(playerID))
                chars.Add(playerID, photonView);
        }

        public CharacterBase GetCharacter(int playerId)
        {
            return chars.ContainsKey(playerId) ? chars[playerId] : null;
        }

        public bool RemoveCharacter(int playerId)
        {
            return chars.Remove(playerId);
        }
    }
}