using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Serializable]
    public struct PlayerData
    {
        public Color    hairColor;
        public Color    bodyColor;
        public int      deviceId;
    }

    [SerializeField] private int                _numPlayers = 1;
    [SerializeField] private List<PlayerData>   _playerData;

    static GameManager _Instance;

    public static GameManager Instance => _Instance;

    void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public PlayerData GetPlayerData(int playerId)
    {
        if (_playerData == null) _playerData = new();

        for (int i = _playerData.Count; i <= playerId; i++)
        {
            _playerData.Add(new PlayerData());
        }

        return _playerData[playerId];
    }

    public void SetPlayerData(int playerId, PlayerData pd)
    {
        if (_playerData == null) _playerData = new();

        for (int i = _playerData.Count; i <= playerId; i++)
        {
            _playerData.Add(new PlayerData());
        }

        _playerData[playerId] = pd;
    }

    public int numPlayers
    {
        get { return _numPlayers; }
        set { _numPlayers = value; }
    }
}
