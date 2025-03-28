using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public GameField gameField;

    void Update()
    {
        if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            Debug.Log("Up key pressed");
            gameField.ProcessMove(Vector2Int.down);
        }
        else if (Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            Debug.Log("Down key pressed");
            gameField.ProcessMove(Vector2Int.up);
        }
        else if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            Debug.Log("Left key pressed");
            gameField.ProcessMove(Vector2Int.left);
        }
        else if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            Debug.Log("Right key pressed");
            gameField.ProcessMove(Vector2Int.right);
        }
    }
}