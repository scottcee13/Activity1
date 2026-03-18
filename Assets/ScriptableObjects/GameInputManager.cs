using UnityEngine;

public class GameInputManager : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject shopPanel;

    public SaveSystem saveSystem;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            shopPanel.SetActive(!shopPanel.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            saveSystem.ResetSave();
        }
    }
}