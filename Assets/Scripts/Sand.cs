using UnityEngine;

public class Sand : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MeshRenderer meshRenderer;

    [Header("Params")]
    [SerializeField] private GameObject fullSand;
    [SerializeField] private GameObject dugSand;
    [SerializeField, Range(0.0f, 1.0f)] private float highlightDarkenFactor = 1.0f;

    [Header("State")]
    public bool isFull;
    private bool isHighlighted = false;

    private void Start()
    {
        Fill();
    }

    public void Dig()
    {
        isFull = false;
        fullSand.SetActive(false);
        dugSand.SetActive(true);
        Debug.Log($"{gameObject.name} was dug");
    }

    public void Fill()
    {
        isFull = true;
        fullSand.SetActive(true);
        dugSand.SetActive(false);
        Debug.Log($"{gameObject.name} was filled");
    }

    public void EnableHighlight()
    {
        if (!isHighlighted)
        {
            meshRenderer.material.color *= highlightDarkenFactor;
            isHighlighted = true;
        }
    }

    public void DisableHighlight()
    {
        if (isHighlighted)
        {
            meshRenderer.material.color /= highlightDarkenFactor;
            isHighlighted = false;
        }
    }
} 
