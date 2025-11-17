using UnityEngine;

public class NoiseManager : MonoBehaviour
{
    public static NoiseManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void GenerateNoise(Vector3 position, float radius)
    {
        // Notify all enemies about the noise
        JimmyAI[] enemies = FindObjectsOfType<JimmyAI>();
        
        foreach (JimmyAI enemy in enemies)
        {
            float distance = Vector3.Distance(position, enemy.transform.position);
            if (distance <= radius)
            {
                enemy.HearNoise(position);
            }
        }
    }
}
