using UnityEngine;

public class TeleportInteractable : MonoBehaviour, IInteractable
{
    public Transform player;
    public Transform teleportTarget;  // Точка, куда телепортироваться
    private Teleport teleportLocation;

    private void Awake()
    {
        teleportLocation = new Teleport(player, teleportTarget.position, teleportTarget.rotation, null);
    }

    public void Interact()
    {
        teleportLocation.Interact();
    }
}

