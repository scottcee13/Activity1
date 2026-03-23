using UnityEngine;

public class GameInputManager : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject shopPanel;

    public SaveSystem saveSystem;

    public AudioClip openSound;
    public AudioClip closeSound;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            bool isOpen = inventoryPanel.activeSelf;
            inventoryPanel.SetActive(!isOpen);
            if (!isOpen)
                AudioManager.Instance.PlayUI(openSound);
            else
                AudioManager.Instance.PlayUI(closeSound);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            bool isOpen = shopPanel.activeSelf;
            shopPanel.SetActive(!isOpen);
            if (!isOpen)
                AudioManager.Instance.PlayUI(openSound);
            else
                AudioManager.Instance.PlayUI(closeSound);
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            saveSystem.ResetSave();
        }
    }
}