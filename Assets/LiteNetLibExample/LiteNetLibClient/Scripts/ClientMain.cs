using System.Net;
using System.Net.Sockets;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

namespace LiteNetLibExample.Client
{
    public class ClientMain : MonoBehaviour, INetEventListener
    {
        [SerializeField] string _address = "localhost";
        [SerializeField] int _port = 15000;
        [SerializeField] string _key = "LiteNetLibExample";

        NetManager _clientNetManager;
        NetPeer _serverPeer;

        public delegate void OnNetworkReceiveDelegate(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod);
        public OnNetworkReceiveDelegate OnNetworkReceived;

        void Start()
        {
            StartClient();
        }

        void OnApplicationQuit()
        {
            StopClient();
        }

        void FixedUpdate()
        {
            if (_clientNetManager != null && _clientNetManager.IsRunning)
            {
                _clientNetManager.PollEvents();
            }
        }

        public bool StartClient()
        {
            _clientNetManager = new NetManager(this);
            if (_clientNetManager.Start())
            {
                Debug.Log("Client net manager started!");
                _clientNetManager.Connect(_address, _port, _key);
                return true;
            }
            else
            {
                Debug.LogError("Could not start client net manager!");
                return false;
            }
        }

        public void StopClient()
        {
            if (_clientNetManager != null && _clientNetManager.IsRunning)
            {
                _clientNetManager.Stop();
                Debug.Log("Client net manager stopped.");
            }
        }

        public void Send(NetDataWriter dataWriter, DeliveryMethod deliveryMethod)
        {
            if (_serverPeer != null)
            {
                _serverPeer.Send(dataWriter, deliveryMethod);
            }
        }

        void INetEventListener.OnPeerConnected(NetPeer peer)
        {
            _serverPeer = peer;
            Debug.Log("OnPeerConnected : " + peer.EndPoint.Address + " : " + peer.EndPoint.Port);
        }

        void INetEventListener.OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            _serverPeer = null;
            Debug.Log("OnPeerDisconnected : " + peer.EndPoint.Address + " : " + peer.EndPoint.Port + " Reason : " + disconnectInfo.Reason.ToString());
        }

        void INetEventListener.OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Debug.LogError("OnNetworkError : " + socketError);
        }

        void INetEventListener.OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            OnNetworkReceived?.Invoke(peer, reader, deliveryMethod);
        }

        void INetEventListener.OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            Debug.Log("OnNetworkReceiveUnconnected");
        }

        void INetEventListener.OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        void INetEventListener.OnConnectionRequest(ConnectionRequest request)
        {
        }
    }
}
