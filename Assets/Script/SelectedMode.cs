using Assets.Script.Client;
using UnityEngine;
using UnityEngine.UI;

public class SelectedMode : MonoBehaviour
{
    public GameObject selectingPanel, editingPanel, aRDevice, environmentalLight, planeGenerator, gameTable, tCPClient;
    public void Mode(bool single) //single true / multi false
    {
        GameObject.Find("Scene Controller").GetComponent<GameController>().Mode(single);
    }
}
