using Assets.Script.Client;
using UnityEngine;

namespace Assets.Script
{
    public class HandlePacket
    {
        private const string welcome = "Your id is:";
        public static void Welcome(Packet packet)
        {
            string message = packet.ReadString();
            if (message == welcome)
            {
                int id = packet.ReadInt();

                Client.Client.SetId(id);

                Debug.Log($"Client.Message from server: {message} {id}");
            }
            else
            {
                Debug.Log($"Client.Message from server dont have special protocol");

            }
        }

        public static void Authorization(Packet packet)
        {
            string message = packet.ReadString();

            GameObject.Find("AuthorizationMenu").GetComponent<Authorization>().Authorized(message);

            Debug.Log($"Client.Message from server: {message}");
        }
        public static void Registration(Packet packet)
        {
            string message = packet.ReadString();

            GameObject.Find("AuthorizationMenu").GetComponent<Authorization>().Registered(message);

            Debug.Log($"Client.Message from server: {message}");
        }

        private static string successfulConnectToRoom = "Successful connect to room.";
        internal static void ConnectToRoom(Packet packet)
        {
            string message = packet.ReadString();

            if (message == successfulConnectToRoom)
            {
                GameObject.Find("Multiplayer Menu").GetComponent<MultiplayerMenu>().ConnectToRoom();
            }
        }

        internal static void ErrorFindPrivateRoom(Packet packet)
        {
            string message = packet.ReadString();
            GameObject.Find("Multiplayer Menu").GetComponent<MultiplayerMenu>().ErrorFindPrivateRoom(message);
        }

        internal static void RoomInfo(Packet packet)
        {
            int idRoom = packet.ReadInt();
            string nameRoom = packet.ReadString();
            int countPlayers = packet.ReadInt();

            GameObject.Find("Scene Controller").GetComponent<GameController>().SetRoomInfo(idRoom, nameRoom, countPlayers);
            
            Transform camera = Camera.main.transform;
            if (camera != null)
            {
                Vector3 position = camera.position;

                Send.SpawnPlayer(Client.Client.Id, Client.Client.login,
                    new Vector3(position.x, position.y, position.z));
            }
            else Debug.LogError("Error camera not found. WTF!!!!");
        }

        internal static void SpawnPlayer(Packet packet)
        {
            int id = packet.ReadInt();
            string nickName = packet.ReadString();
            Vector3 position = packet.ReadVector3();

            GameObject.Find("Scene Controller").GetComponent<GameController>().SpawnPlayer(id, nickName, position);
        }

        internal static void PositionAndRotation(Packet packet)
        {
            int id = packet.ReadInt();
            Vector3 position = packet.ReadVector3();
            Vector3 forwardPlayer = packet.ReadVector3();
            Vector3 upPlayer = packet.ReadVector3();

            GameController.SyncPosition(id, position, forwardPlayer, upPlayer);
        }
    }
}
