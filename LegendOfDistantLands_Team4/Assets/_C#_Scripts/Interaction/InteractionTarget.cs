using UnityEngine;
using UnityEngine.Events;

public class InteractionTarget : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private string interactionId;
    [SerializeField] private string prompt;
    [SerializeField] private Transform anchor;
    [SerializeField, Min(0.1f)]
    private float dwellDuration = 1.5f;

    [Header("Action")]
    [SerializeField] private UnityEvent onSelected;

    public string InteractionId => interactionId;
    public string Prompt => prompt;
    public Transform Anchor => anchor;
    public float DwellDuration => dwellDuration;

    public void Select()
    {
        onSelected?.Invoke();
    }
}