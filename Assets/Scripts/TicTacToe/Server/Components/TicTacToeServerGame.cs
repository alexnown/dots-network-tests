using Unity.Entities;

namespace com.tictactoe.server
{
    public struct TicTacToeServerGame : IComponentData
    {
        public Entity Player1;
        public Entity Player2;
    }
}