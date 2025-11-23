using Unity.Entities;
using UnityEngine;

namespace com.testnet.common
{
    public class LobbyStatisticsAuthoring : MonoBehaviour
    {
        public class Baker : Baker<LobbyStatisticsAuthoring>
        {
            public override void Bake(LobbyStatisticsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<LobbyStatisticsData>(entity);
                //AddComponent<TicTacToeStatistics>(entity);
                //AddComponent<RealtimeRoomStatistics>(entity);
            }
        }
    }
}