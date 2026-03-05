using UnityEngine;

public class Dagger : MonoBehaviour, IWeapon
{
    public void Attack()
    {
        Debug.Log("Stab! Stabbing with Dagger");
    }
}
