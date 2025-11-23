using com.testnet.common;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace com.testnet.client
{

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct SendConnectToLobbyRpcSystem : ISystem
    {
        private EntityQuery _pendingNetworkIdQuery;

        public void OnCreate(ref SystemState state)
        {
            _pendingNetworkIdQuery = SystemAPI.QueryBuilder().WithAll<NetworkId>().WithNone<NetworkStreamInGame>().Build();
            state.RequireForUpdate(_pendingNetworkIdQuery);
            state.RequireForUpdate<EstablishNetworkStreamInGameStateRpc>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            foreach(var (netId, entity) in SystemAPI.Query<NetworkId>().WithNone<NetworkStreamInGame>().WithEntityAccess())
            {
                ecb.AddComponent<NetworkStreamInGame>(entity);
            }
            var requestEntity = ecb.CreateEntity();
            ecb.AddComponent(requestEntity, new EstablishNetworkStreamInGameStateRpc());
            ecb.AddComponent(requestEntity, new SendRpcCommandRequest());
        }
    }
}
