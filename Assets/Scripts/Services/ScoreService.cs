using UnityEngine;
using Assets.Scripts.Patterns.Singleton;
using Assets.Scripts.Patterns.Observer;
using Assets.Scripts.Core; // For ServiceLocator
using Assets.Scripts.Interfaces; // For ISaveService
namespace Assets.Scripts.Services
{
    public class ScoreService : SingletonMonoBehaviour<ScoreService>
    {
        private int currentScore = 0;
        private float currentDistanceScore = 0f;
    
        private int highScore = 0;
        private int lastScore = 0;
        private ISaveService saveService;
        private Transform playerTransform; 
        public int HighScore => highScore;
        public int LastScore => lastScore;
        public int CurrentScore => currentScore +
        Mathf.FloorToInt(currentDistanceScore);
        
        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;
            ServiceLocator.Instance.RegisterService<ScoreService>(this);
        }

        private void Start()
        {
            saveService = ServiceLocator.Instance.GetService<ISaveService>();
            LoadScores();
            // Subscribe to relevant events
            GameEventSystem.Instance.Subscribe(GameEvents.ENEMY_KILLED, OnEnemyKilled);
            GameEventSystem.Instance.Subscribe(GameEvents.COIN_COLLECTED,OnCoinCollected);
            // GameEventSystem.Instance.Subscribe(GameEvents.LEVEL_COMPLETE,OnLevelComplete); 
            GameEventSystem.Instance.Subscribe(GameEvents.GAME_RESTARTED, ResetScore); // Reset on restart
            GameEventSystem.Instance.Subscribe(GameEvents.PLAYER_DIED,OnPlayerFinalDeath); // Save score on final death
            // Find player for distance tracking
            GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null) playerTransform = playerGO.transform;
            UpdateAndPublishScores(); // Publish initial scores
        }

        protected override void OnDestroy()
        {
            if (GameEventSystem.Instance != null)
            {
                GameEventSystem.Instance.Unsubscribe(GameEvents.ENEMY_KILLED, OnEnemyKilled);
                GameEventSystem.Instance.Unsubscribe(GameEvents.COIN_COLLECTED, OnCoinCollected);
                GameEventSystem.Instance.Unsubscribe(GameEvents.GAME_RESTARTED, ResetScore);
                GameEventSystem.Instance.Unsubscribe(GameEvents.PLAYER_DIED, OnPlayerFinalDeath);
            }
            base.OnDestroy();
        }
        private void Update()
        {
            if (playerTransform != null && GameManager.Instance.CurrentState == GameState.Playing)
            {
                float distance = Mathf.Max(0, playerTransform.position.x);
                if (distance > currentDistanceScore)
                {
                    currentDistanceScore = distance;
                    UpdateAndPublishScores();
                }
            }
        }
        private void LoadScores()
        {
            highScore = saveService?.LoadGame<int>("highScore", 0) ?? 0;
            lastScore = saveService?.LoadGame<int>("lastScore", 0) ?? 0;
            currentScore = 0; // Reset session score
            currentDistanceScore = 0f;
        }
        private void SaveScores()
        {
            int finalScore = CurrentScore;
            lastScore = finalScore;
            if (finalScore > highScore)
            {
                highScore = finalScore;
                saveService?.SaveGame("highScore", highScore);
            }
            saveService?.SaveGame("lastScore", lastScore);
        }
        private void OnPlayerFinalDeath(object data)
        {
            SaveScores(); 
        }
        private void AddPoints(int points)
        {
            if (points <= 0) return;
            currentScore += points;
            UpdateAndPublishScores();
        }
        private void ResetScore(object data = null)
        {
            SaveScores();
            currentScore = 0;
            currentDistanceScore = 0f;
            UpdateAndPublishScores();
    }

    // --- Event Handlers ---
    private void OnEnemyKilled(object data)
        {
            if (data is int scoreValue) AddPoints(scoreValue);
            // else if (data is MVC.Models.Enemy.EnemyModel enemyModel)
            //     AddPoints(enemyModel.ScoreValue);
        }
        private void OnCoinCollected(object data)
        {
            if (data is int value) AddPoints(value * 10);
        
    }

    private void UpdateAndPublishScores()
    {
        GameEventSystem.Instance.Publish(GameEvents.SCORE_CHANGED, CurrentScore);
    }

    public int GetCurrentScore() => CurrentScore;
    public int GetHighScore() => highScore;
    public int GetLastScore() => lastScore;
    protected override void OnApplicationQuit()
        {
            SaveScores(); // Ensure scores are saved on quit
            base.OnApplicationQuit();
        }
    }
}