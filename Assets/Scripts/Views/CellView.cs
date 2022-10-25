using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;


namespace BubbleShooter
{
    public class CellView : MonoBehaviour
    {
        [field: BoxGroup("Settings")] [field: SerializeField] public ColorTypes ColorTypes { get; private set; }
        [field: BoxGroup("Settings")] [field: SerializeField] public List<CellView> NeighboursList { get; set; }

        [Button("Select Neighbours")]
        private void SelectNeighbours()
        {
            if (NeighboursList.Count != 0)
            {
                Selection.objects = NeighboursList.Select(x => x.gameObject).ToArray();
                NeighboursList.ForEach(x => DebugExtension.DebugWireSphere(x.transform.position, Color.magenta, 0.3f, 3f));
                
            }
        }
        
    }
}