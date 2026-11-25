using UnityEngine;
using System;

public class NarrativeController : MonoBehaviour
{
    [SerializeField]
    private TimelineDriver timelineDriver;

    public enum NarrativeState
    {
        Idle,
        Playing,
        WaitingForInteraction
    }

    public event Action<string> OnInteractionChanged;
    public string RequiredInteractionId => requiredInteractionId;

    private void OnEnable()
    {
        if (timelineDriver == null) return;

        timelineDriver.OnStopped += HandleTimelineStopped;
    }

    private void OnDisable()
    {
        if (timelineDriver == null) return;

        timelineDriver.OnStopped -= HandleTimelineStopped;
    }

    private void HandleTimelineStopped()
    {
        ResetNarrativeState();
    }

    [Header("Runtime Debug")]
    [SerializeField]
    private NarrativeState state = NarrativeState.Idle;

    [SerializeField]
    private string requiredInteractionId;

    public void StartNarrative()
    {
        if (state != NarrativeState.Idle) return;

        state = NarrativeState.Playing;
        timelineDriver.Play();
    }

    public void WaitForInteraction(string interactionId)
    {
        if (state != NarrativeState.Playing
            || string.IsNullOrWhiteSpace(interactionId)) return;

        requiredInteractionId = interactionId;
        state = NarrativeState.WaitingForInteraction;

        timelineDriver.Pause();

        OnInteractionChanged?.Invoke(requiredInteractionId);
    }

    public void CompleteInteraction(string interactionId)
    {
        if (state != NarrativeState.WaitingForInteraction 
            || requiredInteractionId != interactionId) return;

        requiredInteractionId = null;
        state = NarrativeState.Playing;

        OnInteractionChanged?.Invoke(null);

        timelineDriver.Resume();
    }

    public void StopNarrative()
    {
        requiredInteractionId = null;
        timelineDriver.Stop();
    }

    private void ResetNarrativeState()
    {
        bool wasWaiting = requiredInteractionId != null;

        requiredInteractionId = null;
        state = NarrativeState.Idle;

        if (wasWaiting == true)
        {
            OnInteractionChanged?.Invoke(null);
        }
    }
}
