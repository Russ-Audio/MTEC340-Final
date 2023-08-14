using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSwitch : MonoBehaviour
{
    public DoorBehavior doorBehavior;

    public void SetSwitch()
    {
        doorBehavior.DoorOpen();
        Destroy(this.gameObject);
    }
}
