

using UnityEngine;

namespace Assets.Script.Client
{
    class Send
    {
        internal static void RegistrationOrAuthorization(bool authorization, string login, string password)
        {
            int idClient = Client.Id;
            if (idClient != -1)
            {
                int packetId;
                if (authorization)
                    packetId = (int)ClientPackets.authorization;
                else
                    packetId = (int)ClientPackets.registration;
                using (Packet packet = new Packet(packetId))
                    Write(packet, login, password);
            }
        }

        internal static void CreateRoom(string name, string password)
        {
            using (Packet packet = new Packet((int)ClientPackets.createPrivateRoom))
                Write(packet, name, password);
        }

        internal static void FindPrivateRoom(string name, string password)
        {
            using (Packet packet = new Packet((int)ClientPackets.findPrivateRoom))
                Write(packet, name, password);
        }

        internal static void SpawnPlayer(int id, string nickName, Vector3 position)
        {
            using (Packet packet = new Packet((int)ClientPackets.spawnPlayer))
            {
                packet.Write(id);
                packet.Write(nickName);
                packet.Write(position);
                SendData(packet);
            }
        }
        internal static void PositionAndRotation(int id, Vector3 position, Vector3 forwardPlayer, Vector3 upPlayer)
        {
            using (Packet packet = new Packet((int)ClientPackets.positionAndRotation))
            {
                packet.Write(id);
                packet.Write(position);
                packet.Write(forwardPlayer);
                packet.Write(upPlayer);

                SendData(packet);
            }
        }

        private static void Write(Packet packet, string parametr1, string parametr2)
        {
            packet.Write(parametr1);
            packet.Write(parametr2);

            SendData(packet);
        }

        private static void SendData(Packet packet)
        {
            packet.WriteLength();
            Client.Write(packet);
        }
    }
}
