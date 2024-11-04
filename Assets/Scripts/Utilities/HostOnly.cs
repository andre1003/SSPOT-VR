using Photon.Pun;
using UnityEngine;

namespace SSPot.Utilities
{
    public class HostOnly : MonoBehaviour
    {
        private void Awake()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Destroy(gameObject);
            }
        }
    }
}