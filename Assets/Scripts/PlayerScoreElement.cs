using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerScoreElement : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text nameText;

    public void Init(string playerName, int score)
    {
        nameText.text = playerName;
        scoreText.text = score.ToString();
    }
}
