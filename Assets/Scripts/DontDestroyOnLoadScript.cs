using UnityEngine;

public class DontDestroyOnLoadScript : MonoBehaviour
{
    private static DontDestroyOnLoadScript instance;

    private void Awake()
    {
        // Check if an instance of this script already exists
        if (instance == null)
        {
            // If not, set the instance to this script and mark it as "Don't Destroy On Load"
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If an instance already exists, destroy this duplicate
            Destroy(gameObject);
        }
    }
}

