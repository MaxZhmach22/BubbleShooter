using System;
using System.Collections.Generic;
using System.Linq;
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
        

        private void Awake()
        {
            if(!Renderer) Renderer = GetComponentInChildren<Renderer>();
            if(!Collider) Collider = GetComponentInChildren<Collider>();

            this.ObserveEveryValueChanged(x => x.ColorTypes)
                .Subscribe(x => SwitchColor(x))
                .AddTo(this);
        }

        public void Init(Renderer renderer, Collider collider, List<Material> materials)
        {
            Renderer = renderer;
            Collider = collider;
            Materials = materials;
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
            if (NeighboursList.Count != 0)
            {
                Selection.objects = NeighboursList.Select(x => x.gameObject).ToArray();
                NeighboursList.ForEach(x => DebugExtension.DebugWireSphere(x.transform.position, Color.magenta, 0.3f, 3f));
            }
        }
        
        private void OnValidate()
        {
            if(Materials != null)
                SwitchColor(ColorTypes);
        }
    }
}