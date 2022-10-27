using GameHandlers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;


namespace BubbleShooter 
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GameUiHandler))]
    sealed class EcsStartup : MonoBehaviour 
    {
        private EcsWorld _world;        
        private IEcsSystems _initSystems;
        private IEcsSystems _fixedUpdateSystems;
        private IEcsSystems _updateSystems;

        private GameUiHandler _gameUiHandler;
        private GridCreator _gridCreator;
        private DynamicJoystick _dynamicJoystick;
        private LevelSetting _levelSetting;

        void Start ()
        {
            _gameUiHandler = GetComponent<GameUiHandler>();
            _gridCreator = _gameUiHandler.GridCreator;
            _dynamicJoystick = _gameUiHandler.GameScreen.DynamicJoystick;
            _levelSetting = GetComponent<LevelSetting>();
            
            _world = new EcsWorld ();
            _initSystems = new EcsSystems(_world);
            _fixedUpdateSystems = new EcsSystems(_world);
            _updateSystems = new EcsSystems (_world);

            AddInitSystems();
            AddFixedUpdateSystems();
            AddUpdateSystems();
            
            InjectDependencies();
            
            _initSystems.Init();
            _fixedUpdateSystems.Init();
            _updateSystems.Init();
        }

        private void AddInitSystems()
        {
            _initSystems
                .Add(new InputInitSystem())
                .Add(new PlayerInitSystem());
        }

        private void AddFixedUpdateSystems()
        {
            _fixedUpdateSystems
                .Add(new DrawRaySystem());
        }

        private void AddUpdateSystems()
        {
            _updateSystems
#if UNITY_EDITOR
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
#endif
                .Add(new InputRunSystem())
                .Add(new MouseButtonInputSystem())
                .Add(new PlayerMovement())
                .Add(new GridCheckSystem())
                .Add(new ChangeColorSystem())
                .Add(new WinSystem());
        }

        private void InjectDependencies()
        {
            _initSystems
                .Inject()
                .Inject(_gameUiHandler, _gridCreator, _dynamicJoystick, _levelSetting);

            _fixedUpdateSystems
                .Inject()
                .Inject(_gameUiHandler, _gridCreator, _dynamicJoystick, _levelSetting);

            _updateSystems
                .Inject()
                .Inject(_gameUiHandler, _gridCreator, _dynamicJoystick, _levelSetting);
        }

        private void FixedUpdate()
        {
            _fixedUpdateSystems?.Run();
        }
        
        private void Update () 
        {
            _updateSystems?.Run ();
        }

        private void OnDestroy () 
        {
            if (_updateSystems != null) 
            {
                _updateSystems.Destroy ();
                _updateSystems = null;
            }
            
            if (_world != null) {
                _world.Destroy ();
                _world = null;
            }
        }
    }
}