using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Visuals (Optional)")]
    public GameObject activeEffect; // ใส่เอฟเฟกต์ไฟหรือแสงเพื่อให้รู้ว่าเซฟแล้ว

    void Start()
    {
        if (activeEffect != null) activeEffect.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // ถ้าผู้เล่นเดินเข้ามาสัมผัส
        if (other.CompareTag("Player"))
        {
            // บันทึกตำแหน่งของจุดนี้ลงใน Manager
            CheckpointManager.instance.lastCheckPointPos = transform.position;
            
            if (activeEffect != null) activeEffect.SetActive(true);
            
            Debug.Log("บันทึกจุดเซฟเรียบร้อย!");
            
            // (Optional) ถ้าอยากให้เซฟได้ครั้งเดียวแล้วหายไปเลย ให้ใส่บรรทัดล่างนี้ครับ
            // GetComponent<Collider>().enabled = false; 
        }
    }
}