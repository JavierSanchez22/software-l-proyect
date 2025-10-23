using UnityEngine;
using DinoRunner.Interfaces; // Necesita saber qu� interfaz implementa
using DinoRunner.Core;      // Necesita el ServiceLocator
using DinoRunner.Patterns.Singleton; // Necesita la base Singleton

namespace DinoRunner.Services
{
    // Esta CLASE implementa la INTERFAZ IInputService
    public class InputService : SingletonMonoBehaviour<InputService>, IInputService
    {
        private bool inputEnabled = true;

        protected override void Awake()
        {
            base.Awake();
            // Solo se registra si es la instancia Singleton real
            if (Instance == this && ServiceLocator.Instance != null)
            {
                // Solo registrar si A�N NO HAY un IInputService registrado
                // (GameInitializer decidir� cu�l registrar al final)
                if (!ServiceLocator.Instance.HasService<IInputService>())
                {
                    ServiceLocator.Instance.RegisterService<IInputService>(this);
                    // Debug.Log("[InputService] Registered itself.");
                }
                else
                {
                    // Si ya hay uno (probablemente Mobile), este de PC se destruye.
                    // Debug.LogWarning("[InputService] Another IInputService already registered. Destroying PC InputService.");
                    Destroy(gameObject);
                }
            }
        }

        // --- Implementaci�n de los m�todos definidos en IInputService ---

        public Vector2 GetMovementInput()
        {
            if (!inputEnabled) return Vector2.zero;
            // Usa el Input Manager de Unity para leer "Horizontal" (A/D, Flechas Izq/Der)
            return new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        }

        public bool GetJumpInputHeld() // El nombre en la interfaz es GetJumpInputHeld
        {
            if (!inputEnabled) return false;
            // Usa el Input Manager de Unity para leer "Jump" (Espacio por defecto)
            return Input.GetButton("Jump");
        }

        public bool GetJumpInputDown()
        {
            if (!inputEnabled) return false;
            // Usa el Input Manager de Unity para leer si "Jump" se presion� este frame
            return Input.GetButtonDown("Jump");
        }

        public bool GetPauseInput()
        {
            if (!inputEnabled) return false;
            // Lee las teclas Escape o P
            return Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P);
        }

        public void SetInputEnabled(bool enabled)
        {
            inputEnabled = enabled;
        }

        // --- Limpieza al destruir ---
        protected override void OnDestroy()
        {
            // Aseg�rate de desregistrarte si eras t� el servicio activo
            if (ServiceLocator.Instance != null && ServiceLocator.Instance.HasService<IInputService>() && ServiceLocator.Instance.GetService<IInputService>() == this)
            {
                ServiceLocator.Instance.UnregisterService<IInputService>();
                // Debug.Log("[InputService] Unregistered itself.");
            }
            base.OnDestroy();
        }
    }
}