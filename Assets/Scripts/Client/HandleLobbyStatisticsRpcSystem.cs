using com.testnet.common;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace com.testnet.client
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct HandleLobbyStatisticsRpcSystem : ISystem
    {
        private EntityQuery _lobbyStatisticsDataReceiver;

        public void OnCreate(ref SystemState state)
        {
            var receivedRpcQuery = SystemAPI.QueryBuilder().WithAll<LobbyStatisticsDataRpc, ReceiveRpcCommandRequest>().Build();
            state.RequireForUpdate(receivedRpcQuery);
            _lobbyStatisticsDataReceiver = SystemAPI.QueryBuilder().WithAll<LobbyStatisticsData>().WithNone<GhostInstance>().Build();
            //state.RequireForUpdate(_lobbyStatisticsDataReceiver);
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            foreach (var (statistics, entity) in SystemAPI.Query<LobbyStatisticsDataRpc>().WithAll<ReceiveRpcCommandRequest>().WithEntityAccess())
            {
                ecb.DestroyEntity(entity);
                using (var statisticsDataReceivers = _lobbyStatisticsDataReceiver.ToEntityArray(Allocator.Temp))
                {
                    foreach(var receiverEntity in statisticsDataReceivers)
                    {
                        SystemAPI.SetComponent(receiverEntity, new LobbyStatisticsData { ConnectionsCount = statistics.ConnectionsCount });
                    }
                }
            }
        }
    }
}
