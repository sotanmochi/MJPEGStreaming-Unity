using UnityEngine;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;

namespace LiteNetLibExample.Server
{
    public class ServerMain : MonoBehaviour, INetEventListener
    {
        [SerializeField] int _port = 15000;
        [SerializeField] string _key = "LiteNetLibExample";

        NetManager _serverNetManager;
        Dictionary<int, NetPeer> _networkClientDictionary;

        public delegate void OnPeerConnectedDelegate(NetPeer peer);
        public OnPeerConnectedDelegate OnPeerConnectedHandler;

        public delegate void OnPeerDisconnectedDelegate(NetPeer peer);
        public OnPeerDisconnectedDelegate OnPeerDisconnectedHandler;

        public delegate void OnNetworkReceiveDelegate(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod);
        public OnNetworkReceiveDelegate OnNetworkReceived;

        void Awake()
        {
            StartServer();
        }

        void OnApplicationQuit()
        {
            StopServer();
        }

        void StartServer()
        {
            _serverNetManager = new NetManager(this);
            _networkClientDictionary = new Dictionary<int, NetPeer>();

            if (_serverNetManager.Start(_port))
            {
                Console.WriteLine("Server started listening on port " + _port);
            }
            else
            {
                Console.WriteLine("Server could not start!");
            }
        }

        void StopServer()
        {
            if (_serverNetManager != null && _serverNetManager.IsRunning)
            {
                _serverNetManager.Stop();
                Console.WriteLine("Server stopped.");
            }
        }

        void FixedUpdate()
        {
            if (_serverNetManager.IsRunning)
            {
                _serverNetManager.PollEvents();
            }
        }

        public void SendData(int clientId, NetDataWriter dataWriter, DeliveryMethod deliveryMethod)
        {
            if (_networkClientDictionary.ContainsKey(clientId))
            {
                _networkClientDictionary[clientId].Send(dataWriter, deliveryMethod);
            }
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Console.WriteLine("OnPeerConnected : " + peer.EndPoint.Address + " : " + peer.EndPoint.Port);

            if (!_networkClientDictionary.ContainsKey(peer.Id))
            {
                _networkClientDictionary.Add(peer.Id, peer);
            }

            OnPeerConnectedHandler?.Invoke(peer);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Console.WriteLine("OnPeerDisconnected : " + peer.EndPoint.Address + " : " + peer.EndPoint.Port + " Reason : " + disconnectInfo.Reason.ToString());

            if (_networkClientDictionary.ContainsKey(peer.Id))
            {
                _networkClientDictionary.Remove(peer.Id);
            }

            OnPeerDisconnectedHandler?.Invoke(peer);
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Console.WriteLine("OnNetworkError : " + socketError);
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            OnNetworkReceived?.Invoke(peer, reader, deliveryMethod);
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            Console.WriteLine("OnNetworkReceiveUnconnected");
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            request.AcceptIfKey(_key);
        }
    }
}
