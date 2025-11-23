using com.testnet.common;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace com.testnet.server
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct HandleConnectToLobbyRpcSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            var rpcQuery = SystemAPI.QueryBuilder().WithAll<EstablishNetworkStreamInGameStateRpc, ReceiveRpcCommandRequest>().Build();
            state.RequireForUpdate(rpcQuery);
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            foreach (var (request, entity) in SystemAPI.Query<ReceiveRpcCommandRequest>().WithAll<EstablishNetworkStreamInGameStateRpc>().WithEntityAccess())
            {
                ecb.DestroyEntity(entity);
                ecb.AddComponent<NetworkStreamInGame>(request.SourceConnection);
                var clientId = SystemAPI.GetComponent<NetworkId>(request.SourceConnection).Value;
                Debug.Log("Server Lobby connected client with id = " + clientId);
            }
        }
    }
}