using System;
using BubbleShooter;
using NaughtyAttributes;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace GameHandlers
{
    public class GameUiHandler : MonoBehaviour
    {
        [field: Foldout("Reference")] [field: SerializeField] public GameScreen GameScreen { get; private set; }
        [field: Foldout("Reference")] [field: SerializeField] public GridCreator GridCreator { get; private set; }

        private void Awake()
        {
            if(!GridCreator) GridCreator = FindObjectOfType<GridCreator>();
        }

        private void Start()
        {

            GameScreen.Restart.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    if (GridCreator)
                    {
                        GridCreator.CellsList.ForEach(x =>
                        {
                            x.Renderer.transform.localScale = GridCreator.LocalSphereScale;
                            x.Reset();
                        });
                    }
                    SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
                })
                .AddTo(this);
        }
    }
}