using com.testnet.common;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace com.testnet.client
{
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial struct UpdatePlayerInputSystem : ISystem
    {
        //private InputSystem_Actions _inputs;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerInputData>();
            state.RequireForUpdate<NetworkStreamInGame>();
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach(var input in SystemAPI.Query<RefRW<PlayerInputData>>().WithAll<GhostOwnerIsLocal>())
            {
                float2 inputVector = new float2();
                if(Input.GetKey(KeyCode.W))
                {
                    inputVector.y += 1;
                } 
                if (Input.GetKey(KeyCode.S))
                {
                    inputVector.y -= 1;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    inputVector.x -= 1;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    inputVector.x += 1;
                }
                input.ValueRW.MoveDirection = inputVector;

                if(Input.GetKeyDown(KeyCode.Space))
                {
                    input.ValueRW.Shoot.Set();
                } 
                else
                {
                    input.ValueRW.Shoot = default;
                }
            }
        }
    }
}