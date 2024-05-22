using UnityEngine;

// 체력을 회복하는 아이템
public class HealthPack : MonoBehaviour, IItem {
    public float health = 25; // 체력을 회복할 수치

    public void Use(GameObject target) {
        if (target.CompareTag("Player"))
        {
            PlayerHealth player = target.GetComponent<PlayerHealth>();
            player.RestoreHealth(health);
            Destroy(gameObject);
        }
    }
}