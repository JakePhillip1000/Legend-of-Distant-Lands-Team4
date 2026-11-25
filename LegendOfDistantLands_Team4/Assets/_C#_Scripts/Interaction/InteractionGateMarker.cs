using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class InteractionGateMarker : Marker,
    INotification,
    INotificationOptionProvider
{
    public string interactionId;

    public PropertyName id 
        => new PropertyName(nameof(InteractionGateMarker));

    public NotificationFlags flags 
        => NotificationFlags.TriggerOnce;
}