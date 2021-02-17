using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Text;
using System.Collections.Generic;

namespace Assets.Script.Client
{
    public class Client : MonoBehaviour
    {
        public GameObject authorizationMenu, sceneController;
        public static Client instance;

        public static string login, password;

        public static int Id { get; private set; } = -1;
        public static void SetId(int id)
        {
            if (Id == -1) Id = id;
        }
        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
                InitializeClientData();
            }
            else if(instance != this)
            {
                Debug.Log("Instance already wxists, destroying object!");
                Destroy(this);
            }
        }
        public void StartClient()
        {
            IPAddress ip = IPAddress.Parse("192.168.0.169");//("100.84.121.12");

            TcpClient(ip, 420);
        }


        private static TcpClient server;
        private static NetworkStream stream;
        private Packet recivedPacket;
        public bool authorizated { get; private set; }
        public static readonly int dataBufferSize = 4096;
        private async void TcpClient(IPAddress ip, int port)
        {
            server = new TcpClient();
            try
            {
                await server.ConnectAsync(ip, port);
                Debug.Log("Connected");

                server.ReceiveBufferSize = dataBufferSize;
                server.SendBufferSize = dataBufferSize;

                stream = server.GetStream();

                recivedPacket = new Packet();

                Read(stream);
            }
            catch (SocketException e)
            {
                Debug.Log(e.Message);
                server = null;
            }
        }
        private async void Read(NetworkStream stream)
        {
            try
            {
                byte[] data = new byte[512];
                await stream.ReadAsync(data, 0, 512);

                try
                {
                    recivedPacket.Reset(HandleRecivedData(data));
                }
                catch(Exception ex)
                {
                    Debug.LogError(ex);
                    recivedPacket.Reset(true);
                }
                Read(stream);
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
            /*byte[] type = new byte[1];
            await stream.ReadAsync(type, 0, 1);//SocketException: Удаленный хост принудительно разорвал существующее подключение.

            byte[] length = new byte[4];
            await stream.ReadAsync(length, 0, 4);

            int lengthm = BitConverter.ToInt32(length, 0);
            var buffer = new byte[lengthm];
            var byteCount = await stream.ReadAsync(buffer, 0, lengthm);
            var request = Encoding.ASCII.GetString(buffer, 0, byteCount);
            Debug.Log("[Server]Write:" + request);


            var message = Encoding.ASCII.GetString(buffer);
            output.GetComponent<Text>().text = message;
            if (authorizated)
            {
                switch (type[0])
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        //CreateRoom(message);
                        break;
                    case 3:
                        //FindPrivateRoom(message);
                        break;
                    case 4:
                        int id = int.Parse(message.Substring(0, message.IndexOf(authorizationChar)));
                        message.Remove(0, message.IndexOf(authorizationChar));
                        string position = message.Substring(0, message.IndexOf(authorizationChar));
                        message.Remove(0, message.IndexOf(authorizationChar));
                        string forwardPlayer = message.Substring(0, message.IndexOf(authorizationChar));
                        message.Remove(0, message.IndexOf(authorizationChar));
                        string upPlayer = message.Substring(0, message.IndexOf(authorizationChar));

                        sceneController.GetComponent<GameController>().SyncPosition(id, StringToVector3(position),
                            StringToVector3(forwardPlayer), StringToVector3(upPlayer));
                        break;
                }
            }
            else if (type[0] == 0)
            {
                if(message.IndexOf(regMsg) == 0)
                {
                    string login = message.Substring(regMsg.Length, message.IndexOf(authorizationChar) - regMsg.Length);
                    string password = message.Substring(message.IndexOf(authorizationChar) + 1);
                    authorizationMenu.GetComponent<Authorization>().Registered(login, password);
                }
                else if(message.IndexOf(authMsg) == 0)
                {
                    authorizated = true;
                    authorizationMenu.GetComponent<Authorization>().Authorized();
                }
                else
                {
                    authorizationMenu.GetComponent<Authorization>().DoWind();
                }
            }
            else authorizationMenu.GetComponent<Authorization>().DoWind();
            */
        }

        private bool HandleRecivedData(byte[] data)
        {
            int packetLenght = 0;

            recivedPacket.SetBytes(data);

            if (recivedPacket.UnreadLength() >= 4)
            {
                packetLenght = recivedPacket.ReadInt();
                if (packetLenght <= 0) return true;
            }

            while (packetLenght > 0 && packetLenght <= recivedPacket.UnreadLength())
            {
                byte[] packetBytes = recivedPacket.ReadBytes(packetLenght);

                //Thread
                using (Packet packet = new Packet(packetBytes))
                {
                    int packetId = packet.ReadInt();
                    PacketHandlers[packetId](packet);
                }

                if (recivedPacket.UnreadLength() >= 4)
                {
                    packetLenght = recivedPacket.ReadInt();
                    if (packetLenght <= 0) return true;
                }
                //Thread
            }
            if (packetLenght <= 1) return true;

            return false;
        }

        private delegate void PacketHandler(Packet packet);
        private static Dictionary<int, PacketHandler> PacketHandlers;
        private void InitializeClientData()
        {
            PacketHandlers = new Dictionary<int, PacketHandler>()
            {
                {(int)ServerPackets.welcome, HandlePacket.Welcome },
                {(int)ServerPackets.registration, HandlePacket.Registration },
                {(int)ServerPackets.authorization, HandlePacket.Authorization },

                {(int)ServerPackets.connectToRoom, HandlePacket.ConnectToRoom },
                {(int)ServerPackets.errorFindPrivateRoom, HandlePacket.ErrorFindPrivateRoom },

                {(int)ServerPackets.roomInfo, HandlePacket.RoomInfo },

                {(int)ServerPackets.spawnPlayer, HandlePacket.SpawnPlayer },
                {(int)ServerPackets.positionAndRotation, HandlePacket.PositionAndRotation }
            };
        }

        public static Vector3 StringToVector3(string sVector)
        {
            // split the items
            string[] sArray = sVector.Split(';');

            // store as a Vector3
            Vector3 result = new Vector3(
                float.Parse(sArray[0]),
                float.Parse(sArray[1]),
                float.Parse(sArray[2]));

            return result;
        }
        public static async void Write(Packet packet)//(NetworkStream stream, string message, int type)
        {
            // Write a message over the TCP Connection
            try
            {
                if (server != null)
                {
                    byte[] msg = packet.ToArray();
                    Debug.Log($"[Client]Send: {Encoding.ASCII.GetString(msg)}");
                    await stream.WriteAsync(msg, 0, msg.Length);//System.IO.IOException: "Unable to read data from the transport connection: Программа на вашем хост-компьютере разорвала установленное подключение..".                    
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"[Client]Error sending data to client {Id} via TCP: {ex}");
            }
        }
    }
}

struct Message
{
    public byte[] message;


    public Message(string msg, int type)
    {
        byte typem = Convert.ToByte(type);

        int length = msg.Length;
        byte[] lengthm = new byte[4];
        lengthm = BitConverter.GetBytes(length);

        message = new byte[5 + length];
        message[0] = typem;
        for (int i = 1; i < 5; i++) { message[i] = lengthm[i - 1]; }
        length += 5;
        var b = Encoding.ASCII.GetBytes(msg);
        for (int i = 5; i < length; i++)
        {
            message[i] = b[i - 5];
        }
    }
}
