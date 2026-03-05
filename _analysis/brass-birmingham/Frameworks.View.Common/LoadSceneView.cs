using System;
using System.Collections;
using BoardGameRules.Utils.Logging;
using InterfaceAdapters.Common;
using InterfaceAdapters.Common.LoadScene;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Frameworks.View.Common;

public class LoadSceneView : BaseView, ILoadSceneView, IBaseView
{
	private LoadSceneController _controller;

	private new static readonly ILog Logger = LogFactory.BuildLogger(typeof(LoadSceneView));

	private TextMeshProUGUI TipText { get; set; }

	protected override bool ShowImmediately => true;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is LoadSceneController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public void LoadScene(string sceneName)
	{
		StartCoroutine(LoadSceneCoroutine(sceneName));
	}

	public void DisplayTip(string tipText, int tipLocalizationId, bool wasLastTipNew)
	{
		TipText.text = tipText;
		Global.Instance.LastTipId = tipLocalizationId;
		Global.Instance.WasLastTipNew = wasLastTipNew;
	}

	private IEnumerator LoadSceneCoroutine(string sceneName)
	{
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
		while (!asyncOperation.isDone)
		{
			Logger.Debug("Load progress = " + asyncOperation.progress);
			yield return new WaitForEndOfFrame();
		}
	}

	public bool GetWasLastTipNewGlobal()
	{
		return Global.Instance.WasLastTipNew;
	}

	public int GetLastTipIdGlobal()
	{
		return Global.Instance.LastTipId;
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		TipText = base.transform.Find("Popup/TipText").GetComponent<TextMeshProUGUI>();
	}
}
