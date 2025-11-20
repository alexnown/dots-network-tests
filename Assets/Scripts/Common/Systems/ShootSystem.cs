
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace com.testnet.common
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    partial struct ShootSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<PlayerInputData>();
            state.RequireForUpdate<GamePrefabs>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var netTime = SystemAPI.GetSingleton<NetworkTime>();
            var prefabs = SystemAPI.GetSingleton<GamePrefabs>();
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach(var (inputData, transform, ghostOwner) in SystemAPI.Query<PlayerInputData, LocalTransform, GhostOwner>().WithAll<Simulate>())
            {
                if(netTime.IsFirstTimeFullyPredictingTick && inputData.Shoot.IsSet)
                {
                    //Debug.Log(state.World.Name + " shoot " +netTime);
                    var bullet = ecb.Instantiate(prefabs.Bullet);
                    ecb.SetComponent(bullet, new GhostOwner { NetworkId = ghostOwner.NetworkId });
                    //todo: if bullet scale = 1, just use ecb.SetComponent(bullet, LocalTransform.FromPosition(transform.Position));
                    var bulletTransform = SystemAPI.GetComponent<LocalTransform>(prefabs.Bullet);
                    bulletTransform.Position = transform.Position;
                    ecb.SetComponent(bullet, bulletTransform);
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}
