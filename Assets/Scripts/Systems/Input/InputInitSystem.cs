using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;


namespace BubbleShooter
{
    public class InputInitSystem : IEcsInitSystem
    {
        private readonly EcsWorldInject _world = default;
        private readonly EcsCustomInject<DynamicJoystick> _joystick = default;
        private readonly EcsPoolInject<JoystickComponent> _joystickPool = default;
        
        public void Init(IEcsSystems systems)
        {
            if(_joystick.Value)
                _joystickPool.Value.Add(_world.Value.NewEntity());
        }
    }
}