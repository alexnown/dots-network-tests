using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace com.testnet.common
{
    public struct PlayerGhostData : IComponentData
    {
        [GhostField] public float PassedPath;
    }

    public struct PlayerInputData : IInputComponentData
    {
        public float2 MoveDirection;
        public InputEvent Shoot;
    }

    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    partial struct ApplyPlayerInputSystem : ISystem
    {

        public void OnUpdate(ref SystemState state)
        {
           foreach( var (inputData, ghostData, localTransform) in  
                SystemAPI.Query<RefRO<PlayerInputData>, RefRW<PlayerGhostData>, RefRW<LocalTransform>> ().WithAll<Simulate>())
           {
                float moveSpeed = 10f;
                float3 moveVector = new float3(inputData.ValueRO.MoveDirection.x, 0, inputData.ValueRO.MoveDirection.y);
                if(math.lengthsq(moveVector) > 0)
                {
                    float3 passedVector = math.normalize(moveVector) * moveSpeed * SystemAPI.Time.DeltaTime;
                    localTransform.ValueRW.Position += passedVector;
                    if(state.World.IsServer())
                    {
                        ghostData.ValueRW.PassedPath += math.length(passedVector);
                    }
                }
           }
        }
    }
}