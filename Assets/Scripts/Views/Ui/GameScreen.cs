using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace BubbleShooter
{
    public class GameScreen : ScreenBase
    {
        [field: Foldout("References")] [field: SerializeField] public Button Restart { get; private set; }
        [field: Foldout("References")] [field: SerializeField] public DynamicJoystick DynamicJoystick { get; private set; }
    }
}