using TMPro;
using UnityEngine;

public class DataInitializer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _targetText;
    [SerializeField] private int targetScore;

    public int TargetScore => targetScore;

    private GoogleSheetsParser parser;

    private void Awake()
    {
        _targetText.text = $"Target : {targetScore}";
        parser = GetComponent<GoogleSheetsParser>();
        StartCoroutine(parser.ParseGoogleSheets());
    }
}
