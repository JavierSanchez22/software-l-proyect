using UnityEngine;
using Assets.Scripts.Core;
using Assets.Scripts.Services;
using Assets.Scripts.Interfaces;

namespace Assets.Scripts.Core
{
    public class GameInitializer : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private GameConfig gameConfig;
        [Tooltip("Assign the PREFAB containing MobileInputService and its UI Canvas")]
        [SerializeField] private GameObject mobileInputPrefab; // Asigna el PREFAB aquí

        private void Awake()
        {
            // Evitar que se ejecute múltiples veces si ya existe otro Initializer
            // (Aunque no debería pasar si está en un GO dedicado y no es Singleton)
            InitializeGame();
        }

        private void InitializeGame()
        {
            // --- Configuración Inicial ---
            if (gameConfig != null)
            {
                Application.targetFrameRate = gameConfig.targetFrameRate;
            }
            else { Debug.LogError("[GameInitializer] GameConfig is not assigned!"); }

            var serviceLocator = ServiceLocator.Instance; // Asegura que exista el ServiceLocator

            // --- Selección e Instanciación/Registro del Input Service ---
            IInputService inputServiceInstance = null; // Variable para guardar la instancia correcta

            #if UNITY_ANDROID || UNITY_IOS
                // --- Lógica para Móvil ---
                // Intenta encontrar si ya existe una instancia (quizás de una escena anterior)
                MobileInputService mobileInstance = FindObjectOfType<MobileInputService>();
                if (mobileInstance == null && mobileInputPrefab != null)
                {
                    // Si no existe y tenemos prefab, instáncialo
                    GameObject go = Instantiate(mobileInputPrefab);
                    mobileInstance = go.GetComponent<MobileInputService>(); // El Singleton se registrará en su Awake
                    DontDestroyOnLoad(go); // Hazlo persistente si los controles deben sobrevivir cargas de escena
                    Debug.Log("[GameInitializer] Instantiated MobileInputService from Prefab.");
                }
                else if (mobileInstance == null)
                {
                    Debug.LogError("[GameInitializer] MobileInputService instance not found and prefab not assigned!");
                }
                // Obtén la referencia a través de la propiedad Instance (forzará Awake si es necesario)
                inputServiceInstance = MobileInputService.Instance;
                Debug.Log("[GameInitializer] Using MobileInputService.");

            #else
                // --- Lógica para PC/Editor ---
                // Simplemente accede a la propiedad Instance. El Singleton se creará si no existe.
                inputServiceInstance = InputService.Instance; // Forzará Awake/Creación y Registro si no existe
                Debug.Log("[GameInitializer] Using Standard InputService.");
            #endif

            // --- Registro Final (Doble Check) ---
            // Aunque los Singletons se auto-registran, nos aseguramos aquí
            // que la instancia correcta esté registrada COMO IInputService.
            if (inputServiceInstance != null)
            {
                 // Si ya existe un IInputService registrado, podría ser uno incorrecto
                 // (ej. el de PC en móvil), así que lo reemplazamos si es necesario.
                 if (serviceLocator.HasService<IInputService>() && serviceLocator.GetService<IInputService>() != inputServiceInstance)
                 {
                      Debug.LogWarning($"[GameInitializer] Replacing existing IInputService registration with the correct one for this platform.");
                      // No necesitamos desregistrar explícitamente si RegisterService sobrescribe
                 }
                 serviceLocator.RegisterService<IInputService>(inputServiceInstance); // Registra o Sobrescribe
            }
            else
            {
                 Debug.LogError("[GameInitializer] Failed to obtain an Input Service instance!");
            }

            // --- Forzar Inicialización de Otros Servicios Esenciales ---
            // Simplemente acceder a .Instance fuerza su Awake si aún no se han creado.
            var audioService = AudioService.Instance;
            var saveService = SaveService.Instance;
            var scoreService = ScoreService.Instance;

            // --- Inicializar GameManager (último) ---
            var gameManager = GameManager.Instance; // Forzará su Awake

            Debug.Log("[GameInitializer] Core systems initialization sequence complete.");
        }
    }
}