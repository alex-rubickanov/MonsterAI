using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private float distanceToDie;
    [SerializeField] private Transform enemyTransform;
    [SerializeField] private GameObject deathScreen;


    private void Start()
    {
        Time.timeScale = 1;
        deathScreen.SetActive(false);
    }

    private void Update()
    {
        if ((enemyTransform.position - transform.position).magnitude <= distanceToDie)
        {
            Time.timeScale = 0;
            deathScreen.SetActive(true);
        }
    }
}
