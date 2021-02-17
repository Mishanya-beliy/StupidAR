using Assets.Script.Client;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    void Update()
    {
        Transform camera = Camera.main.transform;
        gameObject.transform.SetPositionAndRotation(camera.position, camera.rotation);

        
        GameObject gameTable = GameObject.Find("Game Table");
        if (gameTable != null)
        {
            Transform player = gameObject.transform;
            Transform table = gameTable.transform;

            Vector3 position = player.position;
            position = table.InverseTransformPoint(position);

            Vector3 forwardPlayer, upPlayer;
            forwardPlayer = table.InverseTransformDirection(table.forward - player.TransformDirection(Vector3.forward));
            upPlayer = table.InverseTransformDirection(table.up - player.TransformDirection(Vector3.up));

            Send.PositionAndRotation(Client.Id, position, forwardPlayer, upPlayer);
        }
    }
}
