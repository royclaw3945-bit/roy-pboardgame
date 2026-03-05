using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Board.BuildingSlots;
using DG.Tweening;
using Frameworks.Utils;
using Frameworks.View.Animation;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.Market;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Game;

public class MarketView : BaseView, IMarketView, IBaseView
{
	public GameObject resourceOutlinePrefab;

	private MarketController _controller;

	private Transform _coalMarketGridTransform;

	private Transform _ironMarketGridTransform;

	private Transform ironMarketOutline;

	private Transform coalMarketOutline;

	private Button _buttonCoalMarket;

	private Button _buttonIronMarket;

	private Action _actionOnClickCoalMarket = delegate
	{
	};

	private Action _actionOnClickIronMarket = delegate
	{
	};

	public AudioClip BoughtCoalClip;

	public AudioClip BoughtIronClip;

	public AudioClip SingleSoldClip;

	public AudioClip MultipleSoldClip;

	private AudioSource _audioSource;

	protected override bool ShowImmediately => true;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is MarketController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		_buttonCoalMarket.onClick.AddListener(delegate
		{
			_actionOnClickCoalMarket();
		});
		_buttonIronMarket.onClick.AddListener(delegate
		{
			_actionOnClickIronMarket();
		});
	}

	public void UpdateMarket(MarketViewModel viewModel)
	{
		for (int i = 0; i < _coalMarketGridTransform.childCount; i++)
		{
			_coalMarketGridTransform.GetChild(i).gameObject.SetActive(i < viewModel.AmountOfCoal);
			_coalMarketGridTransform.GetChild(i).GetChild(0).GetComponent<Image>()
				.enabled = i < viewModel.AmountOfCoal;
		}
		for (int j = 0; j < _ironMarketGridTransform.childCount; j++)
		{
			_ironMarketGridTransform.GetChild(j).gameObject.SetActive(j < viewModel.AmountOfIron);
			_ironMarketGridTransform.GetChild(j).GetChild(0).GetComponent<Image>()
				.enabled = j < viewModel.AmountOfIron;
		}
	}

	public void TakeResource()
	{
		Sequence sequence = DOTween.Sequence();
		bool flag = false;
		for (int i = 0; i < _coalMarketGridTransform.childCount; i++)
		{
			Transform highlight = _coalMarketGridTransform.GetChild(i).GetChild(0).GetChild(0);
			if (highlight.gameObject.activeSelf)
			{
				flag = true;
				sequence.AppendCallback(delegate
				{
					_audioSource.PlayOneShot(BoughtCoalClip);
				});
				sequence.AppendCallback(delegate
				{
					highlight.parent.GetComponent<Image>().enabled = false;
				});
				sequence.Append(highlight.GetComponent<Image>().DOFade(1f, 0f));
				sequence.Append(highlight.DOScale(highlight.transform.localScale * 4f, 0.3f));
				sequence.Join(highlight.GetComponent<Image>().DOFade(0f, 0.5f));
				sequence.onKill = (TweenCallback)Delegate.Combine(sequence.onKill, (TweenCallback)delegate
				{
					highlight.localScale = Vector3.one;
					highlight.GetComponent<Image>().DOFade(1f, 0f);
					highlight.gameObject.SetActive(value: false);
				});
			}
		}
		bool flag2 = false;
		for (int num = _ironMarketGridTransform.childCount - 1; num >= 0; num--)
		{
			Transform highlight2 = _ironMarketGridTransform.GetChild(num).GetChild(0).GetChild(0);
			if (highlight2.gameObject.activeSelf)
			{
				flag2 = true;
				sequence.AppendCallback(delegate
				{
					_audioSource.PlayOneShot(BoughtIronClip);
				});
				sequence.AppendCallback(delegate
				{
					highlight2.parent.GetComponent<Image>().enabled = false;
				});
				sequence.Append(highlight2.GetComponent<Image>().DOFade(1f, 0f));
				sequence.Append(highlight2.DOScale(highlight2.transform.localScale * 4f, 0.3f));
				sequence.Join(highlight2.GetComponent<Image>().DOFade(0f, 0.5f));
				sequence.onKill = (TweenCallback)Delegate.Combine(sequence.onKill, (TweenCallback)delegate
				{
					highlight2.localScale = Vector3.one;
					highlight2.GetComponent<Image>().DOFade(1f, 0f);
					highlight2.gameObject.SetActive(value: false);
				});
			}
		}
		if (flag2)
		{
			sequence.Append(ironMarketOutline.GetComponent<Image>().DOFade(0f, 0.5f));
			sequence.AppendCallback(delegate
			{
				StopIronHighlight();
			});
		}
		if (flag)
		{
			sequence.Append(coalMarketOutline.GetComponent<Image>().DOFade(0f, 0.5f));
			sequence.AppendCallback(delegate
			{
				StopCoalHighlight();
			});
		}
	}

	public void StopIronHighlight()
	{
		ironMarketOutline.gameObject.SetActive(value: false);
		_actionOnClickIronMarket = delegate
		{
		};
		for (int num = 0; num < _ironMarketGridTransform.childCount; num++)
		{
			_coalMarketGridTransform.GetChild(num).GetChild(0).GetChild(0)
				.gameObject.SetActive(value: false);
		}
	}

	public void StopCoalHighlight()
	{
		coalMarketOutline.gameObject.SetActive(value: false);
		_actionOnClickCoalMarket = delegate
		{
		};
		for (int num = 0; num < _coalMarketGridTransform.childCount; num++)
		{
			_coalMarketGridTransform.GetChild(num).GetChild(0).GetChild(0)
				.gameObject.SetActive(value: false);
		}
	}

	public void SetUpIronMarketOnClickAction(int resource, int availableResource, Action actionOnClickIronMarket)
	{
		ironMarketOutline.gameObject.SetActive(value: true);
		ironMarketOutline.GetComponent<Image>().DOFade(1f, 0.5f);
		for (int num = resource; num > 0; num--)
		{
			if (availableResource >= num)
			{
				_ironMarketGridTransform.GetChild(availableResource - num).GetChild(0).GetChild(0)
					.gameObject.SetActive(value: true);
				_ironMarketGridTransform.GetChild(availableResource - num).GetChild(0).GetChild(0)
					.GetComponent<Image>()
					.DOFade(0f, 0f);
			}
		}
		_actionOnClickIronMarket = actionOnClickIronMarket;
	}

	public void SetUpCoalMarketOnClickAction(int resource, int availableResource, Action actionOnClickCoalMarket)
	{
		coalMarketOutline.gameObject.SetActive(value: true);
		coalMarketOutline.GetComponent<Image>().DOFade(1f, 0.5f);
		for (int num = resource; num > 0; num--)
		{
			if (availableResource >= num)
			{
				_coalMarketGridTransform.GetChild(availableResource - num).GetChild(0).GetChild(0)
					.gameObject.SetActive(value: true);
				_coalMarketGridTransform.GetChild(availableResource - num).GetChild(0).GetChild(0)
					.GetComponent<Image>()
					.DOFade(0f, 0f);
			}
		}
		_actionOnClickCoalMarket = actionOnClickCoalMarket;
	}

	public IAnimation CreateAddResourcesToMarketAnimation(BuildingType type, int amount)
	{
		Transform market = ((type == BuildingType.CoalMine) ? _coalMarketGridTransform : _ironMarketGridTransform);
		IEnumerable<Transform> slots = GetEmptySlots(market, amount);
		Sequence sequence = DOTween.Sequence();
		sequence.AppendCallback(delegate
		{
			foreach (Transform item in slots)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(resourceOutlinePrefab, item.position, Quaternion.identity, item);
				gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
				item.gameObject.SetActive(value: true);
				item.GetComponent<Image>().enabled = false;
				item.GetChild(0).gameObject.SetActive(value: false);
				item.GetChild(0).GetComponent<Image>().enabled = false;
				Sequence sequence2 = DOTween.Sequence();
				sequence2.Append(gameObject.GetComponent<Image>().DOFade(0.7f, 4f * UIProperties.FrameDuration));
				sequence2.Append(gameObject.GetComponent<Image>().DOFade(1f, 4f * UIProperties.FrameDuration));
				sequence2.SetLoops(5);
				sequence2.Play();
			}
		});
		sequence.AppendInterval(40f * UIProperties.FrameDuration);
		sequence.AppendCallback(delegate
		{
			if (amount == 1)
			{
				_audioSource.PlayOneShot(SingleSoldClip);
			}
			else
			{
				_audioSource.PlayOneShot(MultipleSoldClip);
			}
			market.GetComponent<GridLayoutGroup>().enabled = false;
			Sequence s = DOTween.Sequence();
			int num = 0;
			foreach (Transform item2 in slots)
			{
				item2.GetChild(1).gameObject.SetActive(value: false);
				item2.GetComponent<Image>().enabled = true;
				item2.GetChild(0).gameObject.SetActive(value: true);
				item2.GetChild(0).GetComponent<Image>().enabled = true;
				RectTransform component = item2.GetComponent<RectTransform>();
				Vector2 anchoredPosition = component.anchoredPosition;
				float y = anchoredPosition.y;
				anchoredPosition = new Vector2(anchoredPosition.x, anchoredPosition.y - 50f);
				component.anchoredPosition = anchoredPosition;
				s.Insert((float)(3 * num++) * UIProperties.FrameDuration, component.DOAnchorPosY(y, 6f * UIProperties.FrameDuration).SetEase(Ease.OutBack));
			}
		});
		sequence.AppendInterval(20f * UIProperties.FrameDuration);
		sequence.OnKill(delegate
		{
			market.GetComponent<GridLayoutGroup>().enabled = true;
			foreach (Transform item3 in slots)
			{
				UnityEngine.Object.Destroy(item3.GetChild(1).gameObject);
				item3.GetComponent<Image>().enabled = true;
				item3.GetChild(0).gameObject.SetActive(value: true);
				item3.GetChild(0).GetComponent<Image>().enabled = true;
			}
		});
		return new DOTweenAnimationSequence(sequence);
	}

	private IEnumerable<Transform> GetEmptySlots(Transform market, int amount)
	{
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < market.childCount; i++)
		{
			if (list.Count >= amount)
			{
				break;
			}
			Transform child = market.GetChild(i);
			if (!child.gameObject.activeSelf)
			{
				list.Add(child);
			}
		}
		return list;
	}

	public void OnMouseEnterCoal()
	{
		Vector3 position = _coalMarketGridTransform.position;
		_controller.OnMouseEnter("Coal", position.x, position.y);
	}

	public void OnMouseEnterIron()
	{
		Vector3 position = _ironMarketGridTransform.position;
		_controller.OnMouseEnter("Iron", position.x, position.y);
	}

	public void OnMouseExit()
	{
		_controller.OnMouseExit();
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		coalMarketOutline = base.transform.Find("SafeArea/Popup/Markets/CoalMarketOutline");
		ironMarketOutline = base.transform.Find("SafeArea/Popup/Markets/IronMarketOutline");
		_coalMarketGridTransform = base.transform.Find("SafeArea/Popup/Markets/CoalMarket/CoalGrid");
		_ironMarketGridTransform = base.transform.Find("SafeArea/Popup/Markets/IronMarket/IronGrid");
		_buttonCoalMarket = base.transform.Find("SafeArea/Popup/Markets/CoalMarket").GetComponent<Button>();
		_buttonIronMarket = base.transform.Find("SafeArea/Popup/Markets/IronMarket").GetComponent<Button>();
		_audioSource = GetComponent<AudioSource>();
	}
}
