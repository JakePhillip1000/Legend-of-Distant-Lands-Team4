using UnityEngine;
using UnityEngine.Playables;

public class DialogueMixer : PlayableBehaviour
{
    private int activeClipIndex = -1;
    private DialoguePresenter cachedPresenter;

    public override void ProcessFrame(
        Playable playable, 
        FrameData info, 
        object playerData)
    {
        if (!Application.isPlaying) return;

        DialoguePresenter presenter = playerData as DialoguePresenter;

        if (presenter == null) return;

        cachedPresenter = presenter;
        int nextClipIndex = -1;

        for (int i = 0; i < playable.GetInputCount(); i++)
        {
            float weight = playable.GetInputWeight(i);

            if (weight > 0f)
            {
                nextClipIndex = i;
                break;
            }
        }

        if (nextClipIndex == activeClipIndex) return;

        activeClipIndex = nextClipIndex;

        if (nextClipIndex == -1)
        {
            presenter.EndDialogue();
            return;
        }

        var inputPlayable = 
            (ScriptPlayable<DialogueBehaviour>) 
            playable.GetInput(nextClipIndex);

        DialogueBehaviour behaviour = inputPlayable.GetBehaviour();

        presenter.BeginDialogue(
            behaviour.message, 
            behaviour.speaker, 
            behaviour.anchor);
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        if (Application.isPlaying
            && cachedPresenter != null
            && activeClipIndex != -1)
        {
            cachedPresenter.EndDialogue();
        }

        activeClipIndex = -1;
        cachedPresenter = null;
    }
}
