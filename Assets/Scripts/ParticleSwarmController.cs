using UnityEngine;

public class ParticleSwarmController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    [Header("References")]
    [SerializeField] private Transform playerTransform;

    private void Update()
    {
        //slowly move towards the player's current position
        Vector3 target = playerTransform.position + Vector3.up * 2.5f;
        Vector3 dir = (target - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
    }
}
