using Unity.Entities;

namespace com.tictactoe.server
{
    public struct TicTacToeGameState : IComponentData
    {
        public ushort CellsPlayer1;
        public ushort CellsPlayer2;
        public byte Turn;
    }
}