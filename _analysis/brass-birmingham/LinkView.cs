using System;
using System.Threading.Tasks;
using BoardGameRules.Entities.Players;
using BrassApplication.UseCases.Game.Actions;
using DG.Tweening;
using InterfaceAdapters.Game.Board;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class LinkView : UIBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	private GameObject _tokenObject;

	private GameObject _outlineObject;

	private Button _tokenButton;

	private Image _tokenImage;

	private GameObject _canalImage;

	private GameObject _railImage;

	private TextMeshProUGUI _victoryPoint;

	private string linkId;

	private PlayerColor tokenColor;

	private Action<string, float, float> _onHover = delegate
	{
	};

	private Action _onHoverExit = delegate
	{
	};

	private bool isTooltipVisible;

	private AudioSource _audioSource;

	public AudioClip PlaceLinkClip;

	public float minWidth
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public float preferredWidth
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public float flexibleWidth
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public float minHeight
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public float preferredHeight
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public float flexibleHeight
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public int layoutPriority
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	private new void Awake()
	{
		PrepareReferences();
		_victoryPoint.color = new Color(_victoryPoint.color.r, _victoryPoint.color.g, _victoryPoint.color.b, 0f);
		_victoryPoint.transform.rotation = Quaternion.identity;
	}

	private void LateUpdate()
	{
		if (isTooltipVisible && (Mathf.Abs(Input.GetAxis("Mouse X")) >= 0.15f || Mathf.Abs(Input.GetAxis("Mouse Y")) >= 0.15f))
		{
			_onHoverExit();
			isTooltipVisible = false;
		}
	}

	public void UpdateLink(LinkViewModel linkViewModel, ILinkSelectedDelegate linkSelectedDelegate)
	{
		_tokenButton.onClick.RemoveAllListeners();
		_tokenImage.color = (linkViewModel.IsTokenVisible ? new Color(1f, 1f, 1f, 1f) : new Color(0f, 0f, 0f, 0f));
		_outlineObject.SetActive(linkViewModel.IsOutlineVisible);
		if (linkViewModel.IsOutlineVisible)
		{
			OutlineAnimation();
		}
		tokenColor = linkViewModel.TokenColor;
		if (linkViewModel.Link != null)
		{
			linkId = linkViewModel.Link.LinkId;
		}
		_tokenImage.sprite = Resources.Load<Sprite>(string.Concat("Board/Links/Token", linkViewModel.CurrentEra, linkViewModel.TokenColor));
		_onHover = linkViewModel.OnHover;
		_onHoverExit = linkViewModel.OnHoverExit;
		if ((bool)_canalImage)
		{
			_canalImage.SetActive(linkViewModel.IsRiverVisible);
		}
		if ((bool)_railImage)
		{
			_railImage.SetActive(linkViewModel.IsRailVisible);
		}
		if (linkViewModel.IsInteractive)
		{
			_tokenButton.onClick.AddListener(delegate
			{
				linkSelectedDelegate?.LinkSelected(linkViewModel.Link);
			});
		}
	}

	private void OutlineAnimation()
	{
		Vector3 scale = Vector3.one;
		Sequence sequence = DOTween.Sequence();
		sequence.Append(_outlineObject.transform.DOScale(scale * 2f, 0f));
		sequence.Join(_outlineObject.GetComponent<Image>().DOFade(0f, 0f));
		sequence.Append(_outlineObject.transform.DOScale(scale, 0.5f));
		sequence.Join(_outlineObject.GetComponent<Image>().DOFade(1f, 0.5f));
		sequence.OnKill(delegate
		{
			Sequence sequence2 = DOTween.Sequence();
			sequence2.Append(_outlineObject.transform.DOScale(scale * 1.1f, 0.2f));
			sequence2.Append(_outlineObject.transform.DOScale(scale, 0.2f));
			sequence2.Append(_outlineObject.transform.DOScale(scale, UnityEngine.Random.Range(1.75f, 2.25f)));
			sequence2.SetLoops(-1);
		});
	}

	public void RemoveTokenAnimation(int points)
	{
		Sequence sequence = DOTween.Sequence();
		sequence.AppendCallback(delegate
		{
			_victoryPoint.text = _victoryPoint.text.Replace("1", points.ToString());
			_tokenImage.transform.SetParent(base.transform.parent.parent);
			_tokenImage.transform.SetAsLastSibling();
		}).Join(_tokenImage.transform.DOScale(2f, 1f)).Join(_tokenImage.DOFade(0f, 1f));
		sequence.InsertCallback(0f, delegate
		{
			_victoryPoint.transform.SetParent(base.transform.parent.parent);
			_victoryPoint.transform.SetAsLastSibling();
		}).Join(_victoryPoint.DOFade(1f, 1f)).Join(_victoryPoint.transform.DOMoveY(base.transform.position.y + 100f, 1f));
		sequence.Append(_victoryPoint.transform.DOScale(2f, 1f)).Join(_victoryPoint.DOFade(0f, 1f));
		sequence.AppendCallback(delegate
		{
			_tokenImage.transform.SetParent(base.transform);
			_victoryPoint.transform.SetParent(base.transform);
		});
		sequence.OnComplete(delegate
		{
			_victoryPoint.text = _victoryPoint.text.Replace(points.ToString(), "1");
			_victoryPoint.alpha = 0f;
			_victoryPoint.transform.localPosition = Vector3.zero;
			_victoryPoint.transform.localScale = Vector3.one;
			_tokenImage.transform.localScale = Vector3.one;
		});
		sequence.Play();
	}

	public PlayerColor GetLinkColor()
	{
		return tokenColor;
	}

	public string GetLinkId()
	{
		return linkId;
	}

	public bool isBuilt()
	{
		return _tokenImage.color.a == 1f;
	}

	private void PrepareReferences()
	{
		_tokenObject = base.transform.Find("LinkToken").gameObject;
		_outlineObject = base.transform.Find("LinkToken/Outline").gameObject;
		_tokenButton = base.transform.Find("LinkToken").GetComponent<Button>();
		_tokenImage = _tokenObject.GetComponent<Image>();
		_canalImage = base.transform.Find("CanalImage").gameObject;
		_railImage = base.transform.Find("RailImage").gameObject;
		_victoryPoint = base.transform.Find("VictoryPoint").gameObject.GetComponent<TextMeshProUGUI>();
		_audioSource = base.transform.GetComponent<AudioSource>();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		_onHover(linkId, base.transform.position.x, base.transform.position.y);
		TooltipDisappearDelay();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isTooltipVisible = false;
		_onHoverExit();
	}

	public async void TooltipDisappearDelay()
	{
		await Task.Delay(1000);
		isTooltipVisible = true;
	}

	public void PlayPlaceLinkClip()
	{
		_audioSource.PlayOneShot(PlaceLinkClip);
	}
}
