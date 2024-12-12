using Photon.Pun;
using System.Collections.Generic;
using SSPot.Utilities;
using UnityEngine;

public class AmbientSetup : Singleton<AmbientSetup>
{
    [SerializeField] private GameObject computerToHide;
    public List<TeleportToObject> teleports = new();
    
    private void Start()
    {
        if(PhotonNetwork.OfflineMode && teleports.Count > 0)
        {
            teleports[2].enabled = false;
            teleports[2].gameObject.SetActive(false);
        }
        
        if (computerToHide)
            computerToHide.SetActive(false);
    }
}
