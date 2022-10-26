using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace BubbleShooter
{
    public class PlayerInitSystem : IEcsInitSystem
    {
        private readonly EcsWorldInject _world = default;
        private readonly EcsPoolInject<PlayerComponent> _pool = default;

        public void Init(IEcsSystems systems)
        {
            _pool.Value.Add(_world.Value.NewEntity());
        }
        
    }
}