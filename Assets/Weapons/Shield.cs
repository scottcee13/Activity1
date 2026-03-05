using UnityEngine;

public class Shield : MonoBehaviour, IWeapon
{
    public void Attack()
    {
        Debug.Log("Thud! Bashing with Shield");
    }
}
