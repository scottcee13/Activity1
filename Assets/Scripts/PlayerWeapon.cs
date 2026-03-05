using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{

    [SerializeField] private WeaponData[] weaponObjects;

    private int currentIndex = 0;
    private float lastAttackTime = -Mathf.Infinity;

    void Start()
    {
        currentIndex = 0;
    }

    private void OnEnable()
    {
        CharacterControllerMovement.OnAttack += Attack;
    }

    private void OnDisable()
    {
        CharacterControllerMovement.OnAttack -= Attack;
    }


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchWeapon(-1);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchWeapon(1);
        }
    }

    void SwitchWeapon(int direction)
    {
        currentIndex += direction;

        if (currentIndex >= weaponObjects.Length) currentIndex = 0;
        else if (currentIndex < 0) currentIndex = weaponObjects.Length - 1;
    }

    public bool Attack()
    {
        WeaponData currentWeapon = weaponObjects[currentIndex];

        float nextReadyTime = lastAttackTime + currentWeapon.attackCooldown;
        float remainingTime = nextReadyTime - Time.time;

        if (Time.time >= nextReadyTime)
        {
            lastAttackTime = Time.time;

            Debug.Log(currentWeapon.weaponName + ": " +
                "Dealt " + currentWeapon.weaponDamage + " damage!\n" +
                currentWeapon.weaponDescription + " (" +
                currentWeapon.attackCooldown + " second(s) cooldown.)");
            return true;
        }
        else
        {
            Debug.Log(currentWeapon.weaponName +
            " is on cooldown! Ready in " +
            remainingTime.ToString("F2") + "s");
            return false;
        }
    }

}

