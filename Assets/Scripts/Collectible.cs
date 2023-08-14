using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour, IInteractable
{
    public GameObject Pistol;
    public GameObject Rifle;
    public WeaponSwitching _ws;

    public GameBehavior gameBehavior;

    private void Start()
    {
        gameBehavior = GameObject.Find("GameBehavior").GetComponent<GameBehavior>();
    }

    public void Interact()
    {
        if (Pistol.activeSelf == false && Rifle.activeSelf == false)
        {
            Pistol.SetActive(true);
            gameBehavior.WeaponPickedup();
            Destroy(this.gameObject);
        }
        else
        {
            Pistol.SetActive(false);
            Rifle.SetActive(true);
            _ws.EquipRifle();
            Destroy(this.gameObject);
        }
    }
}
