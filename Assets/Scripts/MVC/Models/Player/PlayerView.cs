using UnityEngine;

namespace Assets.Scripts.MVC.Models.Player
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    public class PlayerView : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] public SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;

        [Header("Visual Effects")]
        [SerializeField] private GameObject hitEffectPrefab;
        [SerializeField] private GameObject deathEffectPrefab;

        private readonly int speedHash = Animator.StringToHash("Speed");
        private readonly int jumpHash = Animator.StringToHash("Jump");
        private readonly int isGroundedHash = Animator.StringToHash("IsGrounded");

        private void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (animator == null) animator = GetComponent<Animator>();
        }

        public void SetFacing(bool facingRight)
        {
            transform.localScale = new Vector3(facingRight ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        public void UpdateMoveAnimation(float speed)
        {
            animator?.SetFloat(speedHash, Mathf.Abs(speed));
        }

        public void SetGrounded(bool grounded)
        {
            animator?.SetBool(isGroundedHash, grounded);
        }

        public void PlayJumpAnimation()
        {
            animator?.SetTrigger(jumpHash);
        }

        public void PlayHitVisual()
        {
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }
            StartCoroutine(FlashColor(Color.red, 0.1f));
        }

        public void PlayDeathVisual()
        {
            if (deathEffectPrefab != null)
            {
                Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            }
        }

        public void SetInvulnerabilityVisual(bool invulnerable, float duration)
        {
            StopAllCoroutines();
            if (invulnerable)
            {
                StartCoroutine(BlinkEffect(duration));
            }
            else
            {
                spriteRenderer.enabled = true;
                spriteRenderer.color = Color.white;
            }
        }

        private System.Collections.IEnumerator FlashColor(Color flashColor, float duration)
        {
            if (spriteRenderer != null)
            {
                Color originalColor = spriteRenderer.color;
                spriteRenderer.color = flashColor;
                yield return new WaitForSeconds(duration);
                spriteRenderer.color = originalColor;
            }
        }

        private System.Collections.IEnumerator BlinkEffect(float blinkDuration, float interval = 0.1f)
        {
            if (spriteRenderer == null) yield break;

            float endTime = Time.time + blinkDuration;
            while(Time.time < endTime)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                yield return new WaitForSeconds(interval);
            }
            spriteRenderer.enabled = true;
        }
    }
}
