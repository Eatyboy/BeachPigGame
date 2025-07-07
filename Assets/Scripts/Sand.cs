using UnityEngine;

public class Sand : MonoBehaviour
{
    public bool isFull;

    private void Start()
    {
        isFull = true;
    }

    public void Dig()
    {
        if (isFull)
        {
            isFull = false;
        }
    }

    public void Highlight()
    {

    }
}
