using System;
using System.Linq;
using DG.Tweening;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace BubbleShooter
{
    public class PlayerMovement :  IEcsRunSystem
    {
        private readonly EcsWorldInject _world = default;
        private readonly EcsPoolInject<WayPointsComponent> _wayPointsPool = default;
        private readonly EcsPoolInject<PlayerComponent> _playerComp = default;
        private readonly EcsPoolInject<ChangeColorRequest> _colorPool = default;
        private readonly EcsPoolInject<OpenCellRequest> _openCellRequest = default;
        private readonly EcsFilterInject<Inc<PlayerComponent, WayPointsComponent>> _filter = default;
        private GameObject _player;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var playerComponent = ref _playerComp.Value.Get(entity);
                ref var wayPointsComp = ref _wayPointsPool.Value.Get(entity);
                
                var filtredWayPoints = wayPointsComp.WayPointsPositions.Where(x => x != Vector3.zero).ToArray();
                
                _player = playerComponent.PlayerSphere;
                var hittedCell = wayPointsComp.HittedCell;
                
                _player.transform.DOPath(filtredWayPoints, filtredWayPoints.Length / 2f)
                    .OnWaypointChange(Shake)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        _player.transform.position = Vector3.zero;
                        _colorPool.Value.Add(entity);

                        if (hittedCell)
                        {
                            ref var openCell = ref _openCellRequest.Value.Add(entity);
                            openCell.HitPosition = filtredWayPoints.Last();
                            openCell.HittedCellView = hittedCell;
                        }
                    });

                _wayPointsPool.Value.Del(entity);
            }
        }

        private void Shake(int value)
        {
            _player.transform.DOShakeScale(0.5f, 0.5f, 10, 10, true, ShakeRandomnessMode.Harmonic);
        }
    }
}