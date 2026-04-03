using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance;
    
    // ตำแหน่งที่จะให้ผู้เล่นไปเกิด
    [HideInInspector] public Vector3 lastCheckPointPos;

    void Awake()
    {
        // ระบบ Singleton เพื่อให้เข้าถึงได้จากทุกสคริปต์
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ให้ตัวจัดการนี้อยู่ข้ามฉากได้
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // เริ่มเกมมา ให้จำตำแหน่งแรกสุดของผู้เล่นไว้เป็นจุดเซฟแรก
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            lastCheckPointPos = player.transform.position;
        }
    }
}