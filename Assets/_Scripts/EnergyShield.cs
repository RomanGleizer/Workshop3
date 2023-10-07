using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class EnergyShield : MonoBehaviour
{
    public TextMeshProUGUI scoreGT;
    public AudioSource audioSource;

    private int score;
    private Camera camera;

    void Start() 
    {
        GameObject scoreGO = GameObject.Find("Score");
        camera = FindObjectOfType<Camera>();

        scoreGT = scoreGO.GetComponent<TextMeshProUGUI>();
        scoreGT.text = "0";
    }

    void Update()
    {
        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);
        Vector3 pos = this.transform.position;
        pos.x = mousePos3D.x;
        this.transform.position = pos;
    }

    private void OnCollisionEnter(Collision coll) 
    {
        GameObject Collided = coll.gameObject;
        if (Collided.tag == "Dragon Egg")
            Destroy(Collided);

        FindObjectsOfType<EnergyShield>().ToList().ForEach(shield => shield.score++);
        scoreGT.text = score.ToString();
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();

        if (score == camera.GetComponent<DataInitializer>().TargetScore)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
