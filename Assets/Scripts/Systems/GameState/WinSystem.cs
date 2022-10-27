using GameHandlers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;


namespace BubbleShooter
{
    public class WinSystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<GameUiHandler> _gameUi = default;
        private readonly EcsPoolInject<WinRequest> _pool = default;
        private readonly EcsFilterInject<Inc<WinRequest>> _filter = default;
        
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                _gameUi.Value.WinScreen.gameObject.SetActive(true);
                _gameUi.Value.GameScreen.gameObject.SetActive(false);
                _pool.Value.Del(entity);
            }
        }
    }
}