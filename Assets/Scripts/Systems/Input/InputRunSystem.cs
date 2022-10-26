using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;


namespace BubbleShooter
{
    public class InputRunSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject _world = default;
        private readonly EcsCustomInject<DynamicJoystick> _joystick = default;
        private readonly EcsPoolInject<JoystickComponent> _joystickPool = default;
        private readonly EcsFilterInject<Inc<JoystickComponent>> _filter = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var joystickComp = ref _joystickPool.Value.Get(entity);
                joystickComp.Horizontal = _joystick.Value.Horizontal;
                joystickComp.Vertical = _joystick.Value.Vertical;
            }
        }
    }
}