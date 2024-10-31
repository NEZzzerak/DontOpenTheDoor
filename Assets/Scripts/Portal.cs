using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{

	public string sceneToLoad; // Имя сцены, в которую нужно телепортироваться
	public int TimeToTeleport;
	private bool isPlayerInTrigger = false; // Флаг, чтобы отслеживать, находится ли игрок в триггере

	void Update()
	{
		// Проверяем, нажата ли клавиша E и находится ли игрок в триггере
		if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
		{
			StartCoroutine(Teleport());
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		// Проверяем, является ли объект игроком
		if (other.CompareTag("Player"))
		{
			isPlayerInTrigger = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		// Проверяем, покинул ли игрок триггер
		if (other.CompareTag("Player"))
		{  
			isPlayerInTrigger = false;
		}
	}

	private IEnumerator Teleport()
	{
		yield return new WaitForSeconds(TimeToTeleport);
		SceneManager.LoadScene(sceneToLoad);
	}


}
