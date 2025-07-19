using UnityEngine;

public class AbilityWheel : MonoBehaviour
{
    [SerializeField] private GameObject abilityWheel;
    [SerializeField] private Transform abilityContainer;
    [SerializeField] private float abilityRadialDistance = 50.0f;

    public int abilityCount = 0;
    public int selectedAbilityIndex = 0;

    private void Awake()
    {
        DoLayout();
        Close();
    }

    public void Open()
    {
        abilityWheel.SetActive(true);
    }

    public void Close()
    {
        abilityWheel.SetActive(false);
    }

    public void AddAbility(Transform ability)
    {
        abilityCount++;
        ability.SetParent(abilityContainer);
        DoLayout();
    }

    public void SelectPreviousAbility()
    {
        selectedAbilityIndex = (selectedAbilityIndex - 1) % abilityCount;
    }

    public void SelectNextAbility()
    {
        selectedAbilityIndex = (selectedAbilityIndex + 1) % abilityCount;
    }

    private void DoLayout()
    {
        for (int i = 0; i < abilityContainer.childCount; ++i)
        {
            float theta = (float)i / abilityContainer.childCount * 2.0f * Mathf.PI;
            Vector2 direction = new(Mathf.Sin(theta), Mathf.Cos(theta));
            Vector2 position = abilityRadialDistance * direction;
            abilityContainer.GetChild(i).localPosition = position;
        }
    }
}
