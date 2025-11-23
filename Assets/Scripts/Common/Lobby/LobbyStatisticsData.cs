using Unity.Entities;
using Unity.NetCode;

namespace com.testnet.common
{
    public struct LobbyStatisticsData : IComponentData
    {
        [GhostField] public int ConnectionsCount;
    }


}