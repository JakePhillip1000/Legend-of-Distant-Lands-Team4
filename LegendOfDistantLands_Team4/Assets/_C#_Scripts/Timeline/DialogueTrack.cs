using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.45f, 0.55f, 0.85f)]
[TrackClipType(typeof(DialogClip))]
[TrackBindingType(typeof(DialoguePresenter))]
public class DialougeTrack : TrackAsset
{
    public override Playable CreateTrackMixer(
        PlayableGraph graph, 
        GameObject go, 
        int inputCount)
    {
        return ScriptPlayable<DialogueMixer>.Create(
            graph,
            inputCount
        );
    }
}