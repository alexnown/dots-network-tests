using com.testnet.common;
using Unity.Entities;
using Unity.NetCode;

namespace com.testnet.server
{
    public struct SendBrodcastRpcWithLobbyStatistics : IComponentData 
    {
        public float TimeSinceLastSend;
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct UpdateLobbyStatisticsSystem : ISystem
    {
        public const float SEND_STATISTICS_RPC_INTERVAL_SEC = 1f;

        private EntityArchetype _brodcastRpcWithStatistics;
        private EntityQuery _activeNetworkIdsQuery;
        public void OnCreate(ref SystemState state)
        {
            _brodcastRpcWithStatistics = state.EntityManager.CreateArchetype(
                ComponentType.ReadOnly<LobbyStatisticsDataRpc>(),
                ComponentType.ReadOnly<SendRpcCommandRequest>());
            _activeNetworkIdsQuery = SystemAPI.QueryBuilder().WithAll<NetworkId>().Build(); 
            state.RequireForUpdate<LobbyStatisticsData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            var connectionsCount = _activeNetworkIdsQuery.CalculateEntityCount();
            foreach (var statistics in SystemAPI.Query<RefRW<LobbyStatisticsData>>())
            {
                statistics.ValueRW.ConnectionsCount = connectionsCount;
            }
            //send brodcast rpc with lobby statistics if this option enabled by [SendBrodcastRpcWithLobbyStatistics] tag
            foreach (var sendDelay in SystemAPI.Query<RefRW<SendBrodcastRpcWithLobbyStatistics>>().WithAll<LobbyStatisticsData>())
            {
                sendDelay.ValueRW.TimeSinceLastSend += SystemAPI.Time.DeltaTime;
                if(sendDelay.ValueRW.TimeSinceLastSend > SEND_STATISTICS_RPC_INTERVAL_SEC)
                {
                    sendDelay.ValueRW.TimeSinceLastSend = 0;
                    var rpc = ecb.CreateEntity(_brodcastRpcWithStatistics);
                    ecb.SetComponent(rpc, new LobbyStatisticsDataRpc { ConnectionsCount = connectionsCount });
                }
            }
        }
    }
}