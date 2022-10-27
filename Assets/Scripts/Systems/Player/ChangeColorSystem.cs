using System.Linq;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;


namespace BubbleShooter
{
    public class ChangeColorSystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<GridCreator> _gridCreator = default;
        private readonly EcsWorldInject _world = default;
        private readonly EcsPoolInject<ChangeColorRequest> _requestPool = default;
        private readonly EcsFilterInject<Inc<PlayerComponent, ChangeColorRequest>> _filter = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var playerComp = ref _world.Value.GetPool<PlayerComponent>().Get(entity);
                var rnd = Random.Range(1, 5);
                playerComp.ColorType = (ColorTypes) rnd;
                SwitchColor(playerComp.ColorType, playerComp.Renderer, _gridCreator.Value);
                _requestPool.Value.Del(entity);
            }
        }
        
        private void SwitchColor(ColorTypes colorTypes, Renderer renderer, GridCreator gridCreator)
        {
            switch (colorTypes)
            {
                case ColorTypes.Red:
                    renderer.material = gridCreator.Materials.First(x => x.name.EndsWith("Red"));
                    break;
                case ColorTypes.Green:
                    renderer.material = gridCreator.Materials.First(x => x.name.EndsWith("Green"));
                    break;
                case ColorTypes.Blue:
                    renderer.material = gridCreator.Materials.First(x => x.name.EndsWith("Blue"));
                    break;
                case ColorTypes.Yellow:
                    renderer.material = gridCreator.Materials.First(x => x.name.EndsWith("Yellow"));
                    break;
            }
        }

        
    }
}