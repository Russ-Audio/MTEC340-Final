using UnityEngine;
using UnityEngine.UI;

public class BringImageForward : MonoBehaviour
{
    public Canvas canvas; // Reference to the Canvas component
    //public Image image;   // Reference to the Image component

    private void Start()
    {
        // Increase the sorting order of the Canvas to bring UI elements forward
        canvas.sortingOrder = 1;
    }
}
