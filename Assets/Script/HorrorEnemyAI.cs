using UnityEngine;
using UnityEngine.AI;

public class HorrorEnemyAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;

    [Header("Detection Settings")]
    public Transform player;         // ลากตัวละคร Player มาใส่ตรงนี้
    public float viewDistance = 15f; // ระยะการมองเห็น
    public float viewAngle = 90f;    // องศาการมองเห็น (กว้างแค่ไหน)
    public LayerMask obstructionMask;// เลเยอร์ของกำแพง/สิ่งกีดขวาง (ให้ติ๊กเลือก Map หรือ Obstruction)

    [Header("Attack Settings")]
    public float damageAmount = 20f; // ตีแรงแค่ไหน (เลือดเต็ม 100)
    public float attackRate = 1.0f;  // ความเร็วในการตีซ้ำ (ตีทุกๆ 1 วินาที)
    private float nextAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // ถ้ามีจุดเดินลาดตระเวน ให้เริ่มเดินไปจุดแรก
        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    void Update()
    {
        if (CanSeePlayer())
        {
            // ถ้าเห็นผู้เล่น ให้วิ่งไล่ตาม!
            agent.SetDestination(player.position);
        }
        else
        {
            // ถ้าไม่เห็น หรือผู้เล่นแอบอยู่ ให้เดินลาดตระเวนต่อไป
            Patrol();
        }
    }

    bool CanSeePlayer()
    {
        // คำนวณทิศทางและระยะห่างระหว่างศัตรูกับผู้เล่น
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 1. เช็คว่าอยู่ในระยะสายตาหรือไม่
        if (distanceToPlayer < viewDistance)
        {
            // 2. เช็คว่าอยู่ในองศาการมองเห็นด้านหน้าหรือไม่
            if (Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2)
            {
                // 3. ยิงเลเซอร์ (Raycast) ไปหาผู้เล่น เช็คว่ามีกำแพงขวางไหม
                // ถ้า "ไม่ชน" กำแพง (obstructionMask) แปลว่ามองเห็นผู้เล่นเต็มๆ
                if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstructionMask))
                {
                    return true; 
                }
            }
        }
        return false;
    }

    void Patrol()
    {
        if (waypoints.Length == 0) return;

        // ถ้าเดินมาถึงจุดที่กำหนดแล้ว (เหลือระยะทางน้อยกว่า 0.5) ให้เปลี่ยนไปจุดถัดไป
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    // ระบบทำดาเมจเมื่อเดินมาชนตัวผู้เล่น
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // เช็คว่าถึงเวลาที่จะโจมตีครั้งต่อไปหรือยัง (หน่วงเวลาตาม attackRate)
            if (Time.time >= nextAttackTime)
            {
                PlayerStats pStats = collision.gameObject.GetComponent<PlayerStats>();
                if (pStats != null)
                {
                    pStats.TakeDamage(damageAmount); // สั่งลดเลือดและร้องโอ๊ย!
                    nextAttackTime = Time.time + attackRate; // รีเซ็ตคูลดาวน์การตี
                }
            }
        }
    }
}