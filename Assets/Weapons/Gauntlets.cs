using UnityEngine;

public class Gauntlets : MonoBehaviour, IWeapon
{
    public void Attack()
    {
        Debug.Log("Wham! Punching with Gauntlets");
    }
}
