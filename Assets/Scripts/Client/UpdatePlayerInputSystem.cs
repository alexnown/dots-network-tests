using com.testnet.common;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace com.testnet.client
{
    //[UpdateInGroup(typeof(GhostInputSystemGroup))]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class UpdatePlayerInputSystem : SystemBase
    {
        private InputSystem_Actions _inputs;

        protected override void OnCreate()
        {
            RequireForUpdate<PlayerInputData>();
            RequireForUpdate<NetworkStreamInGame>();
            _inputs = new InputSystem_Actions();
        }

        protected override void OnStartRunning()
        {
            _inputs.Enable();
        }

        protected override void OnStopRunning()
        {
            _inputs.Disable();
        }

        protected override void OnUpdate()
        {
            var moveInput = _inputs.Player.Move.ReadValue<Vector2>();
            bool isShoot = _inputs.Player.Attack.WasPressedThisFrame();
            foreach (var input in SystemAPI.Query<RefRW<PlayerInputData>>().WithAll<GhostOwnerIsLocal>())
            {
                input.ValueRW.MoveDirection = new float2(moveInput.x, moveInput.y);
                if (isShoot)
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