using Unity.Entities;
using Unity.NetCode;

namespace com.tictactoe.common
{
    [GhostComponent]
    public struct TicTacToeStatistics : IComponentData
    {
        [GhostField] public int ActiveGames;
        [GhostField] public int AwaitingOpponent;
    }
}