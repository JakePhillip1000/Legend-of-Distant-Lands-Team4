using UnityEngine;
using UnityEngine.Playables;
using System;

public class TimelineDriver : MonoBehaviour
{
    public event Action OnStopped;
    private void OnEnable()
    {
        if (director == null) return;

        director.stopped += HandleDirectorStopped;
    }

    private void OnDisable()
    {
        if (director == null) return;

        director.stopped -= HandleDirectorStopped;
    }

    private void HandleDirectorStopped(PlayableDirector source)
    {
        OnStopped?.Invoke();
    }

    [SerializeField] private PlayableDirector director;

    [ContextMenu("Test/Play")]
    public void Play()
    {
        director.Play();
    }

    [ContextMenu("Test/Pause")]
    public void Pause()
    {
        director.Pause();
    }

    [ContextMenu("Test/Resume")]
    public void Resume()
    {
        director.Resume();
    }

    [ContextMenu("Test/Stop")]
    public void Stop()
    {
        director.Stop();
    }
}