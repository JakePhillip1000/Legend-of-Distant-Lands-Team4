
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class PickUpObject : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform playercam;
    [SerializeField] private Transform rightHand;
    [SerializeField]
    private InputAction pickupAction = new InputAction(
        type: InputActionType.Button,
        binding: "<XRController>{LeftHand}/primaryButton"
    );

    [SerializeField] private GameObject pickupObject;
    [SerializeField] private GameObject itemInHand;
    [SerializeField] private TMP_Text pickupText;

    private bool playerInside = false;
    private bool pickedUp = false;
    private Rigidbody rb;
    private Collider pickupCollider;

    private void OnEnable() {
        pickupAction.Enable();
    }

    private void OnDisable() {
        pickupAction.Disable();
    }

    private void Start()
    {
        rb = pickupObject.GetComponent<Rigidbody>();
        pickupCollider = pickupObject.GetComponent<Collider>();

        pickupText.gameObject.SetActive(false);
        itemInHand.SetActive(false);
    }

    private void Update()
    {
        if (pickupAction.WasPressedThisFrame())
        {
            if (pickedUp)
            {
                Drop();
            }
            else {
                PickUp();
            }
        }
    }

    private void LateUpdate()
    {
        if (pickupText.gameObject.activeSelf)
        {
            pickupText.transform.rotation = playercam.rotation;
        }
    }

    private bool IsPlayer(Collider other)
    {
        if (player == null)
        {
            return false;
        }

        return other.transform == player ||
               other.transform.IsChildOf(player);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{other.name} enter the collider");

        if (IsPlayer(other) && !pickedUp)
        {
            playerInside = true;
            pickupText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (IsPlayer(other) && !pickedUp)
        {
            playerInside = true;
            pickupText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsPlayer(other) && !pickedUp)
        {
            playerInside = false;
            pickupText.gameObject.SetActive(false);
        }
    }

    private void PickUp(){
        if (!playerInside || pickedUp)
        {
            return;
        }

        pickedUp = true;
        playerInside = false;

        pickupText.gameObject.SetActive(false);
        pickupObject.SetActive(false);
        itemInHand.SetActive(true);

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    private void Drop(){
        if (!pickedUp)
        {
            return;
        }

        pickedUp = false;

        itemInHand.SetActive(false);

        pickupObject.transform.position = rightHand.position + rightHand.forward * 0.3f;
        pickupObject.transform.rotation = rightHand.rotation;
        pickupObject.SetActive(true);

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        playerInside = false;
        pickupText.gameObject.SetActive(false);
    }
}
