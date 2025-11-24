using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameManager instance;
    public GameManager Instance { get; private set; }

    int happiness;  // 어항 행복도
    int money;      // 재화

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
