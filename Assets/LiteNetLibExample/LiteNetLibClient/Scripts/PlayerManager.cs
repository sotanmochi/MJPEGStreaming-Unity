using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

namespace LiteNetLibExample.Client
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] ClientMain _liteNetLibClient;
        public Transform LocalPlayer;
        public GameObject NetworkPlayerPrefab;

        NetDataWriter _dataWriter;
    
        Dictionary<int, Transform> _networkPlayersDictionary;
        Vector3 lastNetworkPosition = Vector3.zero;
        Quaternion lastNetworkRotation = Quaternion.identity;

        void Start()
        {
            _dataWriter = new NetDataWriter();
            _networkPlayersDictionary = new Dictionary<int, Transform>();
            _liteNetLibClient.OnNetworkReceived += OnNetworkReceived;
        }

        void FixedUpdate()
        {
            if (LocalPlayer != null)
            {
                if(!lastNetworkPosition.Equals(LocalPlayer.position)
                || !lastNetworkRotation.Equals(LocalPlayer.rotation))
                {
                    _dataWriter.Reset();

                    _dataWriter.Put((int)NetworkDataType.PlayerTransform);
                    _dataWriter.Put(LocalPlayer.position.x);
                    _dataWriter.Put(LocalPlayer.position.y);
                    _dataWriter.Put(LocalPlayer.position.z);
                    _dataWriter.Put(LocalPlayer.rotation.x);
                    _dataWriter.Put(LocalPlayer.rotation.y);
                    _dataWriter.Put(LocalPlayer.rotation.z);
                    _dataWriter.Put(LocalPlayer.rotation.w);

                    _liteNetLibClient.Send(_dataWriter, DeliveryMethod.Sequenced);

                    lastNetworkPosition = LocalPlayer.position;
                    lastNetworkRotation = LocalPlayer.rotation;
                }
            }
        }

        void OnNetworkReceived(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            if (reader.UserDataSize >= 4)
            {
                NetworkDataType networkDataType = (NetworkDataType)reader.GetInt();
                if (networkDataType == NetworkDataType.PlayerTransformArray)
                {
                    UpdateNetworkPlayerPosition(peer, reader);
                }
                else if (networkDataType == NetworkDataType.PlayerLeave)
                {
                    int playerId = reader.GetInt();
                    DestroyImmediate(_networkPlayersDictionary[playerId].gameObject);
                    _networkPlayersDictionary.Remove(playerId);
                }
            }
        }

        void UpdateNetworkPlayerPosition(NetPeer peer, NetPacketReader reader)
        {
            int dataNum = (reader.UserDataSize - 4) / NetworkDataSize.PlayerIdAndTransform;

            for(int i = 0; i < dataNum; i++)
            {
                int playerId = reader.GetInt();
                float posX = reader.GetFloat();
                float posY = reader.GetFloat();
                float posZ = reader.GetFloat();
                float rotX = reader.GetFloat();
                float rotY = reader.GetFloat();
                float rotZ = reader.GetFloat();
                float rotW = reader.GetFloat();

                if (!_networkPlayersDictionary.ContainsKey(playerId))
                {
                    GameObject go = GameObject.Instantiate(NetworkPlayerPrefab, new Vector3(posX, posY, posZ) , new Quaternion(rotX, rotY, rotZ, rotW));
                    _networkPlayersDictionary.Add(playerId, go.transform);
                }

                Vector3 position = _networkPlayersDictionary[playerId].position;
                position.x = posX;
                position.y = posY;
                position.z = posZ;

                Quaternion rotation = _networkPlayersDictionary[playerId].rotation;
                rotation.x = rotX;
                rotation.y = rotY;
                rotation.z = rotZ;
                rotation.w = rotW;

                _networkPlayersDictionary[playerId].position = position;
                _networkPlayersDictionary[playerId].rotation = rotation;
            }
        }
    }
}
