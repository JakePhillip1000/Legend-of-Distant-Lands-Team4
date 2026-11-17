
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class TextAnimation : MonoBehaviour
{
    [SerializeField] GameObject interactCanvas;
    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private TMP_Text interactionText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private float animationSpeed = 0.05f;

    private InputAction aButton;
    private bool playerInside = false;
    private bool dialogueActive = false;
    private bool isTyping = false;
    private int currentLine = 0;
    private Coroutine typingCorountine;

    private readonly string[] dialogues = {
        "yasyaaysaysyasyasaysa asdyuasyduasyduasydusaydusaydausds",
        "asduasydasdasdasdasdas iuasidua idauiasduius",
        "This is the third dialogue.",
        "This is the fourth dialogue.",
        "This is the final dialogue."
    };

    private void Awake()
    {
        aButton = new InputAction(
            name: "A Button",
            type: InputActionType.Button,
            binding: "<XRController>{RightHand}/primaryButton"
        );

        if (interactCanvas != null)
        {
            interactCanvas.SetActive(false);
        }

        if (dialogueCanvas != null)
        {
            dialogueCanvas.SetActive(false);
        }

        if (interactionText != null)
        {
            interactionText.text = "Press A to interact with NPC";
        }

        if (dialogueText != null)
        {
            dialogueText.text = "";
        }
    }

    private void OnEnable()
    {
        aButton?.Enable();
    }

    private void OnDisable()
    {
        aButton?.Disable();

        if (typingCorountine != null)
        {
            StopCoroutine(typingCorountine);
            typingCorountine = null;
        }
    }

    private void OnDestroy()
    {
        aButton?.Dispose();
    }

    void Update()
    {
        if (!playerInside)
        {
            return;
        }

        if (aButton == null)
        {
            return;
        }

        if (aButton.WasPressedThisFrame())
        {
            if (!dialogueActive)
            {
                StartDialogue();
            }
            else if (isTyping)
            {
                CompleteText();
            }
            else
            {
                NextDialogue();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInside = true;

        if (!dialogueActive && interactCanvas != null)
        {
            interactCanvas.SetActive(true);
            Debug.Log("YEYYEYEYE. Player is inside the collider");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInside = false;

        if (interactCanvas != null)
        {
            interactCanvas.SetActive(false);
        }

        EndDialogue(false);
    }

    private void StartDialogue()
    {
        if (dialogueCanvas == null || dialogueText == null || dialogues.Length == 0)
        {
            return;
        }

        dialogueActive = true;
        currentLine = 0;

        if (interactCanvas != null)
        {
            interactCanvas.SetActive(false);
        }

        dialogueCanvas.SetActive(true);
        StartTyping();
    }

    private void StartTyping()
    {
        if (typingCorountine != null)
        {
            StopCoroutine(typingCorountine);
            typingCorountine = null;
        }

        if (currentLine >= dialogues.Length)
        {
            EndDialogue(true);
            return;
        }

        typingCorountine = StartCoroutine(AnimateText(dialogues[currentLine]));
    }

    private IEnumerator AnimateText(string texts)
    {
        isTyping = true;
        dialogueText.text = texts;
        dialogueText.maxVisibleCharacters = 0;
        dialogueText.ForceMeshUpdate();

        int characterCount = dialogueText.textInfo.characterCount;

        for (int i = 1; i <= characterCount; i++)
        {
            dialogueText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(Mathf.Max(0.001f, animationSpeed));
        }

        isTyping = false;
        typingCorountine = null;
    }

    private void CompleteText()
    {
        if (typingCorountine != null)
        {
            StopCoroutine(typingCorountine);
            typingCorountine = null;
        }

        dialogueText.maxVisibleCharacters = int.MaxValue;
        isTyping = false;
    }

    private void NextDialogue()
    {
        currentLine++;

        if (currentLine < dialogues.Length)
        {
            StartTyping();
        }
        else
        {
            EndDialogue(true);
        }
    }

    private void EndDialogue(bool showInteraction)
    {
        if (typingCorountine != null)
        {
            StopCoroutine(typingCorountine);
            typingCorountine = null;
        }

        dialogueActive = false;
        isTyping = false;
        currentLine = 0;

        if (dialogueText != null)
        {
            dialogueText.text = "";
            dialogueText.maxVisibleCharacters = int.MaxValue;
        }

        if (dialogueCanvas != null)
        {
            dialogueCanvas.SetActive(false);
        }

        if (interactCanvas != null)
        {
            interactCanvas.SetActive(showInteraction && playerInside);
        }
    }
}
