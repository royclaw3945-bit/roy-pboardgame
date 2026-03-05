using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class IncomeTrackBuilder : MonoBehaviour
{
	private readonly int[] incomeTrack = new int[100]
	{
		-10, -9, -8, -7, -6, -5, -4, -3, -2, -1,
		0, 1, 1, 2, 2, 3, 3, 4, 4, 5,
		5, 6, 6, 7, 7, 8, 8, 9, 9, 10,
		10, 11, 11, 11, 12, 12, 12, 13, 13, 13,
		14, 14, 14, 15, 15, 15, 16, 16, 16, 17,
		17, 17, 18, 18, 18, 19, 19, 19, 20, 20,
		20, 21, 21, 21, 21, 22, 22, 22, 22, 23,
		23, 23, 23, 24, 24, 24, 24, 25, 25, 25,
		25, 26, 26, 26, 26, 27, 27, 27, 27, 28,
		28, 28, 28, 29, 29, 29, 29, 30, 30, 30
	};

	public GameObject trackFieldPrefab;

	public GameObject trackRowPrefab;

	public Transform track;

	private void Awake()
	{
		BuildTrack();
	}

	private void BuildTrack()
	{
		int num = 0;
		GameObject gameObject = null;
		for (int i = 0; i < incomeTrack.Length; i++)
		{
			int num2 = incomeTrack[i];
			if (i == 0 || num2 != incomeTrack[i - 1])
			{
				num = 0;
				gameObject = Object.Instantiate(trackRowPrefab, Vector3.zero, Quaternion.identity);
				gameObject.transform.parent = track;
				gameObject.transform.SetSiblingIndex(0);
				string text = string.Empty;
				if (num2 != 0)
				{
					text += ((num2 > 0) ? "+" : "-");
				}
				text += "£";
				text += Mathf.Abs(num2);
				gameObject.GetComponentInChildren<TextMeshProUGUI>().text = text;
			}
			GameObject obj = Object.Instantiate(trackFieldPrefab, Vector3.zero, Quaternion.identity);
			obj.transform.Find("Frame/Number").GetComponent<TextMeshProUGUI>().text = string.Concat(i);
			obj.transform.Find("Arrow").GetComponent<Image>().gameObject.SetActive(num != 0);
			obj.transform.parent = gameObject.transform;
			num++;
		}
	}
}
