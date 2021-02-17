using UnityEngine;

public class Player : MonoBehaviour
{
    public TextMesh nick;

    internal void SetPositionAndRotation(Vector3 position, Vector3 forrward, Vector3 up)
    {
        Transform table = GameController.gameTable.transform;

        Vector3 positionPlayer = table.TransformPoint(position);

        Vector3 forwardPlayer = table.TransformDirection(table.InverseTransformDirection(table.forward) - forrward);
        Vector3 upPlayer = table.TransformDirection(table.InverseTransformDirection(table.up) - up);

        gameObject.transform.SetPositionAndRotation(positionPlayer, Quaternion.LookRotation(forwardPlayer, upPlayer));
    }

    internal void Rename(string nick)
    {
        this.nick.text = nick;
    }
}
