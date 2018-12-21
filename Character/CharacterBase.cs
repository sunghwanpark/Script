using UnityEngine;
using ExitGames.Client.Photon;

namespace SHProject.Ingame
{
    [RequireComponent(typeof(PhotonView))]
    public partial class CharacterBase : CommonLocatePhoton
    {
    }
}
