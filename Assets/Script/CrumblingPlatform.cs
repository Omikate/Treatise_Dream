using UnityEngine;
using System.Collections;

public class CrumblingPlatform : MonoBehaviour
{
    [Header("Settings")]
    public float delayBeforeFall = 1.0f; // ระยะเวลาก่อนพื้นจะพัง (วินาที)
    public float destroyDelay = 2.0f;    // ระยะเวลาก่อน Object จะหายไปจากเกม

    private bool isFalling = false;
    private Rigidbody rb;

    void Start()
    {
        // ตรวจสอบว่ามี Rigidbody ไหม ถ้าไม่มีให้เพิ่มเอง
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // เริ่มต้นให้พื้นอยู่นิ่งๆ ไม่ตกตามแรงโน้มถ่วง
        rb.isKinematic = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ตรวจสอบว่าสิ่งที่มาชนคือ Player และพื้นยังไม่กำลังพังอยู่
        if (collision.gameObject.CompareTag("Player") && !isFalling)
        {
            StartCoroutine(FallSequence());
        }
    }

    IEnumerator FallSequence()
    {
        isFalling = true;

        // (Option) ตรงนี้คุณสามารถเพิ่ม Effect สั่น (Shake) เพื่อความน่ากลัวได้
        yield return new WaitForSeconds(delayBeforeFall);

        // ปล่อยให้พื้นตกลงไปตามแรงโน้มถ่วง
        rb.isKinematic = false;
        rb.useGravity = true;

        // ลบ Object ทิ้งหลังจากตกลงไปสักพักเพื่อประหยัด RAM
        Destroy(gameObject, destroyDelay);
    }
}