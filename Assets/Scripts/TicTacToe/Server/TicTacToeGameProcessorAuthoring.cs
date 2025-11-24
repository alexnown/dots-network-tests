using Unity.Entities;
using UnityEngine;

namespace com.tictactoe.server
{
    public class TicTacToeGameProcessorAuthoring : MonoBehaviour
    {
        public GameObject GameAuthoring;

        public class Baker : Baker<TicTacToeGameProcessorAuthoring>
        {
            public override void Bake(TicTacToeGameProcessorAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                var gamePrefab = GetEntity(authoring.GameAuthoring, TransformUsageFlags.None);
                AddComponent(entity, new TicTacToeGameProcessor { GamePrefab = gamePrefab });
                AddComponent<TicTacToePlayersInWaitList>(entity);
            }
        }
    }
}