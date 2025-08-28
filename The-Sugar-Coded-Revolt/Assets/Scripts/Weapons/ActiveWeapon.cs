using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    [SerializeField] WeaponSO weaponSO;

    Animator animator;
    MyPlayerInput myPlayerInput;
    Weapon currentWeapon;
    const string SHOOT_STRING = "Shoot";

    float timeSinceLastShoot = 0f;

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
        HandleZoom();
    }

    public void SwitchWeapon(WeaponSO weaponSO)
    {
        if (currentWeapon)
        {
            Destroy(currentWeapon.gameObject);
        }

        Weapon newWeapon = Instantiate(weaponSO.weaponPrefab, transform).GetComponent <Weapon>();
        currentWeapon = newWeapon;
        this.weaponSO = weaponSO;
    }
    void HandleShoot()
    {
        timeSinceLastShoot += Time.deltaTime;

        if (!myPlayerInput.shoot) return;

        if (timeSinceLastShoot >= weaponSO.FireRate)
        {
            currentWeapon.Shoot(weaponSO);
            animator.Play(SHOOT_STRING, 0, 0f);
            timeSinceLastShoot = 0f;
        }

        if (!weaponSO.IsAutomatic)
        {
            myPlayerInput.ShootInput(false);
        }
    }

    void HandleZoom()
    {
        if (!weaponSO.CanZoom) return;

        if (myPlayerInput.zoom)
        {
            Debug.Log("Zooming in");
        }
        else
        {
            Debug.Log("Not Zooming In");
        }
    }
}
