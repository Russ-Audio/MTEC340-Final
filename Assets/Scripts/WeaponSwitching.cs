using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    public int SelectedWeapon = 0;
    public bool isEquippedRifle = false;

    [SerializeField] AudioClip WeaponSwitch;

    private AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        int PreviousSelectedWeapon = SelectedWeapon;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if(SelectedWeapon == 0 && isEquippedRifle)
            {
                SelectedWeapon = 1;
            }
            else
            {
                SelectedWeapon = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectedWeapon = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectedWeapon = 1;
        }

        if(PreviousSelectedWeapon != SelectedWeapon)
        {
            SelectWeapon();
            float randPitch = Random.Range(0.8f, 0.9f);
            audioSource.pitch = randPitch;
            audioSource.PlayOneShot(WeaponSwitch, 0.7f);
        }
    }

    void SelectWeapon()
    {
        int i = 0;

        foreach(Transform weapon in transform)
        {
            if(i == SelectedWeapon)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }

            i++;
        }
    }

    public void EquipRifle()
    {
        isEquippedRifle = true;
    }
}
