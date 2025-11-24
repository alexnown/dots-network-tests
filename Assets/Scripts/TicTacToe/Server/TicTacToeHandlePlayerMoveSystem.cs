using com.tictactoe.common;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace com.tictactoe.server
{

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    partial struct TicTacToeHandlePlayerMoveSystem : ISystem
    {
        private EntityQuery _existingGames;
        private EntityArchetype _gameStateRpcArchetype;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TicTacToeGameProcessor>();
            state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<TicTacToeTryMakeMoveRpc, ReceiveRpcCommandRequest>().Build());
            _existingGames = SystemAPI.QueryBuilder().WithAll<TicTacToeServerGame, TicTacToeGameState>().WithNone<TicTacToeGameResult>().Build();
            state.RequireForUpdate(_existingGames);
            _gameStateRpcArchetype = state.EntityManager.CreateArchetype(
                ComponentType.ReadOnly<TicTacToeUpdateGameStateRpc>(),
                ComponentType.ReadOnly<SendRpcCommandRequest>());
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            var existingGamesEntities = _existingGames.ToEntityArray(Allocator.Temp);
            var existingGames = _existingGames.ToComponentDataArray<TicTacToeServerGame>(Allocator.Temp);
            foreach (var (moveRpc, request, entity) in SystemAPI.Query<TicTacToeTryMakeMoveRpc, ReceiveRpcCommandRequest>().WithEntityAccess())
            {
                ecb.DestroyEntity(entity);
                if (TrySearchExistingGame(request.SourceConnection, existingGames, existingGamesEntities, out Entity gameEntity, out var serverGame))
                {
                    int playerOrder = request.SourceConnection == serverGame.Player1 ? 0 : 1;
                    //Debug.Log($"Handle try make move {moveRpc.CellIndex} from client {request.SourceConnection} in game {gameEntity}");
                    var gameStateRW = SystemAPI.GetComponentRW<TicTacToeGameState>(gameEntity);
                    var gameState = gameStateRW.ValueRO;
                    if (gameState.Turn % 2 == playerOrder)
                    {
                        var gameCells = gameState.CellsPlayer1 | gameState.CellsPlayer2;
                        bool isFreeCell = ((gameCells >> moveRpc.CellIndex) & 1) == 0;
                        if (isFreeCell)
                        {
                            if (playerOrder > 0)
                            {
                                gameState.CellsPlayer2 = (ushort)(gameState.CellsPlayer2 | (1 << moveRpc.CellIndex));
                            }
                            else
                            {
                                gameState.CellsPlayer1 = (ushort)(gameState.CellsPlayer1 | (1 << moveRpc.CellIndex));
                            }
                            gameState.Turn = (byte)(gameState.Turn + 1);
                            gameStateRW.ValueRW = gameState;
                            SendUpdatedGameStateRpcToPlayers(gameState, serverGame.Player1, 0, ecb);
                            SendUpdatedGameStateRpcToPlayers(gameState, serverGame.Player2, 1, ecb);
                        }
                        else
                        {
                            Debug.Log($"CELL={moveRpc.CellIndex} NOT EMPTY: Handle try make move from client {request.SourceConnection} in game {gameEntity}");
                        }
                    }
                    else
                    {
                        Debug.Log($"NOT PLAYER TURN: Handle try make move {moveRpc.CellIndex} from client {request.SourceConnection} in game {gameEntity}");
                    }
                }
                else
                {
                    Debug.Log($"GAME NOT EXISTS: Handle try make move {moveRpc.CellIndex} from client {request.SourceConnection}");
                }
            }
        }

        private void SendUpdatedGameStateRpcToPlayers(TicTacToeGameState gameState, Entity player, int playerOrder, EntityCommandBuffer ecb)
        {
            var rpc = ecb.CreateEntity(_gameStateRpcArchetype);
            ecb.SetComponent(rpc, new SendRpcCommandRequest { TargetConnection = player });
            ecb.SetComponent(rpc, new TicTacToeUpdateGameStateRpc
            {
                CellsPlayer1 = gameState.CellsPlayer1,
                CellsPlayer2 = gameState.CellsPlayer2,
                Turn = gameState.Turn,
                PlayerOrder = (byte)playerOrder
            });
        }

        private bool TrySearchExistingGame(Entity playerConnection, NativeArray<TicTacToeServerGame> existingGames, NativeArray<Entity> gameEntities, out Entity gameEntity, out TicTacToeServerGame serverGame)
        {
            serverGame = default;
            for (int i = 0; i < existingGames.Length; i++)
            {
                var game = existingGames[i];
                if (game.Player1 == playerConnection || game.Player2 == playerConnection)
                {
                    serverGame = game;
                    gameEntity = gameEntities[i];
                    return true;
                }
            }
            gameEntity = Entity.Null;
            return false;
        }
    }
}
