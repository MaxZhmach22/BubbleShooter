using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;


namespace BubbleShooter
{
    public class MouseButtonInputSystem : IEcsRunSystem
    {
        private readonly EcsPoolInject<MouseButtonDownComponent> _pool = default;
        private readonly EcsPoolInject<MouseButtonUpRequest> _upRequestPool = default;
        private readonly EcsFilterInject<Inc<JoystickComponent>> _filter = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (!_pool.Value.Has(entity))
                    {
                        _pool.Value.Add(entity);
                    }
                }
                
                if (Input.GetMouseButtonUp(0))
                {
                    if (_pool.Value.Has(entity))
                    {
                        _pool.Value.Del(entity);
                    }

                    if (!_upRequestPool.Value.Has(entity))
                    {
                        _upRequestPool.Value.Add(entity);
                    }
                    
                }
            }
        }
    }
}