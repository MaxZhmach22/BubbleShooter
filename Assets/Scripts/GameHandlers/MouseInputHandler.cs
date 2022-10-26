using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace BubbleShooter
{
    public class MouseInputHandler : IDisposable
    {
        private readonly Camera _camera = Camera.main;
        private readonly MainMenuScreen _mainMenuScreen;
        private readonly Transform _startTransform;
        private readonly Slider _slider;
        private readonly Material _selectionMaterial;
        private readonly CompositeDisposable _disposable = new ();
        public Dictionary<CellView, Material> SelectedCells = new ();

        public MouseInputHandler(MainMenuScreen mainMenuScreen, Transform startTransform, Slider slider,
            Material selectedMaterial)
        {
            _mainMenuScreen = mainMenuScreen;
            _startTransform = startTransform;
            _slider = slider;
            _selectionMaterial = selectedMaterial;
            Bind();
        }

        private void Bind()
        {
            Observable.EveryFixedUpdate()
                .Where(_ => Input.GetMouseButton(0))
                .Where(_ => !_mainMenuScreen.gameObject.activeSelf)
                .Subscribe(_ => Raycasting())
                .AddTo(_disposable);
        }

        public void ResetSelected()
        {
            if(SelectedCells.Count == 0) return;
                    
            foreach (var cell in SelectedCells)
            {
                cell.Key.Renderer.material = cell.Value;
            }
            SelectedCells.Clear();
        }

        private void Raycasting()
        {
            var colliders = new Collider[20];
            var screenPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            var gridPosition = new Vector3(screenPosition.x, screenPosition.y, _startTransform.position.z);
      
            var hits = Physics.OverlapSphereNonAlloc(gridPosition, 2f * _slider.value, colliders);

            if (hits == 0)
            {
                return;
            }

            foreach (var collider in colliders)
            {
                if(collider == null) continue;

                var cellView = collider.GetComponentInParent<CellView>();
                
                if(cellView != null && !SelectedCells.ContainsKey(cellView))
                {
                    SelectedCells.Add(cellView, cellView.Renderer.material);
                    cellView.Renderer.material = _selectionMaterial;
                }
            }
            DebugExtension.DebugWireSphere(gridPosition, Color.cyan, 3f * _slider.value, 0.1f);
        }

        public void Dispose()
        {
            _disposable.Clear();
        }
    }
}