using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interact : MonoBehaviour
{
  public abstract void OnInteract();
  public abstract void OnFocus();
  public abstract void OnLose();
}
