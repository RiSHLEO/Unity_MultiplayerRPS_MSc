using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private string _roomCode = "Map1";
    [Space]
    [SerializeField] private GameObject _player;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private GameObject _ConnectingScreen;

    public FormDatabase FormDatabase;

    private void Start()
    {
        Debug.Log("Connecting...");
        Screen.SetResolution(800, 600, false);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        Debug.Log("Joining Lobby...");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        Debug.Log("Joining or creating room...");
        PhotonNetwork.JoinOrCreateRoom(_roomCode, null, null);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("Joined Room. Spawning Player...");
        _ConnectingScreen.SetActive(false);
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        Vector3 spawnPosition = new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
        GameObject playerPrefab = PhotonNetwork.Instantiate(_player.name, spawnPosition, Quaternion.identity);
        Player player = playerPrefab.GetComponent<Player>();

        if (player.photonView.IsMine)
        {
            CameraFollowPlayer cameraFollow = FindFirstObjectByType<CameraFollowPlayer>();
            if (cameraFollow != null)
                cameraFollow.SetFollow(playerPrefab.transform);
        }

        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        int formIndex = (playerCount - 1) % 3;
        player.photonView.RPC(("AssignInitialForm"), RpcTarget.AllBuffered, formIndex);
    }
}
