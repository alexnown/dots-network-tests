using com.testnet.common;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace com.testnet.server
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct HandleCreatePlayerRpcSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            var builder = new EntityQueryBuilder(Allocator.Temp).WithAll<CreatePlayerRpc, ReceiveRpcCommandRequest>();
            state.RequireForUpdate(state.GetEntityQuery(builder));
            state.RequireForUpdate<GamePrefabs>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var playerPrefab = SystemAPI.GetSingleton<GamePrefabs>().Player;
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach(var (request, entity) in SystemAPI.Query<ReceiveRpcCommandRequest>().WithAll<CreatePlayerRpc>().WithEntityAccess())
            {
                ecb.DestroyEntity(entity);
                ecb.AddComponent<NetworkStreamInGame>(request.SourceConnection);
                var clientId = SystemAPI.GetComponent<NetworkId>(request.SourceConnection).Value;
                Debug.Log("Server received create player request from clientId="+clientId);

                var playerEntity = ecb.Instantiate(playerPrefab);
                var position = LocalTransform.FromPosition(new float3(UnityEngine.Random.Range(-3,3), 0.5f, 0));
                ecb.SetComponent(playerEntity, position);
                ecb.AddComponent(playerEntity, new GhostOwner { NetworkId = clientId });

                if(state.World.IsServer())
                {
                    ecb.AppendToBuffer(request.SourceConnection, new LinkedEntityGroup { Value = playerEntity });
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}