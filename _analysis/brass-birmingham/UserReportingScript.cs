using System;
using System.Collections;
using System.Reflection;
using System.Text;
using Frameworks.View.Game;
using TMPro;
using Unity.Cloud.UserReporting;
using Unity.Cloud.UserReporting.Client;
using Unity.Cloud.UserReporting.Plugin;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class UserReportingScript : MonoBehaviour
{
	[Tooltip("The category dropdown.")]
	public TMP_Dropdown CategoryDropdown;

	[Tooltip("View from this camera will be added to report.")]
	public Camera ScreenshotCamera;

	[Tooltip("The description input on the user report form.")]
	public TMP_InputField DescriptionInput;

	[Tooltip("The UI shown when there's an error.")]
	public CanvasGroup ErrorPopup;

	private bool isCreatingUserReport;

	[Tooltip("A value indicating whether the hotkey is enabled (Left Alt + Left Shift + B).")]
	public bool IsHotkeyEnabled;

	[Tooltip("A value indicating whether the prefab is in silent mode. Silent mode does not show the user report form.")]
	public bool IsInSilentMode;

	[Tooltip("A value indicating whether the user report client reports metrics about itself.")]
	public bool IsSelfReporting;

	private bool isShowingError;

	private bool isSubmitting;

	private bool stateChanged = true;

	[Tooltip("The display text for the progress text.")]
	public TextMeshProUGUI ProgressText;

	[Tooltip("A value indicating whether the user report client send events to analytics.")]
	public bool SendEventsToAnalytics;

	[Tooltip("The UI shown while submitting.")]
	public CanvasGroup SubmittingPopup;

	[Tooltip("The summary input on the user report form.")]
	public TMP_InputField SummaryInput;

	[Tooltip("The thumbnail viewer on the user report form.")]
	public Image ThumbnailViewer;

	private UnityUserReportingUpdater unityUserReportingUpdater;

	[Tooltip("The user report button used to create a user report.")]
	public Button UserReportButton;

	[Tooltip("The UI for the user report form. Shown after a user report is created.")]
	public CanvasGroup UserReportForm;

	[Tooltip("The User Reporting platform. Different platforms have different features but may require certain Unity versions or target platforms. The Async platform adds async screenshotting and report creation, but requires Unity 2018.3 and above, the package manager version of Unity User Reporting, and a target platform that supports asynchronous GPU readback such as DirectX.")]
	public UserReportingPlatformType UserReportingPlatform;

	[Tooltip("The event raised when a user report is submitting.")]
	public UnityEvent UserReportSubmitting;

	[SerializeField]
	[Tooltip("Exit to menu view will comunicate with the rest of the game logic and views.")]
	private ExitToMenuView exitToMenuView;

	private UserReportingState previousState;

	public UserReport CurrentUserReport { get; private set; }

	public UserReportingState State
	{
		get
		{
			if (CurrentUserReport != null)
			{
				if (IsInSilentMode)
				{
					if (previousState != UserReportingState.Idle)
					{
						stateChanged = true;
						previousState = UserReportingState.Idle;
					}
					return UserReportingState.Idle;
				}
				if (isSubmitting)
				{
					if (previousState != UserReportingState.SubmittingForm)
					{
						stateChanged = true;
						previousState = UserReportingState.SubmittingForm;
					}
					return UserReportingState.SubmittingForm;
				}
				if (previousState != UserReportingState.ShowingForm)
				{
					stateChanged = true;
					previousState = UserReportingState.ShowingForm;
				}
				return UserReportingState.ShowingForm;
			}
			if (isCreatingUserReport)
			{
				if (previousState != UserReportingState.CreatingUserReport)
				{
					stateChanged = true;
					previousState = UserReportingState.CreatingUserReport;
				}
				return UserReportingState.CreatingUserReport;
			}
			if (previousState != UserReportingState.Idle)
			{
				stateChanged = true;
				previousState = UserReportingState.Idle;
			}
			return UserReportingState.Idle;
		}
	}

	public UserReportingScript()
	{
		UserReportSubmitting = new UnityEvent();
		unityUserReportingUpdater = new UnityUserReportingUpdater();
	}

	public void CancelUserReport()
	{
		CurrentUserReport = null;
		ClearForm();
	}

	private IEnumerator ClearError()
	{
		yield return new WaitForSeconds(10f);
		isShowingError = false;
	}

	private void ClearForm()
	{
		SummaryInput.text = null;
		DescriptionInput.text = null;
	}

	public void CreateUserReport()
	{
		if (isCreatingUserReport)
		{
			return;
		}
		isCreatingUserReport = true;
		exitToMenuView.HidePopup();
		UnityUserReporting.CurrentClient.TakeScreenshotFromSource(2048, 2048, ScreenshotCamera, delegate
		{
		});
		UnityUserReporting.CurrentClient.TakeScreenshotFromSource(512, 512, ScreenshotCamera, delegate
		{
		});
		UnityUserReporting.CurrentClient.CreateUserReport(delegate(UserReport br)
		{
			if (string.IsNullOrEmpty(br.ProjectIdentifier))
			{
				Debug.LogWarning("The user report's project identifier is not set. Please setup cloud services using the Services tab or manually specify a project identifier when calling UnityUserReporting.Configure().");
			}
			var (s, s2) = exitToMenuView.GetSave();
			br.Attachments.Add(new UserReportAttachment("metadata.storage", "metadata.storage", "application/json", Encoding.UTF8.GetBytes(s)));
			br.Attachments.Add(new UserReportAttachment("events.storage", "events.storage", "application/json", Encoding.UTF8.GetBytes(s2)));
			string arg = "Unknown";
			string arg2 = "0.0";
			foreach (UserReportNamedValue deviceMetadatum in br.DeviceMetadata)
			{
				if (deviceMetadatum.Name == "Platform")
				{
					arg = deviceMetadatum.Value;
				}
				if (deviceMetadatum.Name == "Version")
				{
					arg2 = deviceMetadatum.Value;
				}
			}
			br.Dimensions.Add(new UserReportNamedValue("Platform.Version", $"{arg}.{arg2}"));
			CurrentUserReport = br;
			isCreatingUserReport = false;
			SetThumbnail(br);
			if (IsInSilentMode)
			{
				SubmitUserReport();
			}
		});
	}

	private UserReportingClientConfiguration GetConfiguration()
	{
		return new UserReportingClientConfiguration();
	}

	public bool IsSubmitting()
	{
		return isSubmitting;
	}

	private void SetThumbnail(UserReport userReport)
	{
		if (userReport != null && ThumbnailViewer != null)
		{
			byte[] data = Convert.FromBase64String(userReport.Thumbnail.DataBase64);
			Texture2D texture2D = new Texture2D(1, 1);
			texture2D.LoadImage(data);
			ThumbnailViewer.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
			ThumbnailViewer.preserveAspect = true;
		}
	}

	private void Start()
	{
		if (Application.isPlaying && UnityEngine.Object.FindObjectOfType<EventSystem>() == null)
		{
			GameObject obj = new GameObject("EventSystem");
			obj.AddComponent<EventSystem>();
			obj.AddComponent<StandaloneInputModule>();
		}
		bool flag = false;
		if (UserReportingPlatform == UserReportingPlatformType.Async)
		{
			Type type = Assembly.GetExecutingAssembly().GetType("Unity.Cloud.UserReporting.Plugin.Version2018_3.AsyncUnityUserReportingPlatform");
			if (type != null && Activator.CreateInstance(type) is IUserReportingPlatform platform)
			{
				UnityUserReporting.Configure(platform, GetConfiguration());
				flag = true;
			}
		}
		if (!flag)
		{
			UnityUserReporting.Configure(GetConfiguration());
		}
		string endpoint = $"https://userreporting.cloud.unity3d.com/api/userreporting/projects/{UnityUserReporting.CurrentClient.ProjectIdentifier}/ping";
		UnityUserReporting.CurrentClient.Platform.Post(endpoint, "application/json", Encoding.UTF8.GetBytes("\"Ping\""), delegate
		{
		}, delegate
		{
		});
		CategoryDropdown.options.Clear();
		CategoryDropdown.options.Add(new TMP_Dropdown.OptionData(exitToMenuView.GetTranslation("UI/BUG")));
		CategoryDropdown.options.Add(new TMP_Dropdown.OptionData(exitToMenuView.GetTranslation("UI/PERFORMANCE_ISSUE")));
		CategoryDropdown.options.Add(new TMP_Dropdown.OptionData(exitToMenuView.GetTranslation("UI/SUGGESTION")));
	}

	public void SubmitUserReport()
	{
		if (isSubmitting || CurrentUserReport == null)
		{
			return;
		}
		isSubmitting = true;
		if (SummaryInput != null)
		{
			CurrentUserReport.Summary = SummaryInput.text;
		}
		if (CategoryDropdown != null)
		{
			string text = CategoryDropdown.options[CategoryDropdown.value].text;
			CurrentUserReport.Dimensions.Add(new UserReportNamedValue("Category", text));
			CurrentUserReport.Fields.Add(new UserReportNamedValue("Category", text));
		}
		if (DescriptionInput != null)
		{
			UserReportNamedValue item = new UserReportNamedValue
			{
				Name = "Description",
				Value = DescriptionInput.text
			};
			CurrentUserReport.Fields.Add(item);
		}
		ClearForm();
		RaiseUserReportSubmitting();
		UnityUserReporting.CurrentClient.SendUserReport(CurrentUserReport, delegate(float uploadProgress, float downloadProgress)
		{
			if (ProgressText != null)
			{
				string text2 = $"{uploadProgress:P}";
				ProgressText.text = text2;
			}
		}, delegate(bool success, UserReport br2)
		{
			if (!success)
			{
				isShowingError = true;
				StartCoroutine(ClearError());
			}
			CurrentUserReport = null;
			isSubmitting = false;
		});
	}

	private void Update()
	{
		if (IsHotkeyEnabled && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.B))
		{
			CreateUserReport();
		}
		UnityUserReporting.CurrentClient.IsSelfReporting = IsSelfReporting;
		UnityUserReporting.CurrentClient.SendEventsToAnalytics = SendEventsToAnalytics;
		UserReportingState state = State;
		if (stateChanged)
		{
			stateChanged = false;
			if (UserReportButton != null)
			{
				UserReportButton.interactable = state == UserReportingState.Idle;
			}
			if (UserReportForm != null)
			{
				if (state == UserReportingState.ShowingForm)
				{
					exitToMenuView.ShowPopup();
					UserReportForm.alpha = 1f;
					UserReportForm.interactable = true;
					UserReportForm.blocksRaycasts = true;
				}
				else
				{
					UserReportForm.alpha = 0f;
					UserReportForm.interactable = false;
					UserReportForm.blocksRaycasts = false;
				}
			}
			if (SubmittingPopup != null)
			{
				if (state == UserReportingState.SubmittingForm)
				{
					SubmittingPopup.alpha = 1f;
					SubmittingPopup.interactable = true;
					SubmittingPopup.blocksRaycasts = true;
				}
				else
				{
					SubmittingPopup.alpha = 0f;
					SubmittingPopup.interactable = false;
					SubmittingPopup.blocksRaycasts = false;
				}
			}
		}
		if (ErrorPopup != null)
		{
			if (isShowingError)
			{
				ErrorPopup.alpha = 1f;
				ErrorPopup.interactable = true;
				ErrorPopup.blocksRaycasts = true;
			}
			else
			{
				ErrorPopup.alpha = 0f;
				ErrorPopup.interactable = false;
				ErrorPopup.blocksRaycasts = false;
			}
		}
		unityUserReportingUpdater.Reset();
		StartCoroutine(unityUserReportingUpdater);
	}

	protected virtual void RaiseUserReportSubmitting()
	{
		if (UserReportSubmitting != null)
		{
			UserReportSubmitting.Invoke();
		}
	}
}
