using Unity.Entities;
using UnityEngine;

namespace com.testnet.client
{
    public class ClientSceneAuthoring : MonoBehaviour
    {
        public class Baker : Baker<ClientSceneAuthoring>
        {
            public override void Bake(ClientSceneAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new CreateNewPlayer());
            }
        }
    }
}