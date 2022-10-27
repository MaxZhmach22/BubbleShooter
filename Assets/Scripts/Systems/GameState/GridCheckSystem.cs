using System.Linq;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;


namespace BubbleShooter
{
    public class GridCheckSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject _world = default;
        private readonly EcsCustomInject<GridCreator> _gridCreator = default;
        private readonly EcsCustomInject<LevelSetting> _levelSettings = default;
        private readonly EcsPoolInject<OpenCellRequest> _requestPool = default;
        private readonly EcsPoolInject<PlayerComponent> _playerPool = default;
        private readonly EcsFilterInject<Inc<PlayerComponent, OpenCellRequest>> _filter = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var cellComp = ref _requestPool.Value.Get(entity);
                ref var playerComp = ref _playerPool.Value.Get(entity);
                
                var position = cellComp.HitPosition;

                var cellInGrid = _gridCreator.Value.CellsList
                    .Where(x => !x.Renderer.enabled)
                    .OrderBy(x => Vector3.Distance(position, x.transform.position))
                    .First();

                if (cellComp.HittedCellView.ColorTypes == playerComp.ColorType)
                {
                    cellComp.HittedCellView.Blow(0);
                }
                else
                {
                    cellInGrid.Collider.enabled = true;
                    cellInGrid.Renderer.enabled = true;
                    cellInGrid.ColorTypes = playerComp.ColorType;
                }
                
                _levelSettings.Value.Line.SetPositions(new Vector3[0]);
                _requestPool.Value.Del(entity);
            }

            if (_gridCreator.Value.CellsList.All(x => x.Renderer.enabled == false))
            {
                _world.Value.GetPool<WinRequest>().Add(_world.Value.NewEntity());
            }
        }
    }
}