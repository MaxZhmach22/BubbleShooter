using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;


namespace BubbleShooter
{
    public class WinScreen : MonoBehaviour
    {
        [field: Foldout("References")] [field: SerializeField] public Button Exit { get; private set; }
    }
}