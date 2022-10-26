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
        private readonly EcsFilterInject<Inc<PlayerComponent, WayPointsComponent>> _filter = default;
        private GameObject _player;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var playerComponent = ref _playerComp.Value.Get(entity);
                ref var wayPointsComp = ref _wayPointsPool.Value.Get(entity);
                
                _player = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                var filtredWayPoints = wayPointsComp.WayPointsPositions.Where(x => x != Vector3.zero).ToArray();
                _player.transform.DOPath(filtredWayPoints, filtredWayPoints.Length / 2f)
                    .OnWaypointChange(Shake)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => _player.gameObject.SetActive(false));

                _wayPointsPool.Value.Del(entity);
            }
        }

        private void Shake(int value)
        {
            _player.transform.DOShakeScale(0.5f, 0.5f, 10, 10, true, ShakeRandomnessMode.Harmonic);
        }
    }
}