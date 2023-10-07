# АНАЛИЗ ДАННЫХ И ИСКУССТВЕННЫЙ ИНТЕЛЛЕКТ [in GameDev]
Отчет по лабораторной работе #3 выполнил(а):
- Глейзер Роман
- РИ-220930

| Задание | Выполнение | Баллы |
| ------ | ------ | ------ |
| Задание 1 | * | 60 |
| Задание 2 | * | 20 |
| Задание 3 | * | 20 |

знак "*" - задание выполнено; знак "#" - задание не выполнено;

Работу проверили:
- к.т.н., доцент Денисов Д.В.
- к.э.н., доцент Панов М.А.
- ст. преп., Фадеев В.О.

[![N|Solid](https://cldup.com/dTxpPi9lDf.thumb.png)](https://nodesource.com/products/nsolid)

[![Build Status](https://travis-ci.org/joemccann/dillinger.svg?branch=master)](https://travis-ci.org/joemccann/dillinger)

## Цель работы
Разработать оптимальный баланс для десяти уровней игры Dragon Picker.

## Задание 1
### Предложите вариант изменения найденных переменных для 10 уровней в игре. Визуализируйте изменение уровня сложности в таблице. 

Ход работы:
- Скачать репозиторий игры Dragon Picker, изучить скрипты игровых объектов, выделить переменные, которые можно изменять в 10 уровнях, предложить вариант по изменению найденных переменных, визуализировать эти переменные в Google таблице.

## Переменные, которые подвергнутся изменению в течение 10 уровней:

### Скорость движение дракона(speed). 
- Для усложнения прохождения необходимо сделать поведение дракона более непредсказуемым. Одним из вариантов усложнять прохождение игры является увеличение скорости в каждом новом уровне.

### Расстояние, проходимое драконом (leftRightDistance). 
- Эту переменную, я считаю, нужно увеличивать на каждом уровне. Если увеличивать скорость и расстояние, проходимое драконом, то, по итогу, успевать за ним станет гораздо сложнее.

### Время между сбрасыванием яиц (timeBetweenEggDrops). 
- В первом уровне сбрасывание яйца происходит каждые 2 секунды. Для усложнения прохождения игры я считаю, что стоит уменьшить временной интервал сбрасывания яиц.

### Визуализироваю данные в таблице: https://docs.google.com/spreadsheets/d/1OzlxX6G3bSaRnJ1VfVkes8sOARoCrRixCk0mSf67HCg/edit#gid=0

![image](https://github.com/RomanGleizer/Workshop3/assets/125725530/f2a02008-17ba-43d0-bd29-709ca94832d3)

## Задание 2
### Создайте 10 сцен на Unity с изменяющимся уровнем сложности.

Ход работы:
- Создать ещё 9 аналогичных сцен, написать код, который будет считывать данные из таблицы и выставлять переменным значения, в зависимости от уровня.

## Создание уровней: 
![image](https://github.com/RomanGleizer/Workshop3/assets/125725530/dbdc723f-5577-4691-9669-a7d17de5baaf)

## Для того, чтобы задать переменным нужные значения, необходимо сначала получить данные из таблицы, а затем присвоить игровым переменным нужные значения. Для этого я написал следующий код:

- Код GoogleSheetsParser.cs
```cs

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

```

- Код DataInitializer.cs
```cs

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

```

- Измененный код EnemyDragon.cs, в котором добавлен код, изменяющий значения у переменных, в зависимости от уровня.
```cs

using UnityEngine;

public class EnemyDragon : MonoBehaviour
{
    [SerializeField] private GoogleSheetsParser parser;
    [SerializeField] private int levelIndex;

    public GameObject dragonEggPrefab;
    public float speed;
    public float leftRightDistance;
    public float timeBetweenEggDrops;
    public float chanceDirection;

    private void Awake()
    {
        StartCoroutine(parser.ParseGoogleSheets());
    }

    private void Start()
    {
        Invoke(nameof(Initialize), 1f);
        Invoke("DropEgg", 2f);
    }

    private void Update()
    {
        Vector3 pos = transform.position;
        pos.x += speed * Time.deltaTime;
        transform.position = pos;

        if (pos.x < -leftRightDistance) speed = Mathf.Abs(speed);
        else if (pos.x > leftRightDistance) speed = -Mathf.Abs(speed);
    }

    private void DropEgg()
    {
        Vector3 myVector = new Vector3(0.0f, 5.0f, 0.0f);
        GameObject egg = Instantiate(dragonEggPrefab);
        egg.transform.position = transform.position + myVector;
        Invoke("DropEgg", timeBetweenEggDrops);
    }

    private void FixedUpdate() 
    {
        if (Random.value < chanceDirection) speed *= -1;
    }

    private void Initialize()
    {
        speed = parser.Data[levelIndex][0];
        leftRightDistance = parser.Data[levelIndex][1];
        timeBetweenEggDrops = parser.Data[levelIndex][2];
        chanceDirection = 0.01f;
    }
}

```
### Пример того, что код работает:

### Скрин с 1 уровня
![image](https://github.com/RomanGleizer/Workshop3/assets/125725530/c4fc67ad-7c37-4323-bf75-5345d531a561)

### Скрин со 2 уровня
![image](https://github.com/RomanGleizer/Workshop3/assets/125725530/10390c4a-92bb-44af-a082-f8fcd63c3a56)

### Скрин со 3 уровня
![image](https://github.com/RomanGleizer/Workshop3/assets/125725530/5bf27cff-f4e1-47b6-bd7c-7157fc12ecd3)

### Скрин с 4 уровня
![image](https://github.com/RomanGleizer/Workshop3/assets/125725530/ec434cb7-5cf3-43bb-a2e0-fbf8dd03162a)

### Скрин с 5 уровня
![image](https://github.com/RomanGleizer/Workshop3/assets/125725530/db83ed6b-999b-4cd6-bd52-152cedb64416)

### Скрин с 6 уровня
![image](https://github.com/RomanGleizer/Workshop3/assets/125725530/1c9ae894-30bc-4621-8ab0-3aaeed274086)

### Скрин с 7 уровня
![image](https://github.com/RomanGleizer/Workshop3/assets/125725530/b93ce82c-1d8e-4763-9553-5d5acfd77932)

### Скрин с 8 уровня
![image](https://github.com/RomanGleizer/Workshop3/assets/125725530/3b78b68d-6abf-4997-aec1-30a4326c357a)

### Скрин с 9 уровня
![image](https://github.com/RomanGleizer/Workshop3/assets/125725530/80561bb9-ddb5-44b2-be3e-213b735fb586)

### Скрин с 10 уровня
![image](https://github.com/RomanGleizer/Workshop3/assets/125725530/7574e9d1-d9bb-46e0-b66d-44922955438f)

## Задание 3
### Решение в 80+ баллов должно заполнять google-таблицу данными из Python. В Python данные также должны быть визуализированы.

Ход работы:
- Написать код заполняющий Google таблицу данными из скрипта. Результатом является скрин с первого задания. В Unity также данные изменяются. Результат изменения данных в Unity - скриншоты со 2 задания 

```py

import gspread
import random

gc = gspread.service_account(filename='unityworkshop3-3388140cb2e7.json')
sh = gc.open("Workshop3")


def set_values(column, start_min, start_max, delta):
    counter = 2
    min_limit = start_min
    max_limit = start_max
    while counter < 11:
        counter += 1
        sh.sheet1.update((column + str(counter)), str(random.uniform(min_limit, max_limit)))
        min_limit, max_limit = min_limit + delta, max_limit + delta


set_values(column='B', start_min=5, start_max=5.5, delta=1.75)
set_values(column='D', start_min=1.8, start_max=1.9, delta=-0.17)
set_values(column='C', start_min=11, start_max=12, delta=0.78)

```

## Выводы

Сегодня я впервые попробовал сделать баланс в игре, повторил, как заполнять Google таблицу данными и повторил, как считывать данные из таблицы.

| Plugin | README |
| ------ | ------ |
| Dropbox | [plugins/dropbox/README.md][PlDb] |
| GitHub | [plugins/github/README.md][PlGh] |
| Google Drive | [plugins/googledrive/README.md][PlGd] |
| OneDrive | [plugins/onedrive/README.md][PlOd] |
| Medium | [plugins/medium/README.md][PlMe] |
| Google Analytics | [plugins/googleanalytics/README.md][PlGa] |

## Powered by

**BigDigital Team: Denisov | Fadeev | Panov**
