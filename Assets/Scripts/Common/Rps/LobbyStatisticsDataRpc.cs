using Unity.NetCode;

namespace com.testnet.common
{
    public struct LobbyStatisticsDataRpc : IRpcCommand 
    {
        public int ConnectionsCount;
    }
}