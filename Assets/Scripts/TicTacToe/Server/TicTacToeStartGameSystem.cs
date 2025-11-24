using com.tictactoe.common;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace com.tictactoe.server
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    partial struct TicTacToeStartGameSystem : ISystem
    {
        private EntityQuery _existingGamesQuery;
        private EntityArchetype _gameStateRpcArchetype;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TicTacToeGameProcessor>();
            state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<TicTacToeStartGameRpc, ReceiveRpcCommandRequest>().Build());
            _existingGamesQuery = SystemAPI.QueryBuilder().WithAll<TicTacToeServerGame>().WithNone<TicTacToeGameResult>().Build();
            _gameStateRpcArchetype = state.EntityManager.CreateArchetype(
                ComponentType.ReadOnly<TicTacToeUpdateGameStateRpc>(),
                ComponentType.ReadOnly<SendRpcCommandRequest>());
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            var tictactoeProcessor = SystemAPI.GetSingleton<TicTacToeGameProcessor>();
            var waitingList = SystemAPI.GetSingletonBuffer<TicTacToePlayersInWaitList>(false);
            for (int i = 0; i < waitingList.Length; i++)
            {
                if (!SystemAPI.Exists(waitingList[i].Connection))
                {
                    waitingList.RemoveAtSwapBack(i);
                    i--;
                }
            }
            var existingGames = _existingGamesQuery.ToComponentDataArray<TicTacToeServerGame>(Allocator.Temp);
            foreach (var (request, entity) in SystemAPI.Query<ReceiveRpcCommandRequest>().WithAll<TicTacToeStartGameRpc>().WithEntityAccess())
            {
                ecb.DestroyEntity(entity);
                var playerEntity = request.SourceConnection;
                if (CheckPlayerAlreadyInWaitingList(playerEntity, waitingList))
                {
                    Debug.Log($"Player connection {playerEntity} already in waiting list");
                }
                else if (CheckPlayerAlreadyInExistingGame(playerEntity, existingGames))
                {
                    Debug.Log($"Player connection {playerEntity} already in active game");
                }
                else
                {
                    waitingList.Add(new TicTacToePlayersInWaitList { Connection = playerEntity });
                }
            }
            existingGames.Dispose();
            while (waitingList.Length >= 2)
            {
                var player1 = waitingList[0].Connection;
                var player2 = waitingList[1].Connection;

                var gameSettings = new TicTacToeServerGame
                {
                    Player1 = player1,
                    Player2 = player2
                };
                waitingList.RemoveRange(0, 2);
                var gameInstance = ecb.Instantiate(tictactoeProcessor.GamePrefab);
                ecb.SetComponent(gameInstance, gameSettings);
                SendGameStateRpcToPlayer(player1, true, _gameStateRpcArchetype, ecb);
                SendGameStateRpcToPlayer(player2, false, _gameStateRpcArchetype, ecb);
            }
        }

        private void SendGameStateRpcToPlayer(Entity playerEntity, bool isFirstPlayer, EntityArchetype requestArchetype, EntityCommandBuffer ecb)
        {
            var request = ecb.CreateEntity(requestArchetype);
            ecb.SetComponent(request, new TicTacToeUpdateGameStateRpc { PlayerOrder = (byte)(isFirstPlayer ? 0 : 1) });
            ecb.SetComponent(request, new SendRpcCommandRequest { TargetConnection = playerEntity });
        }

        private bool CheckPlayerAlreadyInWaitingList(Entity playerConnection, DynamicBuffer<TicTacToePlayersInWaitList> waitingList)
        {
            for (int i = 0; i < waitingList.Length; i++)
            {
                if (waitingList[i].Connection == playerConnection)
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckPlayerAlreadyInExistingGame(Entity playerConnection, NativeArray<TicTacToeServerGame> existingGames)
        {
            for (int i = 0; i < existingGames.Length; i++)
            {
                var existingGame = existingGames[i];
                if (existingGame.Player1 == playerConnection || existingGame.Player2 == playerConnection)
                {
                    return true;
                }
            }
            return false;
        }
    }
}