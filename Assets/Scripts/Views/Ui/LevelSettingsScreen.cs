using Cinemachine;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;


namespace BubbleShooter
{
    public class LevelSettingsScreen : ScreenBase
    {
        [field: Foldout("References")] [field: SerializeField] public Button Randomize { get; private set; }
        [field: Foldout("References")] [field: SerializeField] public Button Color { get; private set; }
        [field: Foldout("References")] [field: SerializeField] public Button Reset { get; private set; }
        [field: Foldout("References")] [field: SerializeField] public Button Play { get; private set; }
        [field: Foldout("References")] [field: SerializeField] public Button Apply { get; private set; }
        [field: Foldout("References")] [field: SerializeField] public Slider RadiusSlider { get; private set; }
        [field: Foldout("References")] [field: SerializeField] public CinemachineVirtualCamera LevelSetCamera { get; private set; }
    }
}