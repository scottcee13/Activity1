using UnityEngine;

public class Claws : MonoBehaviour, IWeapon
{
    public void Attack()
    {
        Debug.Log("It's Clawing Time!");
    }
}
