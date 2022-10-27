using NaughtyAttributes;
using UnityEngine;

namespace BubbleShooter
{
    public class LevelSetting : MonoBehaviour
    {
        [field: BoxGroup("Settings")] [field: SerializeField] public LayerMask RayMask { get; private set; }
        [field: BoxGroup("Settings")] [field: SerializeField] public LineRenderer Line { get; private set; }
    }
}