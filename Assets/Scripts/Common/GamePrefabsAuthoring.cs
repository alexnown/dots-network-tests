using Unity.Entities;
using UnityEngine;

namespace com.testnet.common
{
    public struct GamePrefabs : IComponentData
    {
        public Entity Player;
        public Entity Bullet;
    }

    public class GamePrefabsAuthoring : MonoBehaviour
    {
        public GameObject PlayerPrefab;
        public GameObject BulletPrefab;

        public class Baker : Baker<GamePrefabsAuthoring>
        {
            public override void Bake(GamePrefabsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new GamePrefabs
                {
                    Player = GetEntity(authoring.PlayerPrefab, TransformUsageFlags.Dynamic),
                    Bullet = GetEntity(authoring.BulletPrefab, TransformUsageFlags.Dynamic),
                });
            }
        }
    }
}