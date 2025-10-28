using UnityEngine;
using System.Collections;

namespace RedRunner.MVC.Views.Enemy
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class EnemyView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject deathEffectPrefab;
        [SerializeField] private Color hitFlashColor = Color.white;
        [SerializeField] private float hitFlashDuration = 0.1f;

        private static readonly int DeathTriggerHash = Animator.StringToHash("Death");

        private void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (animator == null) animator = GetComponent<Animator>();
        }

        public void PlayHitEffect()
        {
            StartCoroutine(FlashColor(hitFlashColor, hitFlashDuration));
        }

        public void PlayDeathAnimation()
        {
            if (animator != null && animator.runtimeAnimatorController != null)
            {
                bool hasDeathParam = false;
                foreach (var param in animator.parameters)
                {
                    if (param.nameHash == DeathTriggerHash)
                    {
                        hasDeathParam = true;
                        break;
                    }
                }
                if (hasDeathParam) animator.SetTrigger(DeathTriggerHash);
            }

            if (deathEffectPrefab != null)
            {
                Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            }
        }

        private IEnumerator FlashColor(Color flashColor, float duration)
        {
            if (spriteRenderer != null)
            {
                Color originalColor = spriteRenderer.color;
                spriteRenderer.color = flashColor;
                yield return new WaitForSeconds(duration);
                if (spriteRenderer != null) spriteRenderer.color = originalColor;
            }
        }

        public void SetFacing(bool faceRight)
        {
            transform.localScale = new Vector3(faceRight ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}