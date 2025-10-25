using UnityEngine;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Core;
using Assets.Scripts.Patterns.Singleton;
using UnityEngine.EventSystems; 

namespace Assets.Scripts.Services
{
    public class MobileInputService : SingletonMonoBehaviour<MobileInputService>, IInputService
    {
        [Header("UI References (Assign in Inspector)")]
        [SerializeField] private RectTransform joystickBackground;
        [SerializeField] private RectTransform joystickHandle;
        [SerializeField] private float joystickMovementRange = 75f;
        [SerializeField] private GameObject mobileControlsCanvas; 

        private Vector2 joystickInput = Vector2.zero;
        private Vector2 joystickStartPos;
        private int joystickPointerId = -1; // To track which touch is controlling the joystick

        private bool jumpButtonDown = false;
        private bool jumpButtonHeld = false;
        private bool pauseButtonDown = false; 

        private bool inputEnabled = true;

        protected override void Awake()
        {
            base.Awake();
            if (Instance == this)
            {
                ServiceLocator.Instance.RegisterService<IInputService>(this);
                if (mobileControlsCanvas != null)
                {
                    mobileControlsCanvas.SetActive(true);
                }
            }
            else
            {
                if (mobileControlsCanvas != null)
                {
                    Destroy(mobileControlsCanvas);
                }
            }

            #if !UNITY_ANDROID && !UNITY_IOS
            if (mobileControlsCanvas != null) mobileControlsCanvas.SetActive(false);
            #endif
        }

        private void Start()
        {
            #if UNITY_ANDROID || UNITY_IOS
            if (joystickBackground != null)
            {
                joystickStartPos = joystickBackground.position;
            }
            else
            {
                Debug.LogWarning("[MobileInputService] Joystick Background not assigned. Mobile movement will not work.");
            }
            #endif
        }

        private void Update()
        {
            #if UNITY_ANDROID || UNITY_IOS
            if (!inputEnabled || joystickBackground == null || joystickHandle == null)
            {
                joystickInput = Vector2.zero;
                return;
            }

            HandleJoystickInput();
            #endif

            jumpButtonDown = false;
            pauseButtonDown = false;
        }

        private void HandleJoystickInput()
        {
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);
                    if (joystickPointerId == -1 && RectTransformUtility.RectangleContainsScreenPoint(joystickBackground, touch.position))
                    {
                        // First touch on joystick area
                        joystickPointerId = touch.fingerId;
                        joystickStartPos = joystickBackground.position; // Lock joystick position to first touch
                    }

                    if (touch.fingerId == joystickPointerId)
                    {
                        if (touch.phase == TouchPhase.Began)
                        {
                            joystickStartPos = joystickBackground.position; // Recalculate if joystick moved
                        }
                        if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                        {
                            Vector2 currentPos = touch.position;
                            Vector2 direction = (currentPos - joystickStartPos);
                            float distance = direction.magnitude;

                            if (distance > joystickMovementRange)
                            {
                                direction = direction.normalized * joystickMovementRange;
                            }

                            joystickHandle.position = joystickStartPos + direction;
                            joystickInput = direction.normalized * (distance / joystickMovementRange); // Normalize and scale
                            joystickInput.y = 0; // Only horizontal movement for now

                        }
                        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                        {
                            ResetJoystick();
                        }
                    }
                }
            }
            else if (joystickPointerId != -1) 
            {
                ResetJoystick();
            }
        }

        private void ResetJoystick()
        {
            joystickInput = Vector2.zero;
            joystickHandle.position = joystickStartPos;
            joystickPointerId = -1;
        }

        public Vector2 GetMovementInput()
        {
            if (!inputEnabled) return Vector2.zero;
            return joystickInput;
        }

        public bool GetJumpInputHeld()
        {
            if (!inputEnabled) return false;
            return jumpButtonHeld;
        }

        public bool GetJumpInputDown()
        {
            if (!inputEnabled) return false;
            return jumpButtonDown;
        }

        public bool GetPauseInput()
        {
            if (!inputEnabled) return false;
            return pauseButtonDown;
        }

        public void SetInputEnabled(bool enabled)
        {
            inputEnabled = enabled;
            if (!enabled)
            {
                ResetJoystick();
                jumpButtonHeld = false;
                jumpButtonDown = false;
                pauseButtonDown = false;
            }
        }

        // --- UI Callbacks (Assigned to UI Buttons in Inspector) ---
        public void OnJumpButtonDown()
        {
            if (inputEnabled)
            {
                jumpButtonDown = true;
                jumpButtonHeld = true;
            }
        }

        public void OnJumpButtonUp()
        {
            if (inputEnabled)
            {
                jumpButtonHeld = false;
            }
        }

        public void OnPauseButtonDown() // For a dedicated mobile pause button
        {
            if (inputEnabled)
            {
                pauseButtonDown = true;
            }
        }

        protected override void OnDestroy()
        {
            if (ServiceLocator.Instance != null && ServiceLocator.Instance.HasService<IInputService>() && ServiceLocator.Instance.GetService<IInputService>() == this)
            {
                ServiceLocator.Instance.UnregisterService<IInputService>();
            }
            base.OnDestroy();
        }
    }
}