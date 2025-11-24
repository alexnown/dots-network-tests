using com.tictactoe.common;
using System;
using Unity.Entities;
using Unity.NetCode;

namespace com.tictactoe.client
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial class TicTacToeHandleUpdateGameStateSystem : SystemBase
    {
        public static Action<TicTacToeUpdateGameStateRpc> OnHandleGameState;

        protected override void OnUpdate()
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(this.World.Unmanaged);
            foreach(var (stateData, entity) in SystemAPI.Query<TicTacToeUpdateGameStateRpc>().WithAll<ReceiveRpcCommandRequest>().WithEntityAccess())
            {
                ecb.DestroyEntity(entity);
                OnHandleGameState?.Invoke(stateData);
            }
        }
    }
}