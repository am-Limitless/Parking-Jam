using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool levelPassed = false;

    public bool pauseActive = false;

    public bool gameOver = false;

    // Called when the game starts
    private void Update()
    {
        if (!levelPassed && !pauseActive && !gameOver)
        {
            LockCursor();
        }
        else if (gameOver)
        {
            FreeCursor();
        }
        else
        {
            FreeCursor();
        }
    }

    // Locks the cursor and makes it invisible
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked; // Locks the cursor to the center of the screen
        Cursor.visible = false;                   // Hides the cursor
    }

    private void FreeCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
}
