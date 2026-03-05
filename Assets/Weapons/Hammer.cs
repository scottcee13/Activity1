using UnityEngine;

public class Hammer : MonoBehaviour, IWeapon
{
    public void Attack()
    {
        Debug.Log("Slam! Crushing with Hammer");
    }
}
