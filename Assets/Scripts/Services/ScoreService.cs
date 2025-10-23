using UnityEngine;
using RedRunner.Patterns.Singleton;
using RedRunner.Patterns.Observer;
using RedRunner.Core; // For ServiceLocator
using RedRunner.Interfaces; // For ISaveService
namespace RedRunner.Services
{
    public class ScoreService : SingletonMonoBehaviour<ScoreService>
    {
        private int currentScore = 0;
        private float currentDistanceScore = 0f; // Score based on distance
        traveled
    
        private int highScore = 0;
        private int lastScore = 0;
        private ISaveService saveService;
        private Transform playerTransform; // To track distance
        public int HighScore => highScore;
        public int LastScore => lastScore;
        public int CurrentScore => currentScore +
        Mathf.FloorToInt(currentDistanceScore); // Combine points + distance
        
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
            // GameEventSystem.Instance.Subscribe(GameEvents.LEVEL_COMPLETE,OnLevelComplete); // If bonus score exists
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
                GameEventSystem.Instance.Unsubscribe(GameEvents.LEVEL_COMPLETE,OnLevelComplete);
                GameEventSystem.Instance.Unsubscribe(GameEvents.GAME_RESTARTED, ResetScore);
                GameEventSystem.Instance.Unsubscribe(GameEvents.PLAYER_DIED, OnPlayerFinalDeath);
            }
            // Save high score on quit? GameManager might handle this better.
            // SaveHighScore();
            base.OnDestroy();
        }
        private void Update()
        {
            // Update distance score if player exists and game is playing
            if (playerTransform != null && GameManager.Instance.CurrentState == GameState.Playing)
            {
                // Assuming score increases based on positive X movement from
                origin float distance = Mathf.Max(0, playerTransform.position.x); //
                Adjust if start X isn't 0 
                    if (distance > currentDistanceScore)
                {
                    currentDistanceScore = distance;
                    UpdateAndPublishScores(); // Update UI as distance
                    increases
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
            SaveScores(); // Save scores when the player is completely out of
            lives
        }
        private void AddPoints(int points)
        {
            if (points <= 0) return;
            currentScore += points;
            UpdateAndPublishScores();
        }
        private void ResetScore(object data = null) // Can be called by event or directly
        {
            SaveScores(); // Save previous score before resetting
            currentScore = 0;
            currentDistanceScore = 0f;
            // Don't reset highScore or lastScore here, they persist
            UpdateAndPublishScores();
    }

    // --- Event Handlers ---
    private void OnEnemyKilled(object data)
        {
            if (data is int scoreValue) AddPoints(scoreValue);
            else if (data is MVC.Models.Enemy.EnemyModel enemyModel)
                AddPoints(enemyModel.ScoreValue);
        }
        private void OnCoinCollected(object data)
        {
            if (data is int value) AddPoints(value * 10); // Example: coin gives 10 points
        // Or get value from Coin Model if event passes it
    }
        // private void OnLevelComplete(object data) { /* Add bonus points */
    }

    private void UpdateAndPublishScores()
    {
        // Publish combined score
        GameEventSystem.Instance.Publish(GameEvents.SCORE_CHANGED, CurrentScore);
        // Optionally publish high score too if UI needs it frequently
        //
        GameEventSystem.Instance.Publish(GameEvents.HIGH_SCORE_CHANGED, highScore);
    }

    // Public getters for UI or other systems
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