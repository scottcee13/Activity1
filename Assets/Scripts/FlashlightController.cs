using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightController : MonoBehaviour
{
    public GameObject flashlightObject;
    public Animator animator;

    private bool isEquipped = false;

    public void OnEquipTorch(InputAction.CallbackContext context)
    {
        Debug.Log("Equip Torch Pressed");
        if (!context.started) return;

        isEquipped = !isEquipped;

        animator.SetBool("flashlightEquipped", isEquipped);

        if (isEquipped)
            flashlightObject.SetActive(true);
    }

    // Called via animation event at end of unequip animation
    public void DisableFlashlightObject()
    {
        if (!isEquipped)
            flashlightObject.SetActive(false);
    }
}