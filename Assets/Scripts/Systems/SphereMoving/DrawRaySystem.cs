using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.VisualScripting;
using UnityEngine;


namespace BubbleShooter
{
    public class DrawRaySystem : IEcsRunSystem
    {
        private readonly EcsWorldInject _world = default;
        private readonly EcsCustomInject<LevelSetting> _levelSettings = default;
        private readonly EcsPoolInject<JoystickComponent> _joystickPool = default;
        private readonly EcsPoolInject<WayPointsComponent> _wayPointsPool = default;
        private readonly EcsPoolInject<MouseButtonUpRequest> _mouseUpRequest = default;
        private readonly EcsFilterInject<Inc<JoystickComponent, MouseButtonDownComponent>> _filterMouseDown = default;
        private readonly EcsFilterInject<Inc<JoystickComponent, MouseButtonUpRequest>> _filterMouseUp = default;
        private readonly EcsFilterInject<Inc<PlayerComponent>, Exc<WayPointsComponent>> _filterPlayer = default;
        private Vector3[] _hitPointPositions;
        private CellView _hittedCell;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filterMouseDown.Value)
            {
                ref var joystickComp = ref _joystickPool.Value.Get(entity);
                DrawRay(joystickComp);
            }
            
            foreach (var entity in _filterMouseUp.Value)
            {
                foreach (var player in _filterPlayer.Value)
                {
                    ref var wayPointsComp = ref _wayPointsPool.Value.Add(player);
                    wayPointsComp.WayPointsPositions = _hitPointPositions;

                    if (_hittedCell)
                        wayPointsComp.HittedCell = _hittedCell;
                    else
                        wayPointsComp.HittedCell = null;
                }
                
                if (_mouseUpRequest.Value.Has(entity))
                {
                    _mouseUpRequest.Value.Del(entity);
                }
            }
        }

        private void DrawRay(JoystickComponent joystickComp)
        {
            _hittedCell = null;
            _hitPointPositions = new Vector3[10];
            var direction = new Vector3(joystickComp.Horizontal * -1, joystickComp.Vertical * -1, 0);
            var depth = 0;
            RayCast( Vector3.zero, direction, depth);
            var lineRendererPosition = new List<Vector3> {Vector3.zero};
            lineRendererPosition.AddRange(_hitPointPositions);
            _levelSettings.Value.Line.SetPositions(lineRendererPosition.ToArray());
        }

        private void RayCast(Vector3 startPos, Vector3 direction, int depth)
        {
            if(depth > 9) return;
            
            if (Physics.Raycast(startPos, direction, out var hitInfo,
                    20, _levelSettings.Value.RayMask))
            {
                DebugExtension.DebugPoint(hitInfo.point, Color.magenta, 1, 0.1f);
                _hitPointPositions[depth] = hitInfo.point;
                depth++;
                if (hitInfo.transform.parent.TryGetComponent<CellView>(out var cellView))
                {
                    _hittedCell = cellView;
                    return;
                }

                if (hitInfo.collider.gameObject.name == "Up")
                {
                    return;
                }
                
                RayCast(hitInfo.point,Vector3.Reflect(direction, hitInfo.normal), depth);
            }
         
            DebugExtension.DebugArrow(startPos, direction * 20, Color.cyan, 0.1f);
        }
    }
}