using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectDisplay : NetworkBehaviour
{

    public NetworkList<CharacterSelectState> players;
    [SerializeField] private PlayerCard[] playerCards;
    public static CharacterSelectDisplay Instance {get; private set;}

    private void Awake()
    {
        players = new NetworkList<CharacterSelectState>();
        if(Instance != null && Instance != this){
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        if(IsClient){
            players.OnListChanged += handlePlayerStateChange;
        }

        if(IsServer){
            NetworkManager.Singleton.OnClientConnectedCallback += handleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += handleClientDisconnected;
        }
    }

    private void handleClientConnected(ulong clientID){
        players.Add(new CharacterSelectState(clientID, -1));
    }

    private void handleClientDisconnected(ulong clientID){
        for(int i = 0; i < players.Count; i++){
            if(players[i].clientID == clientID){
                players.RemoveAt(i);
                break;
            }
        }
    }

    private void handlePlayerStateChange(NetworkListEvent<CharacterSelectState> changeEvent){
        for(int i = 0; i < playerCards.Length; i++){
            if(players.Count > i)
            {
                playerCards[i].UpdateDisplay(players[i]);
            } else {
                playerCards[i].DisableDisplay();
            }
        }
    }

    public void Select(int characterID){
        SelectServerRpc(characterID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectServerRpc(int characterID, ServerRpcParams serverRpcParams = default){
        for(int i = 0; i < players.Count; i++){
            if(players[i].clientID == serverRpcParams.Receive.SenderClientId){
                players[i] = new CharacterSelectState(players[i].clientID, characterID);
            }
        }
    }

    public void StartGame(){
        NetworkManager.Singleton.SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }


}