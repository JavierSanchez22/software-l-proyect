using UnityEngine;

namespace DinoRunner.MVC.Models.Level
{
    public abstract class Block : MonoBehaviour
    {
        [SerializeField] protected float blockWidth = 5f;

        public virtual float Width 
        { 
            get => blockWidth; 
            set => blockWidth = value; 
        }

        public virtual void Initialize() { }

        public virtual void OnCleanup() { }
    }
}