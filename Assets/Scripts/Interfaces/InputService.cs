using UnityEngine;

namespace DinoRunner.Interfaces
{
    public interface InputService
    {
        Vector2 GetMovementInput(); // Typically just X for platformer
        bool GetJumpInputDown();    // Pressed this frame
        bool GetJumpInputHeld();    // Held down
        bool GetPauseInput();       // Pressed this frame
        void SetInputEnabled(bool enabled);
    }
}