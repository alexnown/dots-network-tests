using Unity.Entities;

namespace com.tictactoe.server
{
    public struct TicTacToePlayersInWaitList : IBufferElementData
    {
        public Entity Connection;
    }
}