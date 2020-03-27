using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorManager : MonoBehaviour
{
	/*[SerializeField]
	private CharacterSelector _playerOneSelector = null;

	[SerializeField]
	private CharacterSelector _playerTwoSelector = null;

	[SerializeField]
	private GameObject _startbutton = null;

	//public CharacterData _playerOneData { get; private set; } = null;
	//public CharacterData _playerTwoData { get; private set; } = null;

	private bool _bIsReady = false;

	private void Start()
	{
		_startbutton.SetActive(false);

		DontDestroyOnLoad(gameObject);
	}

	private void Update()
	{
		if (_startbutton != null && _startbutton.activeSelf)
		{
			_startbutton.transform.localScale = Vector3.one * Mathf.Cos(Time.time * 2.0f) * 0.2f + Vector3.one;
		}
	}

	public void CheckIfReady()
	{
		_bIsReady = _playerOneSelector._bIsReady && _playerTwoSelector._bIsReady;

		if (_bIsReady && !_startbutton.activeSelf)
		{
			_startbutton.gameObject.SetActive(true);
			_playerOneData = _playerOneSelector._selectedCharacter;
			_playerTwoData = _playerTwoSelector._selectedCharacter;
		}
		else if (!_bIsReady && _startbutton.activeSelf)
			_startbutton.gameObject.SetActive(false);
	}*/
}
