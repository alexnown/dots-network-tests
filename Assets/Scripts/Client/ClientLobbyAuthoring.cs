using com.testnet.common;
using Unity.Entities;
using UnityEngine;

namespace com.testnet.client
{
    public class ClientLobbyAuthoring : MonoBehaviour
    {
        public GameObject StatisticsPrefab; // will spawn on client side by ghost components
        public bool HandleBrodcastRpcWithLobbyStatisticsFromServer;
        public bool EstablishNetworkStreamInGameState;

        public class Baker : Baker<ClientLobbyAuthoring>
        {
            public override void Bake(ClientLobbyAuthoring authoring)
            {
                if(authoring.StatisticsPrefab != null)
                {
                    //add to list prefabs for auto spawn by ghost components
                    GetEntity(authoring.StatisticsPrefab, TransformUsageFlags.None);
                }
                var entity = GetEntity(TransformUsageFlags.None);
                if(authoring.HandleBrodcastRpcWithLobbyStatisticsFromServer)
                {
                    AddComponent<LobbyStatisticsData>(entity);
                }
                if(authoring.EstablishNetworkStreamInGameState)
                {
                    AddComponent<EstablishNetworkStreamInGameStateRpc>(entity);
                    /*
                    var connectLobbyRpcEntity = CreateAdditionalEntity(TransformUsageFlags.None, false, "Establish InGameState to Lobby");
                    AddComponent(connectLobbyRpcEntity, new EstablishNetworkStreamInGameState());
                    AddComponent(connectLobbyRpcEntity, new SendRpcCommandRequest()); */
                }
            }
        }
    }
}