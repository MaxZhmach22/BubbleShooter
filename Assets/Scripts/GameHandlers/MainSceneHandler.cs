using NaughtyAttributes;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace BubbleShooter
{
    public class MainSceneHandler : MonoBehaviour
    {
        [field: Foldout("References")] [field: SerializeField] public MainMenuScreen MainMenuScreen { get; private set; }
        [field: Foldout("References")] [field: SerializeField] public LevelSettingsScreen LevelSettingsScreen { get; private set; }
        [field: Foldout("References")] [field: SerializeField] public GridCreator GridCreator { get; private set; }

        
        private ScreenBase[] _screens;
        private MouseInputHandler _mouseInputHandler;
        private int _counter;
        
        private void Awake()
        {
            _screens = FindObjectsOfType<ScreenBase>(true);
            
            _mouseInputHandler = new MouseInputHandler(
                    MainMenuScreen,
                    GridCreator.StartPoisiton, 
                    LevelSettingsScreen.RadiusSlider,
                    GridCreator.SelectionMaterial)
                .AddTo(this);
            
            Bind();
            ShowScreen(MainMenuScreen);
        }

        private void Bind()
        {
            MainMenuScreen.Play.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    ShowScreen(LevelSettingsScreen);
                })
                .AddTo(this);

            LevelSettingsScreen.Randomize.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    GridCreator.RandomizeColors();
                })
                .AddTo(this);
            
            LevelSettingsScreen.Color.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _counter++;
                    if (_counter >= 5) _counter = 0;
                    
                    foreach (var cell in _mouseInputHandler.SelectedCells.Keys)
                    {
                        cell.ColorTypes = (ColorTypes)_counter;
                    }
                })
                .AddTo(this);
            
            LevelSettingsScreen.Reset.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    if (_mouseInputHandler.SelectedCells.Count == 0)
                    {
                        GridCreator.ResetAllColors();
                    }
                    else
                    {
                        _mouseInputHandler.ResetSelected();
                    }
                })
                .AddTo(this);
            
            LevelSettingsScreen.Apply.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    foreach (var cells in _mouseInputHandler.SelectedCells)
                    {
                        if (cells.Key.Renderer.material.color == GridCreator.SelectionMaterial.color)
                        {
                            cells.Key.Renderer.material = cells.Value;
                        }
                    }
                    _mouseInputHandler.SelectedCells.Clear();
                })
                .AddTo(this);
            
            LevelSettingsScreen.Play.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    GridCreator.OnPlayButton();
                    SceneManager.LoadSceneAsync("GameScene");
                })
                .AddTo(this);
        }

        private void ShowScreen(ScreenBase screen)
        {
            foreach (var screenBase in _screens)
            {
                screenBase.gameObject.SetActive(false);
            }

            screen.gameObject.SetActive(true);
        }
    }
}