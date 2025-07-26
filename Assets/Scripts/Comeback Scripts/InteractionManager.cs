using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InteractionManager : MonoBehaviour
{
    public Transform player;
    public Camera playerCamera;

    [Header("UI затемнение")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 0.5f;

    public float interactionDistance = 3f;

    private IInteractable currentInteractable;

    private Teleport doorToShop;
    private Teleport shopToDoor;

    void Start()
    {
        // Задайте позиции и повороты для телепортаций вручную или из объектов на сцене
        Vector3 doorPosition = new Vector3(0, 0, 0);
        Quaternion doorRotation = Quaternion.identity;

        Vector3 shopPosition = new Vector3(10, 0, 0);
        Quaternion shopRotation = Quaternion.Euler(0, 180, 0);

        doorToShop = new Teleport(player, shopPosition, shopRotation, () => StartCoroutine(FadeIn()));
        shopToDoor = new Teleport(player, doorPosition, doorRotation, () => StartCoroutine(FadeIn()));
    }

    void Update()
    {
        UpdateCurrentInteractable();

        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(FadeOutAndTeleport(currentInteractable));
        }
    }

    private void UpdateCurrentInteractable()
    {
        currentInteractable = null;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, interactionDistance))
        {
            var interactable = hitInfo.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                currentInteractable = interactable;
            }
        }
    }

    private IEnumerator FadeOutAndTeleport(IInteractable interactable)
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            yield return null;
        }

        interactable.Interact();

        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            yield return null;
        }
    }

    private IEnumerator FadeIn()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            yield return null;
        }
    }
}