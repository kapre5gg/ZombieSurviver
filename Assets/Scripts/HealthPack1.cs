using UnityEngine;
using UnityEngine.UI;

// 체력을 회복하는 아이템
public class HealthPack1 : MonoBehaviour, IItem {
    public float health = 10; // 체력을 회복할 수치
    public void Use(GameObject target) {
        if (target.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}