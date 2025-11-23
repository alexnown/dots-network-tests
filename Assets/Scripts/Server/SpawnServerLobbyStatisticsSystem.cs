using Unity.Collections;
using Unity.Entities;

namespace com.testnet.server
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct SpawnServerLobbyStatisticsSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach ( var (prefab, entity) in SystemAPI.Query<SpawnServerLobbyStatistics>().WithEntityAccess())
            {
                var statisticsInstance = ecb.Instantiate(prefab.StatisticsPrefab);
                ecb.RemoveComponent<SpawnServerLobbyStatistics>(entity);
                if(SystemAPI.HasComponent<SendBrodcastRpcWithLobbyStatistics>(entity))
                {
                    ecb.AddComponent<SendBrodcastRpcWithLobbyStatistics>(statisticsInstance);
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}