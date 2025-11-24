using Unity.NetCode;

namespace com.tictactoe.common
{
    public struct TicTacToeUpdateGameStateRpc : IRpcCommand
    {
        public ushort CellsPlayer1;
        public ushort CellsPlayer2;
        public byte PlayerOrder;  //0 - x, 1 - o
        public byte Turn;
        public byte GameResultFlags;
    }
}