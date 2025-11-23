using Unity.Entities;
using Unity.NetCode;

namespace com.testnet.common
{
    public struct RealtimeRoomStatistics : IComponentData
    {
        [GhostField] public int RoomPort;
        [GhostField] public int ActiveConnections;
    }
}