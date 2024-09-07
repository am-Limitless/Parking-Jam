using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Called when the game starts
    private void Start()
    {
        LockCursor();
    }

    // Locks the cursor and makes it invisible
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked; // Locks the cursor to the center of the screen
        Cursor.visible = false;                   // Hides the cursor
    }
}
