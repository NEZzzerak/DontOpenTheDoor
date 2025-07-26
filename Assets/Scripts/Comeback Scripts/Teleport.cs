using UnityEngine;

public class Teleport : IInteractable, ITeleportable
{
    private Transform player;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private System.Action onTeleportCallback; // вызвать после телепорта

    public Teleport(Transform player, Vector3 targetPos, Quaternion targetRot, System.Action callback = null)
    {
        this.player = player;
        targetPosition = targetPos;
        targetRotation = targetRot;
        onTeleportCallback = callback;
    }

    public void OnFocus()
    {
        throw new System.NotImplementedException();
    }

    public void OnDeFocus()
    {
        throw new System.NotImplementedException();
    }
    
    public void Interact()
    {
        Teleportation();
    }

    public void Teleportation()
    {
        player.position = targetPosition;
        player.rotation = targetRotation;
        onTeleportCallback?.Invoke();
        Debug.Log("Телепортация выполнена.");
    }
}
