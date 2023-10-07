using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System.Globalization;

public class GoogleSheetsParser : MonoBehaviour
{
    private List<float[]> data = new List<float[]>();

    private readonly string uri = "https://sheets.googleapis.com/v4/spreadsheets/" +
            "1OzlxX6G3bSaRnJ1VfVkes8sOARoCrRixCk0mSf67HCg/values/A1%3AZ100" +
            "?key=AIzaSyCxD81TDAMungSmcTnRUBXrHm_YDdq8UPQ";

    public List<float[]> Data => data;

    public IEnumerator ParseGoogleSheets()
    {
        UnityWebRequest curentResp = UnityWebRequest.Get(uri);

        yield return curentResp.SendWebRequest();
        string rawResp = curentResp.downloadHandler.text;
        var rawJson = JSON.Parse(rawResp);

        foreach (var itemRawJson in rawJson["values"])
        {
            var parseJson = JSON.Parse(itemRawJson.ToString());
            var selectRow = parseJson[0].AsStringList;
            var count = 0;
            var arr = new float[3];

            foreach (var row in selectRow)
            {
                if (float.TryParse(row, NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"), out _) & count <= 3)
                {
                    arr[count] = float.Parse(row, NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"));
                    count++;
                }

                if (count == 3) data.Add(arr);
            }
        }
    }
}
