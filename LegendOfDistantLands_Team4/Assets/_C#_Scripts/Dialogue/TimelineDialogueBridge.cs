using UnityEngine;

public class TimelineDialogBridge : MonoBehaviour
{
    [SerializeField] private DialoguePresenter presenter;
    [SerializeField] private Transform dialogueAnchor;
    [SerializeField] private string speaker = "Yunanan";
    [SerializeField, TextArea] private string message = "Dakara sa, ano ko wa kawaii. Atashi wa koibito. Anata ni wa atashi ga mienai no. Kodou no mama ni dokidoki suru no. Chiisana haato no onna no ko.";

    public void Begin()
    {
        presenter.BeginDialogue(message, speaker, dialogueAnchor);
    }
    public void End()
    {
        presenter.EndDialogue();
    }
}