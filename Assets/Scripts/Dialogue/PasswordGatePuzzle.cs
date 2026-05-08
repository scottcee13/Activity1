using UnityEngine;
using UnityEngine.UI;

public class PasswordGatePuzzle : MonoBehaviour
{
    [SerializeField] private string password = "SUN";
    [SerializeField] private InputField inputField;
    [SerializeField] private Text feedbackText;
    [SerializeField] private DungeonSectionGate gateToOpen;

    public void SubmitPassword()
    {
        if (inputField == null) return;

        string typed = inputField.text.Trim().ToUpperInvariant();
        if (typed == password.ToUpperInvariant())
        {
            feedbackText.text = "Correct password!";
            gateToOpen?.OpenGate();
            DungeonProgressManager.Instance?.UnlockNextSection();
        }
        else
        {
            feedbackText.text = "Wrong password.";
        }
    }
}
