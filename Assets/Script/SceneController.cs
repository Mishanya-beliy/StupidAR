using UnityEngine;

public class SceneController : MonoBehaviour
{
    public GameObject gameTable;
    public GameObject gameTablered;
    public GameObject gameTablegrreen;

    public void CreateTable()
    {
        GameObject table = Instantiate(gameTable);
        table.name = "GameTable";
    }
    public void Awake()
    {
        // Enable ARCore to target 60fps camera capture frame rate on supported devices.
        // Note, Application.targetFrameRate is ignored when QualitySettings.vSyncCount != 0.
        Application.targetFrameRate = 60;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    bool gameTableMoving = true;
    void Update()
    {

        if (gameTableMoving)
        {
            Touch touch;
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                return;
            }

            if (GameObject.Find("GameTable").GetComponent<PlaceTable>().StopMoving()) gameTableMoving = false;
        }
    }

    private void EditGameTable()
    {
        gameTableMoving = true;
        GameObject.Find("GameTable").GetComponent<PlaceTable>().StartEditTable();
    }
}
