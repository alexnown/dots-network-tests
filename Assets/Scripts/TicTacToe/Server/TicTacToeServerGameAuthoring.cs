using Unity.Entities;
using UnityEngine;

namespace com.tictactoe.server
{
    public class TicTacToeServerGameAuthoring : MonoBehaviour
    {
        public class Baker : Baker<TicTacToeServerGameAuthoring>
        {
            public override void Bake(TicTacToeServerGameAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<TicTacToeServerGame>(entity);
                AddComponent<TicTacToeGameState>(entity);
                AddComponent<TicTacToeGameTimer>(entity);
            }
        }
    }
}