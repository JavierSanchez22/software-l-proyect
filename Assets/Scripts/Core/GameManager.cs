using UnityEngine;
using DinoRunner.Patterns.Singleton;
using DinoRunner.Interfaces;
using DinoRunner.Core;
using UnityEngine.SceneManagement;
using DinoRunner.Patterns.Observer; // For Events
using UnityEngine.Networking;

namespace DinoRunner.Core
{
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.Loading; // Start in Loading

        [Header("Configuration")]
        [SerializeField] private GameConfig gameConfig; 

        private InputService inputService;
        private AudioService audioService;
        private SaveService saveService;

        public GameState CurrentState => currentState;

        protected override void Awake()
        {
            base.Awake();
            if (gameConfig == null) gameConfig = Resources.Load<GameConfig>("GameConfig_Default");
            if (gameConfig == null) Debug.LogError("[GameManager] GameConfig is not assigned!");
        }

        private void Start()
        {
            if (ServiceLocator.Instance == null) { Debug.LogError("[GameManager] ServiceLocator not ready!"); return; }
            inputService = ServiceLocator.Instance.GetService<InputService>();
            audioService = ServiceLocator.Instance.GetService<AudioService>();
            saveService = ServiceLocator.Instance.GetService<SaveService>();

            LoadAudioSettings();

            string currentSceneName = SceneManager.GetActiveScene().name;
            if (currentSceneName.Contains("Menu")) 
            {
                ChangeState(GameState.Menu);
            }
            else if (currentSceneName.Contains("Game"))
            {
                StartCoroutine(InitialLoadSequence());
            } else {
                ChangeState(GameState.Playing);
            }

            GameEventSystem.Instance.Subscribe(GameEvents.PLAYER_DIED, HandlePlayerDeath);
        }

        private System.Collections.IEnumerator InitialLoadSequence()
        {
            ChangeState(GameState.Loading);
             yield return new WaitForSeconds(0.1f); // Simulate loading delay
            ChangeState(GameState.Playing);
        }

        private void Update()
        {
            // Global Pause Handling
            if (inputService != null && inputService.GetPauseInput())
            {
                if (currentState == GameState.Playing)
                    PauseGame();
                else if (currentState == GameState.Paused)
                    ResumeGame();
            }
        }

        protected override void OnDestroy()
        {
            GameEventSystem.Instance.Unsubscribe(GameEvents.PLAYER_DIED, HandlePlayerDeath);
            base.OnDestroy();
        }


        public void ChangeState(GameState newState)
        {
            if (currentState == newState) return;
            ExitState(currentState);
            currentState = newState;
            EnterState(newState);
            Debug.Log($"[GameManager] State changed to: {newState}");
        }

        private void EnterState(GameState state)
        {
            switch (state)
            {
                case GameState.Loading:
                    Time.timeScale = 1f;
                    GameEventSystem.Instance.Publish(GameEvents.SHOW_SCREEN, UIScreenInfo.LOADING_SCREEN);
                    break;
                case GameState.Menu:
                    Time.timeScale = 1f;
                    audioService?.PlayMusic("menu_music");
                    GameEventSystem.Instance.Publish(GameEvents.SHOW_SCREEN, UIScreenInfo.START_SCREEN);
                    break;
                case GameState.Playing:
                    Time.timeScale = 1f;
                    inputService?.SetInputEnabled(true);
                    audioService?.PlayMusic("game_music");
                    GameEventSystem.Instance.Publish(GameEvents.SHOW_SCREEN, UIScreenInfo.IN_GAME_SCREEN);
                    break;
                case GameState.Paused:
                    Time.timeScale = 0f;
                    inputService?.SetInputEnabled(false);
                    GameEventSystem.Instance.Publish(GameEvents.SHOW_SCREEN, UIScreenInfo.PAUSE_SCREEN);
                    break;
                case GameState.GameOver:
                    Time.timeScale = 1f;
                    inputService?.SetInputEnabled(false);
                    audioService?.PlaySound("game_over");
                    GameEventSystem.Instance.Publish(GameEvents.SHOW_SCREEN, UIScreenInfo.END_SCREEN);
                    break;
                case GameState.Victory:
                    Time.timeScale = 1f;
                    inputService?.SetInputEnabled(false);
                    audioService?.PlaySound("victory_sound");
                    break;
            }
        }

        private void ExitState(GameState state)
        {
            switch (state)
            {
                case GameState.Loading: GameEventSystem.Instance.Publish(GameEvents.HIDE_SCREEN, UIScreenInfo.LOADING_SCREEN); break;
                case GameState.Menu: GameEventSystem.Instance.Publish(GameEvents.HIDE_SCREEN, UIScreenInfo.START_SCREEN); break;
                case GameState.Playing: GameEventSystem.Instance.Publish(GameEvents.HIDE_SCREEN, UIScreenInfo.IN_GAME_SCREEN); break;
                case GameState.Paused: GameEventSystem.Instance.Publish(GameEvents.HIDE_SCREEN, UIScreenInfo.PAUSE_SCREEN); break;
                case GameState.GameOver: GameEventSystem.Instance.Publish(GameEvents.HIDE_SCREEN, UIScreenInfo.END_SCREEN); break;
            }
        }

        // --- Public Methods for State Control ---
        public void StartGame()
        {
            if (currentState == GameState.Menu || currentState == GameState.GameOver)
            {
                // Load the main game scene
                SceneManager.LoadScene("GameScene");
            }
        }

        public void PauseGame()
        {
            if (currentState == GameState.Playing) ChangeState(GameState.Paused);
        }

        public void ResumeGame()
        {
            if (currentState == GameState.Paused) ChangeState(GameState.Playing);
        }

        public void TriggerGameOver()
        {
            if (currentState != GameState.GameOver) ChangeState(GameState.GameOver);
        }

        public void RestartGame() // Called by UI
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void LoadMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }

        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        // --- Audio Settings ---
        public void ToggleAudioEnabled()
        {
            bool newState = !(AudioListener.volume > 0f);
            SetAudioEnabled(newState);
            saveService?.SaveGame("audioEnabled", newState);
        }

        public void SetAudioEnabled(bool isEnabled)
        {
            AudioListener.volume = isEnabled ? 1f : 0f;
            GameEventSystem.Instance.Publish(GameEvents.AUDIO_STATE_CHANGED, isEnabled);
        }

        private void LoadAudioSettings()
        {
            bool isEnabled = saveService?.LoadGame<bool>("audioEnabled") ?? true;
            SetAudioEnabled(isEnabled);
        }

        
        public void ShareOnTwitter() => Share("https://x.com/intent/post?text={0}&url=https://i.imgur.com/vJU24Pf.png");
        public void ShareOnFacebook() => Share("https://www.facebook.com/sharer/sharer.php?u={1}");

        private void Share(string urlTemplate)
        {
            if (gameConfig != null)
            {
                Application.OpenURL(string.Format(urlTemplate, UnityWebRequest.EscapeURL(gameConfig.shareText), UnityWebRequest.EscapeURL(gameConfig.shareUrl)));
                Application.OpenURL(url);
            }
            else
            {
                Debug.LogError("Cannot share, GameConfig not assigned!");
            }
        }


         // --- Event Handlers ---
        private void HandlePlayerDeath(object data)
        {
            TriggerGameOver();
        }
    }

    public enum GameState
    {
        Loading,
        Menu,
        Playing,
        Paused,
        GameOver,
        Victory
    }

    public enum UIScreenInfo
    {
        LOADING_SCREEN,
        START_SCREEN,
        END_SCREEN,
        PAUSE_SCREEN,
        IN_GAME_SCREEN,
        VICTORY_SCREEN
    }
}