using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace com.testnet.common
{
    public class PlayerAuthoring : MonoBehaviour
    {
        public class Baker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<PlayerInputData>(entity);
                AddComponent<PlayerGhostData>(entity);
            }
        }
    }
}
