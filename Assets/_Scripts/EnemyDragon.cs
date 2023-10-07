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
