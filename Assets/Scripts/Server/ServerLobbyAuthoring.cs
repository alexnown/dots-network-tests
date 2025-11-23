using com.testnet.common;
using Unity.Entities;
using UnityEngine;

namespace com.testnet.server
{
    public struct SpawnServerLobbyStatistics : IComponentData
    {
        public Entity StatisticsPrefab;
    }

    public class ServerLobbyAuthoring : MonoBehaviour
    {
        public GameObject StatisticsPrefab;
        public bool UseBrodcastRpcToSendStatisticsToClients;

        public class Baker : Baker<ServerLobbyAuthoring>
        {
            public override void Bake(ServerLobbyAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                if(authoring.StatisticsPrefab != null)
                {
                    var prefab = GetEntity(authoring.StatisticsPrefab, TransformUsageFlags.None);
                    AddComponent(entity, new SpawnServerLobbyStatistics { StatisticsPrefab = prefab });
                } 
                else
                {
                    AddComponent<LobbyStatisticsData>(entity);
                }
                if (authoring.UseBrodcastRpcToSendStatisticsToClients)
                {
                    AddComponent<SendBrodcastRpcWithLobbyStatistics>(entity);
                }
            }
        }
    }
}