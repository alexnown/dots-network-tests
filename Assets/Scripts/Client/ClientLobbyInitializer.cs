using com.testnet.common;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Scenes;
using UnityEngine;

namespace com.testnet.client
{
    public class ClientLobbyInitializer : MonoBehaviour
    {
        [SerializeField] private SubScene _serializedLobbyClient;

        private void Start()
        {
            DestroyLocalWorld();
            var clientWorld = ClientServerBootstrap.CreateClientWorld("Client Lobby");
            NetworkEndpoint endpoint = NetworkEndpoint.Parse(NetworkConstants.SERVER_LOBBY_IP, NetworkConstants.SERVER_LOBBY_PORT);
            using (var networkDeliverQuery = clientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>()))
            {
                networkDeliverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(clientWorld.EntityManager, endpoint);
            }
            SceneSystem.LoadSceneAsync(clientWorld.Unmanaged, _serializedLobbyClient.SceneGUID);
            World.DefaultGameObjectInjectionWorld = clientWorld;
        }

        private void DestroyLocalWorld()
        {
            foreach (var world in World.All)
            {
                if (world.Flags == WorldFlags.Game)
                {
                    world.Dispose();
                    break;
                }
            }
        }
    }
}