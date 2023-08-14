using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health_Pickup : MonoBehaviour
{
    public int HealthValue;
    public float TimetoAvailable;

    public MeshRenderer cube1;
    public MeshRenderer cube2;

    public GameBehavior gameBehavior;

    private bool PickedUp = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !PickedUp)
        {
            gameBehavior.PlayerHeal(HealthValue);

            cube1.enabled = false;
            cube2.enabled = false;

            PickedUp = true;
            StartCoroutine(ResetHealthPickup());
        }
    }

    IEnumerator ResetHealthPickup()
    {
        yield return new WaitForSeconds(TimetoAvailable);
        PickedUp = false;
        cube1.enabled = true;
        cube2.enabled = true;
    }
}
