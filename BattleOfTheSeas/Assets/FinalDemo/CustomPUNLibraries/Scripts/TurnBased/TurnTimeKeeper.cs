using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnTimeKeeper : MonoBehaviour
{
    private Text _timeText;
    
    void Start()
    {
        _timeText = GetComponent<Text>();
    }
    void Update()
    {
        if (_timeText)
            _timeText.text = TurnBasedSystem.Instance.GetRemainingTime().ToString();
    }
}
