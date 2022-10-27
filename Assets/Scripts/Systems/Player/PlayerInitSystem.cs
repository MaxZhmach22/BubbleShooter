using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BubbleShooter
{
    public class PlayerInitSystem : IEcsInitSystem
    {
        private readonly EcsWorldInject _world = default;
        private readonly EcsPoolInject<PlayerComponent> _pool = default;

        public void Init(IEcsSystems systems)
        {
            var entity = _world.Value.NewEntity();
            ref var playerComp = ref _pool.Value.Add(entity);
            playerComp.PlayerSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            playerComp.PlayerSphere.transform.position = Vector3.zero;
            playerComp.Renderer = playerComp.PlayerSphere.GetComponent<Renderer>();
            playerComp.ColorType = (ColorTypes)Random.Range(0, 5);
            _world.Value.GetPool<ChangeColorRequest>().Add(entity);
        }
        
    }
}