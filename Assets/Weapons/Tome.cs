using UnityEngine;

public class Tome : MonoBehaviour, IWeapon
{
    public void Attack()
    {
        Debug.Log("It's Toming Time!");
    }
}
