using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
	public struct Idle
	{
		public Sprite idle1;
	}

	[SerializeField]
	private GameObject _spriteQuad = null;

	[SerializeField]
	private Button _selectButton = null;

	[SerializeField]
	private Button _prevButton = null;

	[SerializeField]
	private Button _nextButton = null;

	[SerializeField]
	private Text _charName = null;

	private List<GameObject> _spriteQuads = new List<GameObject>();
	private List<SpriteRenderer> _renderers = new List<SpriteRenderer>();
	private List<Idle> _idleSprites = new List<Idle>();

	private char[] alphabet = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'/*, '?', '!', '#', '.', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' */ };

	public float _radius		= 2.0f;
	private float _tOffset		= 0.0f;
	private float _nextTOffset	= 0.0f;
	private int _index			= 0;
	private bool _bNeedUpdate	= true;
	public bool _bIsReady { get; private set; } = false;

	private float _idleFrequency = 0.3f;
	private float _idleCount = 0.0f;

	private void Start()
	{

		int count = alphabet.Length;

		for (int i = 0; i < count; i++)
		{
			_spriteQuads.Add(Instantiate(_spriteQuad, transform, false));
			Sprite[] sprites = Resources.LoadAll<Sprite>("Visuals/Sprites/Alphabet/" + alphabet[i] + "_Sprites.png");

			if (sprites.Length == 0)
				sprites = Resources.LoadAll<Sprite>("Visuals/Sprites/Alphabet/X_Sprites.png");

			Idle idle;
			idle.idle1 = sprites[0];
			_idleSprites.Add(idle);

			_renderers.Add(_spriteQuads[i].GetComponent<SpriteRenderer>());
			_renderers[i].sprite = idle.idle1;

			if (i == 0)
				_renderers[i].color = Color.white;
			else if (i == 1 || i == count - 1)
				_renderers[i].color = Color.white * 0.5f;
			else
				_renderers[i].color = Color.white * 0.0f;
		}

		_charName.text = alphabet[_index].ToString();
		_charName.GetComponent<Outline>().enabled = false;
	}

	private void Update()
	{

		if (!_bNeedUpdate)
			return;

		_tOffset = Mathf.Lerp(_tOffset, _nextTOffset, Time.deltaTime * 10.0f);

		int count = alphabet.Length;

		if (_tOffset <= _nextTOffset + 0.01f && _tOffset >= _nextTOffset - 0.01f)
		{
			_tOffset = _nextTOffset;
			_bNeedUpdate = false;
			_tOffset = _tOffset % (Mathf.PI * 2.0f);
			_nextTOffset = _nextTOffset % (Mathf.PI * 2.0f);
		}

		for (int i = 0; i < count; i++)
		{
			Vector3 offset = Vector3.zero;
			offset.x = _radius * Mathf.Cos(i * Mathf.PI / count * 2.0f - Mathf.PI / 2.0f + _tOffset);
			offset.z = _radius * Mathf.Sin(i * Mathf.PI / count * 2.0f - Mathf.PI / 2.0f + _tOffset);
			_spriteQuads[i].transform.position = transform.position + offset;
		}
	}

	public void Rotate(bool clokwise)
	{
		if (_bIsReady)
			return;

		_nextTOffset = _nextTOffset +  Mathf.PI / alphabet.Length * 2.0f * (clokwise ? 1.0f : -1.0f);
		_bNeedUpdate = true;
		
		int count = alphabet.Length;

		if (clokwise)
			_index--;
		else
			_index++;

		if (_index < 0)	
			_index = count - 1;
		else if (_index == count)
			_index = 0;

		int prev = _index - 1;
		int next = _index + 1;
		
		if (prev < 0)
			prev = count - 1;
		else if (prev == count)
			prev = 0;
		if (next < 0)
			next = count - 1;
		else if (next == count)
			next = 0;

		for (int i = 0; i < count; i++)
		{
			if (i == _index)
				_spriteQuads[i].GetComponent<SpriteRenderer>().color = Color.white;
			else if (i == prev || i == next)
				_spriteQuads[i].GetComponent<SpriteRenderer>().color = Color.white * 0.5f;
			else
				_spriteQuads[i].GetComponent<SpriteRenderer>().color = Color.white * 0.0f;

			if (i != _index)
				_renderers[i].sprite = _idleSprites[i].idle1;
		}

		_charName.text = alphabet[_index].ToString();
	}

	public void Shuffle()
	{
		//if (_bIsReady)
			//ToggleReady();

		int rand = Random.Range(0, alphabet.Length * 2);
		int lastIndex = _index;

		for (int i = 0; i < rand; i++)
			Rotate(true);

		if (_index == lastIndex)
			Rotate(true);
	}

	/*public void ToggleReady()
	{
		_bIsReady = !_bIsReady;

		if (_bIsReady)
			_selectedCharacter = _characters[_index];
		else
			_selectedCharacter = null;

		_nextButton.enabled = !_bIsReady;
		_nextButton.image.color = (_bIsReady ? _nextButton.colors.disabledColor : _nextButton.colors.normalColor);
		_prevButton.enabled = !_bIsReady;
		_prevButton.image.color = (_bIsReady ? _prevButton.colors.disabledColor : _prevButton.colors.normalColor);
		_selectButton.image.color = (_bIsReady ? _characters[_index]._color1 : _prevButton.colors.normalColor);

		Outline outline = _charName.GetComponent<Outline>();
		outline.enabled = _bIsReady;
		outline.effectColor = _characters[_index]._color3;
		//outline.effectColor = Color.white;
	}*/
}
