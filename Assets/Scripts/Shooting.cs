using UnityEngine;
using System.Collections;
using TMPro;

public class Shooting : MonoBehaviour
{
    public float weaponDamage;
    public float weaponRange;
    public int WeaponIndex;
    public float fireRate;

    public float maxVol;
    public float minVol;

    private int ShotIndex;
    private int PreviousShot = -1;

    public CameraShaker camShake;

    private bool pistolCD = true;
    private GameBehavior gameBehavior;

    public int MaxAmmo = 30;
    private int CurrentAmmo;
    public float reloadTime = 1f;
    private bool isReloading = false;
    //private bool isEquipped = false;

    public Camera fpsCam;

    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    private float nextFire = 0f;

    public Animator _anim;

    public TextMeshProUGUI ammoInfo;

    [SerializeField] AudioClip[] HitMarkSFX;
    [SerializeField] AudioClip[] ShootSFX;
    [SerializeField] AudioClip ReloadSFX;

    private AudioSource audioSource;

    private void Start()
    {
        CurrentAmmo = MaxAmmo;
        audioSource = GetComponent<AudioSource>();
        gameBehavior = GameObject.Find("GameBehavior").GetComponent<GameBehavior>();
    }

    private void OnEnable()
    {
        isReloading = false;
        _anim.SetBool("Reloading", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isReloading)
        {
            return;
        }

        if(CurrentAmmo <= 0 || Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(ReloadWeapon());
            return;
        }

        if (gameBehavior.isPaused == false && WeaponIndex == 0 && Input.GetMouseButton(0) && Time.time >= nextFire)
        {
            nextFire = Time.time + 1f / fireRate;
            Shoot();
        }

        if(gameBehavior.isPaused == false && WeaponIndex == 1 && Input.GetMouseButtonDown(0) && pistolCD == true)
        {
            Shoot();
            pistolCD = false;
            Invoke(nameof(nextAvailableTime), fireRate);
        }
    }

    void nextAvailableTime()
    {
        pistolCD = true;
    }

    void Shoot()
    {

        camShake.Shake();

        muzzleFlash.Play();

        //PlayOneShot ShootSFX
        do
        {
            ShotIndex = Random.Range(0, 4);
        }
        while (ShotIndex == PreviousShot);

        float randPitch = Random.Range(0.95f, 1.05f);
        float randVol = Random.Range(minVol, maxVol);

        audioSource.pitch = randPitch;
        audioSource.volume = randVol;

        audioSource.PlayOneShot(ShootSFX[ShotIndex]);

        RaycastHit Hit;

        CurrentAmmo--;

        ammoInfo.text = $"Ammunition: {CurrentAmmo.ToString()}/{MaxAmmo.ToString()}";

        if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out Hit, weaponRange))
        {
            //Debug.Log(Hit.transform.name);

            EnemyBehaviour enemy = Hit.transform.GetComponent<EnemyBehaviour>();

            if(enemy != null && enemy.anim.GetBool("Dead") == false)
            {
                enemy.TakeDamage(weaponDamage);
                //PlayOneShot HitMarkerSFX
            }

            DoorSwitch door = Hit.transform.GetComponent<DoorSwitch>();

            if(door != null)
            {
                door.SetSwitch();
            }

            GameObject destroyClone = Instantiate(impactEffect, Hit.point, Quaternion.LookRotation(Hit.normal));
            Destroy(destroyClone, 1f);
        }

        PreviousShot = ShotIndex;
    }

    IEnumerator ReloadWeapon()
    {
        //Debug.Log("Reloading...");

        audioSource.PlayOneShot(ReloadSFX, 0.5f);

        isReloading = true;
        _anim.SetBool("Reloading", true);

        yield return new WaitForSeconds(reloadTime - .25f);

        _anim.SetBool("Reloading", false);

        yield return new WaitForSeconds(.25f);

        CurrentAmmo = MaxAmmo;
        ammoInfo.text = $"Ammunition: {CurrentAmmo.ToString()}/{MaxAmmo.ToString()}";

        isReloading = false;
    }

}
