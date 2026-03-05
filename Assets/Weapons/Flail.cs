using UnityEngine;

public class Flail : MonoBehaviour, IWeapon
{
    public void Attack()
    {
        Debug.Log("It's Flailing Time!");
    }
}
