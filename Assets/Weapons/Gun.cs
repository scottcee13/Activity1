using UnityEngine;

public class Gun : MonoBehaviour, IWeapon
{
    public void Attack()
    {
        Debug.Log("Bang! Shooting with Gun");
    }
}
