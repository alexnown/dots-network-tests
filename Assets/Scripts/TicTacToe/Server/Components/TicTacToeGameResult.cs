using Unity.Entities;

namespace com.tictactoe.common
{
    public struct TicTacToeGameResult : IComponentData
    {
        public ushort Winner;
        public byte PlayerSurrendered;
        public byte PlayerDisconnected;
    }
}