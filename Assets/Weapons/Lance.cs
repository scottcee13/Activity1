using UnityEngine;

public class Lance : MonoBehaviour, IWeapon
{
    public void Attack()
    {
        Debug.Log("Sha! Piercing with Lance");
    }
}
