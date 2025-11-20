using Unity.Entities;
using UnityEngine;
namespace com.testnet.common
{
    public class BulletAuthoring : MonoBehaviour
    {
        public float Speed;
        public float LifeTime;

        public class Baker : Baker<BulletAuthoring>
        {
            public override void Bake(BulletAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Bullet { Speed = authoring.Speed , LifeTimer = authoring.LifeTime });
            }
        }
    }
}