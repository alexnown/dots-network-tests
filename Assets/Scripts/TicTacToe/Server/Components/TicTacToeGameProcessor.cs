using Unity.Entities;

namespace com.tictactoe.server
{
    public struct TicTacToeGameProcessor : IComponentData 
    {
        public Entity GamePrefab;
    }
}
