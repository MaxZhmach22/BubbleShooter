using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UniRx;
using UnityEditor;
using UnityEngine;


namespace BubbleShooter
{
    [SelectionBase]
    public class CellView : MonoBehaviour
    {
        [field: BoxGroup("Settings")] [field: SerializeField] public ColorTypes ColorTypes { get; set; }
        [field: BoxGroup("Settings")] [field: SerializeField] public List<CellView> NeighboursList { get; set; }
        [field: BoxGroup("Settings")] [field: SerializeField] public Renderer Renderer { get; private set; }
        [field: BoxGroup("Settings")] [field: SerializeField] public Collider Collider { get; private set; }
        [field: Foldout("References")] [field: SerializeField] public List<Material> Materials { get; private set; }

        private bool _isBlowed;
        private Vector3 _defaultScale;
        
        private void Awake()
        {
            if(!Renderer) Renderer = GetComponentInChildren<Renderer>();
            if(!Collider) Collider = GetComponentInChildren<Collider>();

            this.ObserveEveryValueChanged(x => x.ColorTypes)
                .Subscribe(x => SwitchColor(x))
                .AddTo(this);
            
#if UNITY_EDITOR
            Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.B))
                .Where(_ => gameObject == Selection.gameObjects[0])
                .Subscribe(_ => Blow(0))
                .AddTo(this);
#endif
        }

        public void Init(Renderer renderer, Collider collider, List<Material> materials, Vector3 localSphereScale)
        {
            Renderer = renderer;
            Collider = collider;
            Materials = materials;
            _defaultScale = renderer.transform.localScale;
        }

        private void SwitchColor(ColorTypes colorTypes)
        {
            switch (colorTypes)
            {
                case ColorTypes.None:
                    Renderer.material = Materials.First(x => x.name.EndsWith("None"));
                    break;
                case ColorTypes.Red:
                    Renderer.material = Materials.First(x => x.name.EndsWith("Red"));
                    break;
                case ColorTypes.Green:
                    Renderer.material = Materials.First(x => x.name.EndsWith("Green"));
                    break;
                case ColorTypes.Blue:
                    Renderer.material = Materials.First(x => x.name.EndsWith("Blue"));
                    break;
                case ColorTypes.Yellow:
                    Renderer.material = Materials.First(x => x.name.EndsWith("Yellow"));
                    break;
            }
        }

        [Button("Select Neighbours")]
        private void SelectNeighbours()
        {
#if UNITY_EDITOR
            if (NeighboursList.Count != 0)
            {
                Selection.objects = NeighboursList.Select(x => x.gameObject).ToArray();
                NeighboursList.ForEach(x => DebugExtension.DebugWireSphere(x.transform.position, Color.magenta, 0.3f, 3f));
            }
#endif
        }

        //[Button("Blow!")]
        public async void Blow(float timeShift)
        {
            if(_isBlowed || ColorTypes == ColorTypes.None) return;

            var nextTime = timeShift + 0.05f;
            _isBlowed = true;
            foreach (var cell in NeighboursList)
            {
                if (cell.ColorTypes == ColorTypes)
                {
                    cell.Blow(nextTime);
                }
            }

            await UniTask.Delay(TimeSpan.FromSeconds(timeShift),
                cancellationToken: this.GetCancellationTokenOnDestroy());
            Renderer.gameObject.transform.DOScale(Vector3.one * 1.2f, 1f)
                .SetEase(Ease.OutElastic)
                .OnComplete(() =>
                {
                    Renderer.enabled = false;
                    Collider.enabled = false;
                });
        }

        public void Reset()
        {
            if(!Renderer) return;
            
            _isBlowed = false;
            gameObject.SetActive(true);
        }


        private void OnValidate()
        {
            if(Materials != null)
                SwitchColor(ColorTypes);
        }
    }
}