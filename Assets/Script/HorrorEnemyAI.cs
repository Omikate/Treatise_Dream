using UnityEngine;
using UnityEngine.AI;

public class HorrorEnemyAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;
    public float walkSpeed = 3.5f;
    public float runSpeed = 8.0f;

    [Header("Detection Settings")]
    public Transform player;         
    public float viewDistance = 15f; 
    public float viewAngle = 90f;    
    public LayerMask obstructionMask;

    [Header("Attack Settings")]
    public float damageAmount = 20f; 
    public float attackRate = 1.0f;  
    private float nextAttackTime;

    [Header("Audio Settings")]
    public AudioSource vocalSource;     // ลำโพงเสียงร้องของศัตรู
    public AudioClip spotPlayerSound;   // เสียงร้องตกใจเวลาเจอหน้าเรา
    
    [Space]
    public AudioSource footstepSource;  // ลำโพงเสียงเท้า
    public AudioClip footstepSound;     // ไฟล์เสียงฝีเท้า
    public float walkStepInterval = 0.6f; // ความถี่เสียงเท้าตอนเดิน
    public float runStepInterval = 0.3f;  // ความถี่เสียงเท้าตอนวิ่งไล่
    private float stepTimer;

    private bool isChasing = false; // ตัวเช็คว่ากำลังวิ่งไล่อยู่ไหม

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed; // เริ่มต้นด้วยความเร็วเดิน
        
        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    void Update()
    {
        if (CanSeePlayer())
        {
            // --- จังหวะที่เพิ่งเห็นผู้เล่นครั้งแรก ---
            if (!isChasing)
            {
                isChasing = true;
                agent.speed = runSpeed; // เปลี่ยนเป็นความเร็ววิ่ง
                
                // เล่นเสียงร้องคำราม!
                if (vocalSource != null && spotPlayerSound != null)
                {
                    vocalSource.PlayOneShot(spotPlayerSound);
                }
            }
            
            agent.SetDestination(player.position);
            HandleFootsteps(runStepInterval); // เล่นเสียงเท้าแบบถี่ๆ (วิ่ง)
        }
        else
        {
            // --- จังหวะที่คลาดกับผู้เล่น ---
            if (isChasing)
            {
                isChasing = false;
                agent.speed = walkSpeed; // กลับมาเดินปกติ
            }
            
            Patrol();
            HandleFootsteps(walkStepInterval); // เล่นเสียงเท้าแบบช้าๆ (เดิน)
        }
    }

    void HandleFootsteps(float interval)
    {
        // เช็คว่าศัตรูกำลังขยับอยู่จริงๆ (ความเร็วมากกว่า 0.1)
        if (agent.velocity.magnitude > 0.1f && footstepSource != null && footstepSound != null)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0)
            {
                footstepSource.PlayOneShot(footstepSound);
                stepTimer = interval; // รีเซ็ตเวลาเพื่อรอเล่นเสียงเท้าก้าวต่อไป
            }
        }
    }

    bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < viewDistance)
        {
            if (Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2)
            {
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

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= nextAttackTime)
            {
                PlayerStats pStats = collision.gameObject.GetComponent<PlayerStats>();
                if (pStats != null)
                {
                    pStats.TakeDamage(damageAmount);
                    nextAttackTime = Time.time + attackRate;
                }
            }
        }
    }
}