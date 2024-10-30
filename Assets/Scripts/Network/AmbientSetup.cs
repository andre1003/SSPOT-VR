using Photon.Pun;
using System.Collections.Generic;
using SSpot.Utilities;
using UnityEngine;

public class AmbientSetup : Singleton<AmbientSetup>
{
    [SerializeField] private GameObject computerToHide;
    public List<TeleportToObject> teleports = new();

    private readonly List<PhotonView> _players = new();
    public IReadOnlyList<PhotonView> Players => _players;
    
    public PhotonView LocalPlayer { get; private set; }
    
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

    /// <summary>
    /// Configure ambient with local player settings.
    /// </summary>
    /// <param name="playerId">Local player Photon View ID.</param>
    public void ConfigureAmbient(int playerId)
    {
        PhotonView player = PhotonView.Find(playerId);
        _players.Add(player);
        
        if (player.IsMine)
            LocalPlayer = player;
    }
}
