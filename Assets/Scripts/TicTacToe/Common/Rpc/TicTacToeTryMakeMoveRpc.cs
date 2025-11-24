using Unity.NetCode;

namespace com.tictactoe.common
{
    public struct TicTacToeTryMakeMoveRpc : IRpcCommand
    {
        public int CellIndex;
    }
}