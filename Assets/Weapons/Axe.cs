using UnityEngine;

public class Axe : MonoBehaviour, IWeapon
{
    public void Attack()
    {
        Debug.Log("Hack! Swinging with Axe");
    }
}