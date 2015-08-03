using UnityEngine;
using System.Collections;

public class ShootingStarManager : MonoBehaviour
{
    public Transform shootingStarPrefab;
    public float maxTimeBetween = 5.0f;
    public float distanceFromPlayer = 80.0f;
    public float travelDistance = 60.0f;
    public int poolSize = 3;

    private GameObject player;
    private float currentTimeBetween = 5.0f;
    private float timer;

    // use a pool to reuse shooting star objects
    private GameObject[] shootingStars;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag(Tags.player);
        if (!player)
        {
            Camera[] cams = GameObject.FindObjectsOfType<Camera>();
            if (cams.Length > 0)
            {
                player = cams[0].gameObject;
            }
        }
        shootingStars = new GameObject[poolSize];
        for (int i = 0; i < poolSize; ++i)
        {
            shootingStars[i] = ((Transform)Object.Instantiate(shootingStarPrefab, Vector3.zero, Quaternion.identity)).gameObject;
            shootingStars[i].SetActive(false);
        }
        currentTimeBetween = Random.Range(0.0f, maxTimeBetween);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= currentTimeBetween)
        {
            timer = 0.0f;
            ExecuteShootingstar(ChooseFreeShootingStar());

            // execute shooting start randomly but within maxTimeBetween
            currentTimeBetween = Random.Range(0.0f, maxTimeBetween);
        }
    }

    private GameObject ChooseFreeShootingStar()
    {
        foreach (GameObject star in shootingStars)
        {
            if (!star.activeSelf)
            {
                return star;
            }
        }
        return null;
    }

    private void ExecuteShootingstar(GameObject shootingStar)
    {
        if (shootingStar == null) return;

        // spawn a shooting start distanceFromPlayer away from the player in a random direction, but only above the player
        Vector3 direction = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(0.0f, 0.9f), Random.Range(-1.0f, 1.0f));
        shootingStar.transform.position = player.transform.position + direction.normalized * distanceFromPlayer;

        // directions are relative to the player
        Vector3 directionToStar = shootingStar.transform.position - player.transform.position;
        Vector3 directionRight = Vector3.Cross(directionToStar, Vector3.up).normalized;

        // rotate the right vector so it points in an arc downwards
        Quaternion rotation = Quaternion.AngleAxis(Random.Range(10.0f, 170.0f), directionToStar);

        ShootingStar starComponent = shootingStar.GetComponent<ShootingStar>();

         // ensure the shooting star travels downwards relative to the player's horizon
        starComponent.endPos = shootingStar.transform.position + (rotation * directionRight).normalized * travelDistance;
        shootingStar.SetActive(true);
        starComponent.StartEffect();

    }
}
