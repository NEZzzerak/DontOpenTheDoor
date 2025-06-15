using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private string[] lines;
    [SerializeField] private float textSpeed;
    [SerializeField] private AudioClip typingSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject dialoguePanel; // Панель диалога
    [SerializeField] private float interactionDistance = 2f; // Дистанция взаимодействия

    private int index;
    private bool isTalking = false;


    private void Start()
    {
        if (textComponent == null || audioSource == null || dialoguePanel == null)
        {
            Debug.LogError("Необходимо настроить компоненты TextMeshProUGUI, AudioSource и dialoguePanel!");
            enabled = false;
            return;
        }

        textComponent.text = string.Empty;
        dialoguePanel.SetActive(false); // Скрываем панель в начале
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactionDistance))
            {
                if (hit.collider.CompareTag("NPC") && !isTalking) // Проверяем, что луч попал в NPC и диалог не активен
                {
                    HandleInteraction();
                }
            }
        }

        if (isTalking && Input.GetMouseButtonDown(0)) // Переключение текста во время диалога
        {
            NextLine();
        }
    }


    private void HandleInteraction()
    {
        StartDialogue();
        isTalking = true;

    }

    private void StartDialogue()
    {
        index = 0;
        dialoguePanel.SetActive(true);
        StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        textComponent.text = string.Empty; // Очистить текст перед выводом новой строки

        if (typingSound != null)
        {
            audioSource.PlayOneShot(typingSound);
        }

        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    private void NextLine()
{
    if (isTalking) 
    {
        if (index < lines.Length - 1)
        {
            index++;
            StopAllCoroutines(); // Останавливаем все корутины, чтобы избежать смешивания текста
            StartCoroutine(TypeLine());
        }
        else
        {
            dialoguePanel.SetActive(false); // Скрываем панель после окончания диалога
            isTalking = false;
            textComponent.text = string.Empty;
        }
    }
}
}



