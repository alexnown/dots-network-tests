using com.testnet.common;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace com.testnet.client
{
    public struct CreateNewPlayer : IComponentData { }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct SendCreatePlayerRpcToServerSystem : ISystem
    {
        private EntityQuery _createPlayerQuery;
        private EntityQuery _pendingNetworkIdQuery;

        public void OnCreate(ref SystemState state)
        {
            _pendingNetworkIdQuery = SystemAPI.QueryBuilder().WithAll<NetworkId>().WithNone<NetworkStreamInGame>().Build();
            _createPlayerQuery = state.GetEntityQuery(ComponentType.ReadWrite<CreateNewPlayer>());
            state.RequireForUpdate(_createPlayerQuery);
            state.RequireForUpdate(_pendingNetworkIdQuery);
        }

        public void OnUpdate(ref SystemState state)
        {
            Debug.Log("executed " + typeof(SendCreatePlayerRpcToServerSystem));
            return;
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            ecb.DestroyEntity(_createPlayerQuery, EntityQueryCaptureMode.AtPlayback);
            var pendingNetworkIds = _pendingNetworkIdQuery.ToEntityArray(Allocator.Temp);
            foreach (var pendingNetworkEntity in pendingNetworkIds)
            {
                ecb.AddComponent<NetworkStreamInGame>(pendingNetworkEntity);
            }
            var requestEntity = ecb.CreateEntity();
            ecb.AddComponent(requestEntity, new CreatePlayerRpc());
            ecb.AddComponent(requestEntity, new SendRpcCommandRequest());
            ecb.Playback(state.EntityManager);
        }
    }
}
