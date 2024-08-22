using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testInteractable : Interact
{
    public override void OnFocus()
    {
        print("Ты смотришь на объект" + gameObject.name);
    }

    public override void OnInteract()
    {
        print("Ты взаимодействуешь с объектом" + gameObject.name);
    }

    public override void OnLose()
    {
      print("Ты не смотришь на объект" + gameObject.name);
    }
}
