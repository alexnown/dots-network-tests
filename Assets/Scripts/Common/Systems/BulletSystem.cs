using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

namespace com.testnet.common
{
    public struct Bullet: IComponentData
    {
        public float Speed;
        public float LifeTimer;
    }

    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct BulletSystem : ISystem
    {

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach(var (transform, bullet) in
                SystemAPI.Query<RefRW<LocalTransform>, RefRO<Bullet>>().WithAll<Simulate>())
            {
                transform.ValueRW.Position += new Unity.Mathematics.float3(bullet.ValueRO.Speed * SystemAPI.Time.DeltaTime, 0, 0);
                
            }
            if (state.World.IsServer())
            {
                var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
                foreach (var (bullet, entity) in SystemAPI.Query<RefRW<Bullet>>().WithAll<Simulate>().WithEntityAccess())
                {
                    bullet.ValueRW.LifeTimer -= SystemAPI.Time.DeltaTime;
                    if(bullet.ValueRW.LifeTimer <=0)
                    {
                        ecb.DestroyEntity(entity);
                    }
                }
                ecb.Playback(state.EntityManager);
            }
        }
    }
}