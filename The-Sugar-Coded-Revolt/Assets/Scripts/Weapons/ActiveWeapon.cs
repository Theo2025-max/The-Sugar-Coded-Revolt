using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    [SerializeField] WeaponSO startingWeapon;
    [SerializeField] CinemachineCamera playerFollowCamera;
    [SerializeField] GameObject zoomVignette;
    [SerializeField] TMP_Text ammoText;

    WeaponSO CurrentWeaponSO;
    Animator animator;
    MyPlayerInput myPlayerInput;
    MyFirstPersonController myFirstPersonController;
    Weapon currentWeapon;

    const string SHOOT_STRING = "Shoot";

    float timeSinceLastShoot = 0f;
    float defaultFOV;
    float defaultRotationSpeed;
    int currentAmmo;

    private void Awake()
    {
        myPlayerInput = GetComponentInParent<MyPlayerInput>();
        myFirstPersonController = GetComponentInParent<MyFirstPersonController>();
        animator = GetComponent<Animator>();
        defaultFOV = playerFollowCamera.Lens.FieldOfView;
        defaultRotationSpeed = myFirstPersonController.rotationSpeed;

    }

    private void Start()
    {
        SwitchWeapon(startingWeapon);
        AdjustAmmo(CurrentWeaponSO.MagazineSize);
    }

    void Update()
    {
        HandleShoot();
        HandleZoom();
    }

    public void AdjustAmmo(int amount)
    {
        currentAmmo += amount;

        if (currentAmmo > CurrentWeaponSO.MagazineSize)
        {
            currentAmmo = CurrentWeaponSO.MagazineSize;
        }

        ammoText.text = currentAmmo.ToString("D2");

    }

    public void SwitchWeapon(WeaponSO weaponSO)
    {
        if (currentWeapon)
        {
            Destroy(currentWeapon.gameObject);
        }

        Weapon newWeapon = Instantiate(weaponSO.weaponPrefab, transform).GetComponent <Weapon>();
        currentWeapon = newWeapon;
        this.CurrentWeaponSO = weaponSO;
        AdjustAmmo(CurrentWeaponSO.MagazineSize);
    }
    void HandleShoot()
    {
        timeSinceLastShoot += Time.deltaTime;

        if (!myPlayerInput.shoot) return;

        if (timeSinceLastShoot >= CurrentWeaponSO.FireRate &&  currentAmmo > 0)
        {
            currentWeapon.Shoot(CurrentWeaponSO);
            animator.Play(SHOOT_STRING, 0, 0f);
            timeSinceLastShoot = 0f;
            AdjustAmmo(-1);
        }

        if (!CurrentWeaponSO.IsAutomatic)
        {
            myPlayerInput.ShootInput(false);
        }
    }

    void HandleZoom()
    {
        if (!CurrentWeaponSO.CanZoom) return;

        if (myPlayerInput.zoom)
        {
            playerFollowCamera.Lens.FieldOfView = CurrentWeaponSO.ZoomAmount;
            zoomVignette.SetActive(true);
            myFirstPersonController.ChangeRotationSpeed(CurrentWeaponSO.ZoomRotationSpeed);
        }
        else
        {
            playerFollowCamera.Lens.FieldOfView = defaultFOV;
            zoomVignette.SetActive(false);
            myFirstPersonController.ChangeRotationSpeed(defaultRotationSpeed);
        }
    }
}
