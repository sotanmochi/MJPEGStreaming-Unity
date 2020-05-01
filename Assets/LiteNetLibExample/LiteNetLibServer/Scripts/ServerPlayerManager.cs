using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

namespace LiteNetLibExample.Server
{
    public class ServerPlayerManager : MonoBehaviour
    {
        [SerializeField] ServerMain _liteNetLibServer;

        NetDataWriter _dataWriter;
        Dictionary<int, NetworkPlayer> _networkPlayerDictionary;

        void Start()
        {
            _dataWriter = new NetDataWriter();
            _networkPlayerDictionary = new Dictionary<int, NetworkPlayer>();
            _liteNetLibServer.OnPeerConnectedHandler += OnPeerConnected;
            _liteNetLibServer.OnPeerDisconnectedHandler += OnPeerDisconnected;
            _liteNetLibServer.OnNetworkReceived += OnNetworkReceived;
        }

        void FixedUpdate()
        {
            SendPlayerTransformArray();
        }

        void OnPeerConnected(NetPeer peer)
        {
            int newPlayerId = peer.Id;

            _dataWriter.Reset();
            _dataWriter.Put((int)NetworkDataType.PlayerTransformArray);
            foreach (var player in _networkPlayerDictionary)
            {
                _dataWriter.Put(player.Key);
                _dataWriter.Put(player.Value.Position.x);
                _dataWriter.Put(player.Value.Position.y);
                _dataWriter.Put(player.Value.Position.z);
                _dataWriter.Put(player.Value.Rotation.x);
                _dataWriter.Put(player.Value.Rotation.y);
                _dataWriter.Put(player.Value.Rotation.z);
                _dataWriter.Put(player.Value.Rotation.w);
            }
            _liteNetLibServer.SendData(newPlayerId, _dataWriter, DeliveryMethod.Sequenced);

            if (!_networkPlayerDictionary.ContainsKey(newPlayerId))
            {
                _networkPlayerDictionary.Add(newPlayerId, new NetworkPlayer());
            }

            _networkPlayerDictionary[newPlayerId].Moved = true;
        }

        void OnPeerDisconnected(NetPeer peer)
        {
            if (_networkPlayerDictionary.ContainsKey(peer.Id))
            {
                _networkPlayerDictionary.Remove(peer.Id);
                
                _dataWriter.Reset();
                _dataWriter.Put((int)NetworkDataType.PlayerLeave);
                _dataWriter.Put(peer.Id);

                foreach (var sendToPlayer in _networkPlayerDictionary)
                {
                    _liteNetLibServer.SendData(sendToPlayer.Key, _dataWriter, DeliveryMethod.ReliableOrdered);
                }
            }
        }

        void OnNetworkReceived(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            if (reader.UserDataSize >= 4)
            {
                NetworkDataType networkDataType = (NetworkDataType)reader.GetInt();
                if (networkDataType == NetworkDataType.PlayerTransform)
                {
                    UpdatePlayerTransform(peer, reader);
                }
            }
        }

        void UpdatePlayerTransform(NetPeer peer, NetPacketReader reader)
        {
            if ((reader.UserDataSize - 4) == NetworkDataSize.Transform)
            {
                float posX = reader.GetFloat();
                float posY = reader.GetFloat();
                float posZ = reader.GetFloat();
                float rotX = reader.GetFloat();
                float rotY = reader.GetFloat();
                float rotZ = reader.GetFloat();
                float rotW = reader.GetFloat();

                _networkPlayerDictionary[peer.Id].Position.x = posX;
                _networkPlayerDictionary[peer.Id].Position.y = posY;
                _networkPlayerDictionary[peer.Id].Position.z = posZ;
                _networkPlayerDictionary[peer.Id].Rotation.x = rotX;
                _networkPlayerDictionary[peer.Id].Rotation.y = rotY;
                _networkPlayerDictionary[peer.Id].Rotation.z = rotZ;
                _networkPlayerDictionary[peer.Id].Rotation.w = rotW;
                _networkPlayerDictionary[peer.Id].Moved = true;
            }
        }

        void SendPlayerTransformArray()
        {
            foreach (var sendToPlayer in _networkPlayerDictionary)
            {
                if (sendToPlayer.Value == null)
                {
                    continue;
                }

                _dataWriter.Reset();
                _dataWriter.Put((int)NetworkDataType.PlayerTransformArray);
                int movedPlayers = 0;

                foreach (var player in _networkPlayerDictionary)
                {
                    if (sendToPlayer.Key == player.Key)
                    {
                        continue;
                    }

                    if (!player.Value.Moved)
                    {
                        continue;
                    }

                    _dataWriter.Put(player.Key);
                    _dataWriter.Put(player.Value.Position.x);
                    _dataWriter.Put(player.Value.Position.y);
                    _dataWriter.Put(player.Value.Position.z);
                    _dataWriter.Put(player.Value.Rotation.x);
                    _dataWriter.Put(player.Value.Rotation.y);
                    _dataWriter.Put(player.Value.Rotation.z);
                    _dataWriter.Put(player.Value.Rotation.w);

                    movedPlayers++;
                }

                if (movedPlayers > 0)
                {
                    _liteNetLibServer.SendData(sendToPlayer.Key, _dataWriter, DeliveryMethod.Sequenced);
                }
            }

            foreach (var player in _networkPlayerDictionary.Values)
            {
                player.Moved = false;
            }
        }
    }
}
