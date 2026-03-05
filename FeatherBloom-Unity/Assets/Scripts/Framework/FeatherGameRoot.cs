using Cysharp.Threading.Tasks;
using DevTools;
using Framework.GlobalServices;
using Framework.LevelLoading;
using Framework.LevelLoading.LevelTransition;
using Framework.Timescaling;
using Input;
using Input.DataTypes;
using Input.SerialComms;
using Saving;
using UnityEngine;

namespace Framework
{
    /// <summary>
    ///     Root game object and entry point for the Feather game
    /// </summary>
    public class FeatherGameRoot : MonoBehaviour
    {
        [Header("Depends")]

        [SerializeField]
        private StartupConfigSO _startupConfig;

        [Header("Services")]

        [SerializeField]
        private GameplayInputService _gameplayInputService;

        [SerializeField]
        private HandFanArduinoComm _handFanArduinoComm;

        [SerializeField]
        private BoxFanArduinoComm _boxFanArduinoComm;

        private ServiceLocator _serviceLocator;
        private ScoreService _scoreService;
        private TimeScaleService _timeScaleService;
        private LevelLoader _levelLoader;
        private SaveService _saveService;

        private void Awake()
        {
            InitializeGame();
        }

        private void OnDestroy()
        {
            _startupConfig.SaveDataValueSOLoader.Cleanup();
        }

        private void InitializeGame()
        {
            SetupServices();

            _startupConfig.SaveDataValueSOLoader.InitializeSaveBack(_saveService);

            InputSetup();

            GameLevelSO startupLevel = DevToolState.GoIntoGameplayImmediately
                ? _startupConfig.GameplayLevel
                : _startupConfig.StartupGameLevel;

            if (DevToolState.OverrideStartupLevel != null)
            {
                startupLevel = DevToolState.OverrideStartupLevel;
            }

            _levelLoader.LoadLevel(startupLevel).Forget();
        }

        private void SetupServices()
        {
            _saveService = new SaveService();
            _serviceLocator = new ServiceLocator();
            _scoreService = new ScoreService();
            _timeScaleService = new TimeScaleService(_startupConfig.TimeScaleLerpFactor);
            _levelLoader = new LevelLoader(new NullLevelLoadTransition());

            _gameplayInputService.Initialize();

            _serviceLocator.RegisterService(_scoreService);
            _serviceLocator.RegisterService(_timeScaleService);
            _serviceLocator.RegisterService(_levelLoader);
            _serviceLocator.RegisterService(_gameplayInputService);
            _serviceLocator.RegisterService(_handFanArduinoComm);
            _serviceLocator.RegisterService(_boxFanArduinoComm);
        }

        private void Update()
        {
            _timeScaleService.DoUpdate();
        }

        private void InputSetup()
        {
            if (DevToolState.QuickArduinoConnect)
            {
                _gameplayInputService.SwitchInputType(GameplayInputType.CustomHardware);

                _boxFanArduinoComm.SetSerialPort(_saveService.SaveModel.LastBoxFanConnectedSerialPortName);
                _boxFanArduinoComm.Connect();

                _handFanArduinoComm.SetSerialPort(_saveService.SaveModel.LastHandFanConnectedSerialPortName);
                _handFanArduinoComm.Connect();
            }
            else
            {
                _gameplayInputService.SwitchInputType(GameplayInputType.Conventional);
            }
        }
    }
}