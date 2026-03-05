using BoardGameRules.Utils.Logging;
using UnityEngine;

namespace Frameworks.View.Common;

public class SafeArea : MonoBehaviour
{
	private RectTransform Panel;

	private Rect LastSafeArea = new Rect(0f, 0f, 0f, 0f);

	private readonly ILog Logger = LogFactory.BuildLogger(typeof(SafeArea));

	private void Awake()
	{
		Panel = GetComponent<RectTransform>();
		Refresh();
	}

	private void Update()
	{
		Refresh();
	}

	private void Refresh()
	{
		Rect safeArea = GetSafeArea();
		if (safeArea != LastSafeArea)
		{
			ApplySafeArea(safeArea);
		}
	}

	private Rect GetSafeArea()
	{
		return Screen.safeArea;
	}

	private void ApplySafeArea(Rect r)
	{
		LastSafeArea = r;
		Vector2 position = r.position;
		Vector2 anchorMax = r.position + r.size;
		position.x /= Screen.width;
		position.y /= Screen.height;
		anchorMax.x /= Screen.width;
		anchorMax.y /= Screen.height;
		Panel.anchorMin = position;
		Panel.anchorMax = anchorMax;
		Logger.Verbose("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}", base.name, r.x, r.y, r.width, r.height, Screen.width, Screen.height);
	}
}
