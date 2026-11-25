
using UnityEngine;
using UnityEngine.Playables;

public class InteractionGateReceiver :
    MonoBehaviour, INotificationReceiver
{
    [SerializeField]
    private NarrativeController narrativeController;

    public void OnNotify(
        Playable origin,
        INotification notification,
        object context)
    {
        if (!Application.isPlaying) return;

        if (narrativeController == null) return;

        InteractionGateMarker gate =
            notification as InteractionGateMarker;

        if (gate == null) return;

        narrativeController.WaitForInteraction(gate.interactionId);
    }
}
