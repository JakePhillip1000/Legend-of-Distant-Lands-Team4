using UnityEngine;
using TMPro;
using System;

public class DialoguePresenter : MonoBehaviour
{
    [Header("World Dialog Setting")]
    [SerializeField] private GameObject worldRoot;
    [SerializeField] private TMP_Text worldText;
    [SerializeField] private TMP_Text worldSpeakerText;
    [SerializeField] private Transform currentAnchor;

    [Header("Subtitle Dialog Setting")]
    [SerializeField] private GameObject subtitleRoot;
    [SerializeField] private TMP_Text subtitleText;
    [SerializeField] private TMP_Text subtitleSpeakerText;

    [Header("Debug")]
    [SerializeField] private bool hasActiveDialogue = false;

    public enum PresentationMode
    {
        Hidden,
        World,
        Subtitle
    }

    private void LateUpdate()
    {
        if (currentAnchor == null) return;

        worldRoot.transform.position = currentAnchor.position;
        UpdateBillboard();
    }
    public void BeginDialogue(
        string message,
        string speaker,
        Transform anchor)
    {
        SetContent(message, speaker);
        SetAnchor(anchor);

        bool visible = visibilityResolver.InitializeTarget(anchor);

        hasActiveDialogue = true;
        HandleVisibilityChanged(visible);
    }
    public void EndDialogue()
    {
        hasActiveDialogue = false;
        SetMode(PresentationMode.Hidden);
    }
    public void SetContent(string message, string speaker)
    {
        worldText.text = message;
        subtitleText.text = message;
        worldSpeakerText.text = $"( {speaker} )";
        subtitleSpeakerText.text = $"( {speaker} )";
    }
    public void SetMode(PresentationMode mode)
    {
        worldRoot.SetActive(mode == PresentationMode.World);
        subtitleRoot.SetActive(mode == PresentationMode.Subtitle);
    }
    public void SetAnchor(Transform anchor)
    {
        currentAnchor.position = anchor.position;
    }

    [Header("Billboard Setting")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private bool lockVertical = false;
    private void UpdateBillboard()
    {
        if (cameraTransform == null) return;

        Vector3 direction = cameraTransform.position 
                            - worldRoot.transform.position;
        
        if (lockVertical) direction.y = 0;

        if (direction.sqrMagnitude < 0.0000001f) return;

        worldRoot.transform.rotation = 
            Quaternion.LookRotation(-direction, Vector3.up);
    }

    [Header("Visibility Resolver")]
    [SerializeField] private DialogueVisibilityResolver visibilityResolver;
    private void OnEnable()
    {
        if (visibilityResolver == null) return;
        
        visibilityResolver.OnVisibilityChanged += HandleVisibilityChanged;
    }
    private void OnDisable()
    {
        if (visibilityResolver == null) return;

        visibilityResolver.OnVisibilityChanged += HandleVisibilityChanged;
    }
    private void HandleVisibilityChanged(bool visible)
    {
        if (!hasActiveDialogue) return;

        SetMode(visible
        ? PresentationMode.World
        : PresentationMode.Subtitle);
    }





    // -------------------------------------------------------------------------
    // dead code for testing refactored is upper
    public void Present(string message, string speaker, PresentationMode mode)
    {
        worldText.text = message;
        subtitleText.text = message;
        worldSpeakerText.text = $"( {speaker} )";
        subtitleSpeakerText.text = $"( {speaker} )";

        worldRoot.SetActive(mode == PresentationMode.World);
        subtitleRoot.SetActive(mode == PresentationMode.Subtitle);
    }

    [ContextMenu("Test/Show World")]
    private void TestWorld()
    {
        Present("Hello", "Speaker", PresentationMode.World);
    }

    [ContextMenu("Test/Show Subtitle")]
    private void TestSubtitle()
    {
        Present("Hello", "Speaker", PresentationMode.Subtitle);
    }

    [ContextMenu("Test/Show Hidden")]
    private void TestHidden()
    {
        Present("Hello", "Speaker", PresentationMode.Hidden);
    }
    // -------------------------------------------------------------------------
}