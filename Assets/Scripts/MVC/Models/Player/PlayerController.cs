using UnityEngine;
using Assets.Scripts.MVC.Models.Player;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Core;
using Assets.Scripts.Patterns.Observer;

namespace Assets.Scripts.MVC.Models.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("MVC References")]
        [SerializeField] private PlayerData playerData;
        [SerializeField] private PlayerView playerView;

        [Header("Physics & Ground Check")]
        [SerializeField] private Rigidbody2D rb2d;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundLayer;

        private PlayerModel playerModel;
        private IInputService inputService;

        private bool isGrounded;
        private Vector2 currentMoveInput;
        private float currentMoveSpeed;
    
        protected virtual void Awake()
        {
            if (playerData == null) Debug.LogError("[PlayerController] PlayerData ScriptableObject is not assigned!");
            if (playerView == null) playerView = GetComponent<PlayerView>();
            if (rb2d == null) rb2d = GetComponent<Rigidbody2D>();
            if (groundCheck == null) Debug.LogError("[PlayerController] GroundCheck Transform is not assigned!");

            playerModel = new PlayerModel(playerData);
            currentMoveSpeed = playerData.moveSpeed;
            rb2d.gravityScale = playerData.gravityScale;
        }

        private void Start()
        {
            inputService = ServiceLocator.Instance.GetService<IInputService>();
            if (inputService == null)
            {
                Debug.LogError("[PlayerController] InputService not found!");
                enabled = false;
                return;
            }

            GameEventSystem.Instance.Subscribe(GameEvents.PLAYER_DIED, OnPlayerDied);
            GameEventSystem.Instance.Subscribe(GameEvents.PLAYER_RESPAWNED, OnPlayerRespawned);

            GameEventSystem.Instance.Publish(GameEvents.HEALTH_CHANGED, new PlayerModel.HealthData { Current = playerModel.GetCurrentHealth(), Max = playerModel.GetMaxHealth() });
            GameEventSystem.Instance.Publish(GameEvents.SCORE_CHANGED, playerModel.Score);
            GameEventSystem.Instance.Publish(GameEvents.COINS_CHANGED, playerModel.Coins);
            GameEventSystem.Instance.Publish(GameEvents.LIVES_CHANGED, playerModel.Lives);
        }

        private void OnDestroy()
        {
            GameEventSystem.Instance.Unsubscribe(GameEvents.PLAYER_DIED, OnPlayerDied);
            GameEventSystem.Instance.Unsubscribe(GameEvents.PLAYER_RESPAWNED, OnPlayerRespawned);
        }

        private void Update()
        {
            if (!playerModel.IsAlive() || GameManager.Instance.CurrentState != GameState.Playing)
            {
                rb2d.linearVelocity = Vector2.zero;
                playerView.UpdateMoveAnimation(0);
                return;
            }

            HandleInput();
            CheckGroundStatus();
            playerModel.UpdateInvulnerability(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            if (!playerModel.IsAlive() || GameManager.Instance.CurrentState != GameState.Playing) return;

            HandleMovement();
            ApplyMaxFallSpeed();

            if (playerModel.IsInvulnerable)
                playerView.SetInvulnerabilityVisual(true, playerModel.InvulnerabilityTimer);
            else
                playerView.SetInvulnerabilityVisual(false, 0);
        }

        private void HandleInput()
        {
            currentMoveInput = inputService.GetMovementInput();

            if (inputService.GetJumpInputDown() && isGrounded)
                Jump();
        }

        private void HandleMovement()
        {
            float targetSpeed = currentMoveInput.x * currentMoveSpeed;
            rb2d.linearVelocity = new Vector2(targetSpeed, rb2d.linearVelocity.y);

            if (currentMoveInput.x != 0)
                playerView.SetFacing(currentMoveInput.x > 0);

            playerView.UpdateMoveAnimation(rb2d.linearVelocity.x);
        }

        private void Jump()
        {
            rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, playerData.jumpForce);
            playerView.PlayJumpAnimation();
            isGrounded = false;
            playerView.SetGrounded(false);
        }

        private void CheckGroundStatus()
        {
            bool wasGrounded = isGrounded;
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, playerData.groundCheckRadius, groundLayer);

            if (isGrounded && !wasGrounded)
                playerView.SetGrounded(true);
            else if (!isGrounded && wasGrounded)
                playerView.SetGrounded(false);
        }

        private void ApplyMaxFallSpeed()
        {
            if (rb2d.linearVelocity.y < -playerData.maxFallSpeed)
                rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, -playerData.maxFallSpeed);
        }

        public void TakeDamage(float damage)
        {
            playerModel.TakeDamage(damage);
            playerView.PlayHitVisual();
            if (rb2d != null && damage > 0 && playerModel.GetCurrentHealth() > 0)
            {
                Vector2 knockbackDirection = (playerView.transform.localScale.x > 0) ? Vector2.left : Vector2.right;
                rb2d.AddForce(knockbackDirection * playerData.knockbackForce, ForceMode2D.Impulse);
            }
        }

        public float GetCurrentHealth() => playerModel.GetCurrentHealth();
        public float GetMaxHealth() => playerModel.GetMaxHealth();
        public bool IsAlive() => playerModel.IsAlive();

        public void Move(Vector2 direction) => currentMoveInput = direction.normalized;
        public void Stop() => currentMoveInput = Vector2.zero;
        public float GetMoveSpeed() => currentMoveSpeed;
        public void SetMoveSpeed(float speed) => currentMoveSpeed = speed;

        public void AddScore(int points) => playerModel.AddScore(points);
        public void AddCoins(int amount) => playerModel.AddCoins(amount);
        public void AddLife() => playerModel.AddLife();
        public void Heal(float amount) => playerModel.Heal(amount);
        public PlayerModel GetModel() => playerModel;

        private void OnPlayerDied(object data)
        {
            inputService.SetInputEnabled(false);
            playerView.PlayDeathVisual();
            rb2d.linearVelocity = Vector2.zero;
            rb2d.bodyType = RigidbodyType2D.Kinematic;
            GetComponent<Collider2D>().enabled = false;
        }

        private void OnPlayerRespawned(object data)
        {
            inputService.SetInputEnabled(true);
            rb2d.bodyType = RigidbodyType2D.Dynamic;
            GetComponent<Collider2D>().enabled = true;
            playerView.SetInvulnerabilityVisual(true, playerData.invulnerabilityDuration);
            transform.position = Vector3.zero;
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheck != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(groundCheck.position, playerData.groundCheckRadius);
            }
        }
    }
}
