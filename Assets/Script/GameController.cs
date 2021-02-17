using Assets.Script.Client;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject authorizationWindow, localPlayerPrefab, playerPrefab, nameRoomTabloPrefab;

    public static GameObject gameTable;

    public GameObject selectingPanel, editingPanel, aRDevice, environmentalLight, planeGenerator, gameTablePref, tCPClient;

    private bool isSingleMode, isEditedTable;
    private static GameObject nameRoomTablo;

    public static GameController instance;
    public static Dictionary<int, Player> players = new Dictionary<int, Player>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already wxists, destroying object!");
            Destroy(this);
        }
    }
    private void Start()
    {
        CreateCamera();
    }

    public void CreateCamera()
    {
        Instantiate(aRDevice).name = "ARCore Device";
        Instantiate(planeGenerator).name = "Plane Generator";
    }
    public void Mode(bool single) //single true / multi false
    {
        //Create
        Instantiate(environmentalLight).name = "Environmental Light";
        gameTable = Instantiate(gameTablePref);
        gameTable.name = "Game Table";

        //Show
        editingPanel.SetActive(true);
        editingPanel.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(
            delegate { gameTable.GetComponent<PlaceTable>().StopMoving(); });

        //Hide
        selectingPanel.SetActive(false);

        //Set default parameters
        GetComponent<GameController>().SetGameMode(single);

        //Create connection with server
        if (!single)
        {
            //Instantiate(tCPClient).name = "TCP Client";
            tCPClient.GetComponent<Client>().StartClient();
        }

    }
    public void SetGameMode(bool single)
    {
        isSingleMode = single;
    }
    public bool EditingTable()
    {
        return !isEditedTable;
    }
    public void EditedTable()
    {
        if (isSingleMode)
        {

        }
        else
        {
            isEditedTable = true;
            authorizationWindow.GetComponent<Authorization>().DoWind();
        }
    }

    public void SetRoomInfo(int idRoom, string nameRoom, int countPlayers)
    {
        gameTable.GetComponent<PlaceTable>().CreateTablo(idRoom + ":" + nameRoom);
    }

    internal void SpawnPlayer(int id, string nickName, Vector3 position)
    {
        GameObject player;
        if (id == Client.Id)
            player = localPlayerPrefab;
        else player = playerPrefab;

        players.Add(id, Instantiate(player, new Vector3(position.x, position.y, position.z),
            Quaternion.identity).GetComponent<Player>());

        players[id].Rename(nickName);
    }



    internal static void SyncPosition(int id, Vector3 position, Vector3 forrward, Vector3 up)
    {
        if (id != Client.Id)
        {
            if (players.ContainsKey(id))
                players[id].SetPositionAndRotation(position, forrward, up);
            else Debug.LogError("GameController.Постановка несуществующего обьекта.");
        }
        else Debug.LogError("GameController.Несанкционированная попытка доступа).");
    }
    void Update()
    {
        //if(nameRoomTablo != null)
        //    nameRoomTablo.transform.LookAt(GameObject.FindGameObjectWithTag("MainCamera").transform);       
    }
}
