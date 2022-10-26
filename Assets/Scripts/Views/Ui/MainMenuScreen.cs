using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;


namespace BubbleShooter
{
    public class MainMenuScreen : ScreenBase
    {
        [field: Foldout("References")] [field: SerializeField] public Button Play { get; private set; }
    }
}