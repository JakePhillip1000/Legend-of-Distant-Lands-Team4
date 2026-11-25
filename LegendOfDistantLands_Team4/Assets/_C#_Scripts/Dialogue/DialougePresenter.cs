using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class DialoguePresenter : MonoBehaviour
{
    [Header("World Dialog Setting")]
    [SerializeField] private GameObject worldRoot;
    [SerializeField] private TMP_Text worldText;
    [SerializeField] private TMP_Text worldSpeakerText;
    [SerializeField] private Transform currentAnchor;
    [SerializeField] private bool alwaysOpenWorldDialog = false;

    [Header("Subtitle Dialog Setting")]
    [SerializeField] private GameObject subtitleRoot;
    [SerializeField] private TMP_Text subtitleText;
    [SerializeField] private TMP_Text subtitleSpeakerText;
    
    [Header("Visibility Resolver")]
    [SerializeField] private DialogueVisibilityResolver visibilityResolver;

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
        if (currentAnchor == null || worldRoot == null) return;

        worldRoot.transform.position = currentAnchor.position;
        UpdateBillboard();
    }
    private void Awake()
    {
        hasActiveDialogue = false;
        SetModeImmediate(PresentationMode.Hidden);
    }
    private void OnEnable()
    {
        if (visibilityResolver == null) return;
        
        visibilityResolver.OnVisibilityChanged += HandleVisibilityChanged;
    }
    private void OnDisable()
    {
        CancelFade();

        if (visibilityResolver == null) return;

        visibilityResolver.OnVisibilityChanged -= HandleVisibilityChanged;
    }

    private void HandleVisibilityChanged(bool visible)
    {
        if (!hasActiveDialogue) return;

        SetMode(visible
        ? PresentationMode.World
        : PresentationMode.Subtitle);
    }

    public void BeginDialogue(
        string message,
        string speaker,
        Transform anchor)
    {
        if (anchor == null)
        {
            Debug.LogWarning($"Missing dialogue anchor: {speaker}");
            EndDialogue();
            return;
        }

        CancelFade();

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

        SetAnchor(null);
        visibilityResolver.ClearTarget();
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
        CancelFade();

        if (!useFade || fadeDuration <= 0f ||
            mode == PresentationMode.Hidden)
        {
            SetModeImmediate(mode);
            return;
        }
        if (IsModeSettled(mode)) return;
        
        fadeRoutine = StartCoroutine(TransitionTo(mode));
    }
    public void SetAnchor(Transform anchor)
    {
        currentAnchor = anchor;
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


    [Header("Canvas Fade Settings")]
    [SerializeField] private CanvasGroup worldGroup;
    [SerializeField] private CanvasGroup subtitleGroup;
    [SerializeField] private bool useFade = true;
    [SerializeField, Min(0f)]
    private float fadeDuration = 0.15f;
    private Coroutine fadeRoutine;
    private IEnumerator TransitionTo(PresentationMode mode)
    {
        bool showWorld = 
            mode == PresentationMode.World
            || (alwaysOpenWorldDialog && mode == PresentationMode.Subtitle);
        bool showSubtitle = mode == PresentationMode.Subtitle;

        float worldTarget = showWorld ? 1f : 0f;
        float subtitleTarget = showSubtitle ? 1f : 0f;

        if (showWorld) worldRoot.SetActive(true);
        if (showSubtitle) subtitleRoot.SetActive(true);

        while (!Mathf.Approximately(worldGroup.alpha, worldTarget) 
            || !Mathf.Approximately(subtitleGroup.alpha, subtitleTarget))
        {
            float step = Time.unscaledDeltaTime / fadeDuration;

            worldGroup.alpha = Mathf.MoveTowards(worldGroup.alpha, worldTarget, step);
            subtitleGroup.alpha = Mathf.MoveTowards(subtitleGroup.alpha, subtitleTarget, step);

            yield return null;
        }

        worldRoot.SetActive(showWorld);
        subtitleRoot.SetActive(showSubtitle);
        fadeRoutine = null;
    }
    private void CancelFade()
    {
        if (fadeRoutine == null) return;

        StopCoroutine(fadeRoutine);
        fadeRoutine = null;
    }
    private bool IsModeSettled(PresentationMode mode)
    {
        bool expectWorld = 
            mode == PresentationMode.World
            || (alwaysOpenWorldDialog && mode == PresentationMode.Subtitle);
        bool expectSubtitle = mode == PresentationMode.Subtitle;

        return worldRoot.activeSelf == expectWorld
            && subtitleRoot.activeSelf == expectSubtitle
            && Mathf.Approximately(
                worldGroup.alpha, expectWorld ? 1f : 0f)
            && Mathf.Approximately(
                subtitleGroup.alpha, expectSubtitle ? 1f : 0f);
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
    private void SetModeImmediate(PresentationMode mode)
    {
        bool showWorld =
            mode == PresentationMode.World
            || (alwaysOpenWorldDialog && mode == PresentationMode.Subtitle);
        bool showSubtitle = mode == PresentationMode.Subtitle;

        worldRoot.SetActive(false);
        subtitleRoot.SetActive(false);

        worldGroup.alpha = showWorld ? 1f : 0f;
        subtitleGroup.alpha = showSubtitle ? 1f : 0f;
        
        worldRoot.SetActive(showWorld);
        subtitleRoot.SetActive(showSubtitle);
    }

    [ContextMenu("Test/Begin Dialogue")]
    private void TestBegin()
    {
        BeginDialogue("Hello", "Speaker", currentAnchor);
    }
    [ContextMenu("Test/End Dialogue")]
    private void TestEnd()
    {
        EndDialogue();
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
