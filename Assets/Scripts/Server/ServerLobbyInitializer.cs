using com.testnet.common;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Scenes;
using UnityEngine;

namespace com.testnet.server
{
    public class ServerLobbyInitializer : MonoBehaviour
    {
        [SerializeField] private SubScene _serializedLobbyServer;

        private void Start()
        {
            DestroyLocalWorld();
            var serverWorld = ClientServerBootstrap.CreateServerWorld("Server Lobby");
            var servEndpoint = NetworkEndpoint.AnyIpv4.WithPort(NetworkConstants.SERVER_LOBBY_PORT);
            using (var networkDriverQuery = serverWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>()))
            {
                networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(servEndpoint);
            }
            SceneSystem.LoadSceneAsync(serverWorld.Unmanaged, _serializedLobbyServer.SceneGUID);
            World.DefaultGameObjectInjectionWorld = serverWorld;
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