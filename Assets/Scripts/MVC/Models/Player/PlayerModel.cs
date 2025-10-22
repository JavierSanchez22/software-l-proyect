using UnityEngine;
using Assets.Scripts.Interfaces;
using Assets.Scripts.MVC.Models.Player;
// using DinoRunner.Patterns.Observer;

namespace Assets.Scripts.MVC.Models.Player
{
    public class PlayerModel : IDamageable
    {
        public float Health { get; private set; }
        public float MaxHealth { get; private set; }
        public int Score { get; private set; }
        public int Lives { get; private set; }
        public int Coins { get; private set; }
        public bool IsInvulnerable { get; private set; }
        public float InvulnerabilityTimer { get; private set; }

        private PlayerData playerData;

        public PlayerModel(PlayerData data)
        {
            if (data == null)
            {
                Debug.LogError("[PlayerModel] PlayerData ScriptableObject is null!");
                return;
            }
            playerData = data;
            MaxHealth = playerData.maxHealth;
            Health = MaxHealth;
            Lives = playerData.initialLives;
            Score = 0;
            Coins = 0;
            IsInvulnerable = false;
            InvulnerabilityTimer = 0f;
        }
        
        public void UpdateInvulnerability(float deltaTime)
        {
            if (IsInvulnerable)
            {
                InvulnerabilityTimer -= deltaTime;
                if (InvulnerabilityTimer <= 0)
                {
                    SetInvulnerability(false);
                }
            }
        }

        public void TakeDamage(float damage)
        {
            if (IsInvulnerable || Health <= 0) return;

            Health = Mathf.Max(0, Health - damage);
            // GameEventSystem.Instance.Publish(GameEvents.HEALTH_CHANGED, new HealthData { Current = Health, Max = MaxHealth });

            if (Health <= 0)
            {
                LoseLife();
            }
            else
            {
                SetInvulnerability(true);
                InvulnerabilityTimer = playerData.invulnerabilityDuration;
            }
        }

        public float GetCurrentHealth() => Health;
        public float GetMaxHealth() => MaxHealth;
        public bool IsAlive() => Lives > 0;

        public void Heal(float amount)
        {
            if (Health <= 0) return;
            Health = Mathf.Min(MaxHealth, Health + amount);
            // GameEventSystem.Instance.Publish(GameEvents.HEALTH_CHANGED, new HealthData { Current = Health, Max = MaxHealth });
        }

        public void AddScore(int points)
        {
            if (points <= 0) return;
            Score += points;
            // GameEventSystem.Instance.Publish(GameEvents.SCORE_CHANGED, Score);
        }

        public void AddCoins(int amount)
        {
            if (amount <= 0) return;
            Coins += amount;
            // GameEventSystem.Instance.Publish(GameEvents.COINS_CHANGED, Coins);
        }

        public void LoseLife()
        {
            if (Lives <= 0) return;

            Lives--;
            // GameEventSystem.Instance.Publish(GameEvents.LIVES_CHANGED, Lives);

            if (Lives > 0)
            {
                Health = MaxHealth;
                // GameEventSystem.Instance.Publish(GameEvents.HEALTH_CHANGED, new HealthData { Current = Health, Max = MaxHealth });
                // GameEventSystem.Instance.Publish(GameEvents.PLAYER_RESPAWNED);
            }
            else
            {
                Health = 0;
                // GameEventSystem.Instance.Publish(GameEvents.HEALTH_CHANGED, new HealthData { Current = Health, Max = MaxHealth });
                // GameEventSystem.Instance.Publish(GameEvents.PLAYER_DIED);
            }
        }

        public void AddLife()
        {
            Lives++;
            // GameEventSystem.Instance.Publish(GameEvents.LIVES_CHANGED, Lives);
        }

        public void SetInvulnerability(bool state)
        {
            IsInvulnerable = state;
            // GameEventSystem.Instance.Publish(GameEvents.PLAYER_INVULNERABILITY_STATUS_CHANGED, state);
        }

        public void ResetForNewGame()
        {
            MaxHealth = playerData.maxHealth;
            Health = MaxHealth;
            Lives = playerData.initialLives;
            Score = 0;
            Coins = 0;
            IsInvulnerable = false;
            InvulnerabilityTimer = 0f;

            // GameEventSystem.Instance.Publish(GameEvents.HEALTH_CHANGED, new HealthData { Current = Health, Max = MaxHealth });
            // GameEventSystem.Instance.Publish(GameEvents.SCORE_CHANGED, Score);
            // GameEventSystem.Instance.Publish(GameEvents.COINS_CHANGED, Coins);
            // GameEventSystem.Instance.Publish(GameEvents.LIVES_CHANGED, Lives);
        }

        public struct HealthData
        {
            public float Current;
            public float Max;
        }
    }
}
