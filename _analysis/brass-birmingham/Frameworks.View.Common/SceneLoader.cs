using System.Collections;
using InterfaceAdapters.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Frameworks.View.Common;

public class SceneLoader : MonoBehaviour, ISceneLoader
{
	public void LoadSceneAsync(string sceneName)
	{
		StartCoroutine(LoadSceneCoroutine(sceneName));
	}

	private IEnumerator LoadSceneCoroutine(string sceneName)
	{
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
		while (!asyncOperation.isDone)
		{
			Debug.Log("Load progress = " + asyncOperation.progress);
			yield return new WaitForEndOfFrame();
		}
	}
}
