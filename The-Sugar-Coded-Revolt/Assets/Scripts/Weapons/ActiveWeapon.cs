using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    [SerializeField] WeaponSO weaponSO;
   
    Animator animator;
    MyPlayerInput myPlayerInput;
    Weapon currentWeapon;

    const string SHOOT_STRING = "Shoot";

    private void Awake()
    {
        myPlayerInput = GetComponentInParent<MyPlayerInput>();
        animator = GetComponent<Animator>();

    }

    private void Start()
    {
        currentWeapon = GetComponentInChildren<Weapon>();
    }
    void Update()
    {
        HandleShoot();
    }

    private void HandleShoot()
    {
        if (!myPlayerInput.shoot)
            return;

        currentWeapon.Shoot(weaponSO);
        animator.Play(SHOOT_STRING, 0, 0f);
        myPlayerInput.ShootInput(false);


    }
}
