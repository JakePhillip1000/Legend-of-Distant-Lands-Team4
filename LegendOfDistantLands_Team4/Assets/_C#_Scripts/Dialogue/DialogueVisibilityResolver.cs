using System;
using UnityEngine;

public class DialogueVisibilityResolver : MonoBehaviour
{
    [Header("Camera and Focus Object Setup")]
    [SerializeField] private Camera xrCamera;
    [SerializeField] private Transform targetAnchor;

    [Header("Debug")]
    [SerializeField] private Vector3 viewportPosition;
    [SerializeField] private bool isVisible;

    public enum VisibilityMode
    {
        FieldOfView,
        FocusAngle
    }
    
    [Header("Mode")]
    [SerializeField] 
    private VisibilityMode visibilityMode = VisibilityMode.FieldOfView;
    
    [Header("FOV Settings")]
    [SerializeField, Range(0f, 0.49f)]
    private float exitMarginX = 0.05f;
    [SerializeField, Range(0f, 0.49f)]
    private float exitMarginY = 0.05f;
    [SerializeField, Range(0f, 0.49f)]
    private float returnMarginX = 0.15f;
    [SerializeField, Range(0f, 0.49f)]
    private float returnMarginY = 0.15f;

    [Header("Focus Settings")]
    [SerializeField, Range(0f, 90f)]
    private float exitAngle = 25f;
    [SerializeField, Range(0f, 90f)]
    private float returnAngle = 15f;

    [Header("Transition Settings")]
    [SerializeField, Min(0f)]
    private float exitDelay = 0.25f;
    [SerializeField, Min(0f)]
    private float returnDelay = 0.15f;

    [Header("Runtime Debug")]
    [SerializeField] private bool rawVisible;
    [SerializeField] private float transitionTimer;



    private void LateUpdate()
    {
        if (xrCamera == null || targetAnchor == null) return;

        rawVisible = EvaluateVisibility();
        UpdateVisibilityState(rawVisible);
    }

    public bool InitializeTarget(Transform anchor)
    {
        targetAnchor = anchor;

        transitionTimer = 0;

        if (xrCamera == null || targetAnchor == null)
        {
            rawVisible =false;
            isVisible = false;
            return false;
        }

        isVisible = true;

        isVisible = EvaluateVisibility();
        rawVisible = isVisible;
        
        return isVisible;
    }
    public event Action<bool> OnVisibilityChanged;
    private void UpdateVisibilityState(bool detectedVisible)
    {
        if (detectedVisible == isVisible)
        {
            transitionTimer = 0f;
            return;
        }

        transitionTimer += Time.unscaledDeltaTime;

        float delay = isVisible ? exitDelay : returnDelay;

        if (transitionTimer >= delay)
        {
            isVisible = detectedVisible;
            transitionTimer = 0f;

            OnVisibilityChanged?.Invoke(isVisible);
        }
    }

    private bool EvaluateVisibility()
    {
        switch (visibilityMode)
        {
            case VisibilityMode.FieldOfView:

                viewportPosition = 
                    xrCamera.WorldToViewportPoint(targetAnchor.position);

                if (isVisible)
                    return IsInsideViewport(
                        viewportPosition, exitMarginX, exitMarginY);
                
                return IsInsideViewport(
                    viewportPosition, returnMarginX, returnMarginY);

            case VisibilityMode.FocusAngle:

                if (isVisible) 
                    return IsInsideFocusAngle(
                        targetAnchor.position, exitAngle);
                
                return IsInsideFocusAngle(
                    targetAnchor.position, returnAngle);
            default:
                return false;
        }
    }
    private bool IsInsideViewport(
        Vector3 point, 
        float marginX, 
        float marginY)
    {
        if (point.z < 0) return false;
        if (point.x <= marginX 
            || point.x >= 1 - marginX) return false;
        if (point.y <= marginY 
            || point.y >= 1 - marginY) return false;

        return true;
    }
    private bool IsInsideFocusAngle(Vector3 worldPosition, float angle)
    {
        Vector3 forward = xrCamera.transform.forward;
        Vector3 direction = worldPosition
                            - xrCamera.transform.position;

        if (direction.sqrMagnitude < 0.0000001f) return false;

        float dot = Vector3.Dot(forward, direction.normalized);
        float threshold = Mathf.Cos(angle * Mathf.Deg2Rad);

        return dot >= threshold;
    }

    public void ClearTarget()
    {
        targetAnchor = null;
        transitionTimer = 0;
        rawVisible = false;
        isVisible = false;
    }
}