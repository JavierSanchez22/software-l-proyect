using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface IInputService
    {
        Vector2 GetMovementInput(); // Typically just X for platformer
        bool GetJumpInputDown();    // Pressed this frame
        bool GetJumpInputHeld();    // Held down
        bool GetPauseInput();       // Pressed this frame
        void SetInputEnabled(bool enabled);
    }
}