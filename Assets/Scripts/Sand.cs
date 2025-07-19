using UnityEngine;

public class Sand : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MeshFilter filter;
    [SerializeField] private MeshRenderer renderer;

    [Header("Params")]
    [SerializeField] private Mesh fullMesh;
    [SerializeField] private Mesh dugMesh;
    [SerializeField] private Material fullMaterial;
    [SerializeField] private Material dugMaterial;
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
        filter.mesh = dugMesh;
        renderer.material = dugMaterial;
        Debug.Log($"{gameObject.name} was dug");
    }

    public void Fill()
    {
        isFull = true;
        filter.mesh = fullMesh;
        renderer.material = fullMaterial;
        Debug.Log($"{gameObject.name} was filled");
    }

    public void EnableHighlight()
    {
        if (!isHighlighted)
        {
            renderer.material.color *= highlightDarkenFactor;
            isHighlighted = true;
        }
    }

    public void DisableHighlight()
    {
        if (isHighlighted)
        {
            renderer.material.color /= highlightDarkenFactor;
            isHighlighted = false;
        }
    }
} 