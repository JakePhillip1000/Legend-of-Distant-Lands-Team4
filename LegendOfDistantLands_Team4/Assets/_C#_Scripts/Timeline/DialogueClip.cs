using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class DialogClip : PlayableAsset, ITimelineClipAsset
{
    [TextArea(2, 5)] public string message;
    public string speaker;
    public ExposedReference<Transform> anchor;
    public ClipCaps clipCaps => ClipCaps.None;

    // DIY Playable
    public override Playable CreatePlayable(
        PlayableGraph graph, 
        GameObject owner)
    {
        var playable = ScriptPlayable<DialogueBehaviour>.Create(graph);

        DialogueBehaviour behaviour = playable.GetBehaviour();

        behaviour.message = message; 
        behaviour.speaker = speaker;
        behaviour.anchor = anchor.Resolve(graph.GetResolver()); 

        return playable;
    }
}