using UnityEngine;

public class ParticleSwarmController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    [Header("References")]
    [SerializeField] private Transform playerTransform;
    
    private PlayerController player;

    private void Start() => player = playerTransform.GetComponent<PlayerController>();

    private void Update()
    {
        if(player.inSafeZone) return;

        //slowly move towards the player's current position
        Vector3 target = playerTransform.position + Vector3.up * 2.5f;
        Vector3 dir = (target - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
    }
}
