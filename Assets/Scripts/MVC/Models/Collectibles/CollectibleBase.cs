using UnityEngine;
using DinoRunner.Interfaces;
namespace DinoRunner.MVC.Models.Collectibles
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class CollectibleBase : MonoBehaviour, ICollectible
    {
        [Header("Collectible Settings")]
        [SerializeField] protected int value = 10;
        [SerializeField] protected string identifier = "Collectible"; // Type identifier
        [SerializeField] protected string collectSoundName = "collect"; // Sound to play on collect
        [SerializeField] protected GameObject collectEffectPrefab; // Particle effect on collect
        protected bool isCollected = false;
        protected Collider2D itemCollider;
        protected SpriteRenderer spriteRenderer;
        protected virtual void Awake()
        {
            itemCollider = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (itemCollider != null) itemCollider.isTrigger = true; // Ensure it acts as a trigger
        }
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (isCollected) return;
            if (other.CompareTag("Player"))
            {
                {
                    Collect(other.gameObject);
                }
            }
        }
        public virtual void Collect(GameObject collector)
        {
            if (isCollected) return;
            isCollected = true;
            PerformCollectAction(collector);
            PlayEffects();
            DisableAndDestroy();
        }
        protected abstract void PerformCollectAction(GameObject collector);
        protected virtual void PlayEffects()
        {
            IAudioService audioService = Core.ServiceLocator.Instance?.GetService<IAudioService>(); audioService?.PlaySoundAt(collectSoundName, transform.position);
            audioService?.PlaySoundAt(collectSoundName, transform.position);
            // Instantiate particle effect
            if (collectEffectPrefab != null)
            {
                GameObject effect = Instantiate(collectEffectPrefab,
                transform.position, Quaternion.identity);
                Destroy(effect, 2f);
            }
        }
        protected virtual void DisableAndDestroy()
        {
            if (itemCollider != null) itemCollider.enabled = false;
            if (spriteRenderer != null) spriteRenderer.enabled = false;
            Destroy(gameObject, 0.5f);
        }
        public virtual int GetValue() => value;
        public virtual string GetIdentifier() => identifier;
    }
}
