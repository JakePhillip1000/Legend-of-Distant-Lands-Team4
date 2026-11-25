using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PickUpObject : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform playercam;
    [SerializeField] private Transform rightHand;
    [SerializeField] private InputAction pickupAction = new InputAction(
        type: InputActionType.Button,
        binding: "<XRController>{RightHand}/triggerPressed"
    );

    [SerializeField] private GameObject pickupObject;
    [SerializeField] private GameObject itemInHand;
    [SerializeField] private TMP_Text pickupText;

    private bool playerInside = false;
    private bool pickedUp = false;
    private Rigidbody rb;
    private BoxCollider pickupCollider;
    private Vector3 colliderOffset;
    private Renderer objectRenderer;

    private void OnEnable() {
        pickupAction.Enable();
    }

    private void OnDisable() {
        pickupAction.Disable();
        playerInside = false;
        if (pickupText != null) {
            pickupText.gameObject.SetActive(false);
        }
    }

    private void OnDestroy() {
        pickupAction.Dispose();
    }

    private void Start()
    {
        if (player == null || playercam == null || rightHand == null ||
            pickupObject == null || itemInHand == null || pickupText == null ||
            pickupObject == gameObject || pickupObject == itemInHand) {
            enabled = false;
            return;
        }

        rb = pickupObject.GetComponent<Rigidbody>();
        foreach (Collider objectCollider in pickupObject.GetComponentsInChildren<Collider>(true)) {
            foreach (Collider playerCollider in player.GetComponentsInChildren<Collider>(true)) {
                Physics.IgnoreCollision(objectCollider, playerCollider);
            }
        }
        if (rb != null) {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
        pickupCollider = GetComponent<BoxCollider>();
        if (pickupCollider == null) {
            pickupCollider = gameObject.AddComponent<BoxCollider>();
            pickupCollider.size = Vector3.one * 2f;
        }
        pickupCollider.isTrigger = true;
        colliderOffset = pickupCollider.center - transform.InverseTransformPoint(pickupObject.transform.position);
        objectRenderer = pickupObject.GetComponentInChildren<Renderer>();

        Rigidbody zoneBody = GetComponent<Rigidbody>();
        if (zoneBody == null) {
            zoneBody = gameObject.AddComponent<Rigidbody>();
        }
        zoneBody.isKinematic = true;
        zoneBody.useGravity = false;

        itemInHand.SetActive(false);
        itemInHand.transform.SetParent(rightHand, false);
        itemInHand.transform.localPosition = Vector3.zero;
        itemInHand.transform.localRotation = Quaternion.identity;
        foreach (Collider handCollider in itemInHand.GetComponentsInChildren<Collider>(true)) {
            handCollider.enabled = false;
        }
        foreach (Rigidbody handBody in itemInHand.GetComponentsInChildren<Rigidbody>(true)) {
            handBody.isKinematic = true;
            handBody.useGravity = false;
        }
        pickupText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!pickedUp) {
            pickupCollider.center = transform.InverseTransformPoint(pickupObject.transform.position) + colliderOffset;
        }
        CheckPlayerInside();
        pickupText.gameObject.SetActive(playerInside && !pickedUp);

        if (pickupAction.WasPressedThisFrame()) {
            if (pickedUp) {
                Drop();
            }
            else {
                PickUp();
            }
        }
    }

    private void LateUpdate() {
        if (!pickedUp) {
            Vector3 textPosition = pickupObject.transform.position;
            if (objectRenderer != null) {
                textPosition = objectRenderer.bounds.center;
                textPosition.y = objectRenderer.bounds.max.y;
            }
            pickupText.transform.position = textPosition + Vector3.up * 0.5f;
        }
        if (pickupText.gameObject.activeSelf) {
            pickupText.transform.rotation = playercam.rotation;
        }
    }

    private bool IsPlayer(Collider other) {
        return player != null && (other.transform == player || other.transform.IsChildOf(player));
    }

    private void CheckPlayerInside() {
        playerInside = false;
        Vector3 scale = transform.lossyScale;
        scale = new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
        Collider[] nearby = Physics.OverlapBox(
            transform.TransformPoint(pickupCollider.center),
            Vector3.Scale(pickupCollider.size * 0.5f, scale),
            transform.rotation, ~0, QueryTriggerInteraction.Collide);
        foreach (Collider other in nearby) {
            if (IsPlayer(other)) {
                playerInside = true;
                return;
            }
        }
    }

    private void PickUp() {
        if (!playerInside || pickedUp) {
            return;
        }

        pickedUp = true;
        pickupText.gameObject.SetActive(false);
        if (rb != null) {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        pickupObject.SetActive(false);
        itemInHand.SetActive(true);
    }

    private void Drop() {
        if (!pickedUp) {
            return;
        }

        itemInHand.SetActive(false);
        pickupObject.transform.position = rightHand.position + rightHand.forward * 0.3f;
        pickupObject.transform.rotation = rightHand.rotation;
        pickupObject.SetActive(true);
        pickedUp = false;
        pickupCollider.center = transform.InverseTransformPoint(pickupObject.transform.position) + colliderOffset;

        if (rb != null) {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        CheckPlayerInside();
        pickupText.gameObject.SetActive(playerInside);
    }
}
