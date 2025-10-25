using UnityEngine;
using Assets.Scripts.Interfaces; 
using Assets.Scripts.Core;      
using Assets.Scripts.Patterns.Singleton; 

namespace Assets.Scripts.Services
{
    public class InputService : SingletonMonoBehaviour<InputService>, IInputService
    {
        private bool inputEnabled = true;

        protected override void Awake()
        {
            base.Awake();
            if (Instance == this && ServiceLocator.Instance != null)
            {
                if (!ServiceLocator.Instance.HasService<IInputService>())
                {
                    ServiceLocator.Instance.RegisterService<IInputService>(this);
                    // Debug.Log("[InputService] Registered itself.");
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

        public Vector2 GetMovementInput()
        {
            if (!inputEnabled) return Vector2.zero;
            return new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        }

        public bool GetJumpInputHeld() // El nombre en la interfaz es GetJumpInputHeld
        {
            if (!inputEnabled) return false;
            return Input.GetButton("Jump");
        }

        public bool GetJumpInputDown()
        {
            if (!inputEnabled) return false;
            return Input.GetButtonDown("Jump");
        }

        public bool GetPauseInput()
        {
            if (!inputEnabled) return false;
            return Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P);
        }

        public void SetInputEnabled(bool enabled)
        {
            inputEnabled = enabled;
        }

        // --- Limpieza al destruir ---
        protected override void OnDestroy()
        {
            if (ServiceLocator.Instance != null && ServiceLocator.Instance.HasService<IInputService>() && ServiceLocator.Instance.GetService<IInputService>() == this)
            {
                ServiceLocator.Instance.UnregisterService<IInputService>();
                // Debug.Log("[InputService] Unregistered itself.");
            }
            base.OnDestroy();
        }
    }
}