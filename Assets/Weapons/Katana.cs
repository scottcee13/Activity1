using UnityEngine;

public class Katana : MonoBehaviour, IWeapon
{
    public void Attack()
    {
        Debug.Log("It's Katana-ing Time!");
    }
}
