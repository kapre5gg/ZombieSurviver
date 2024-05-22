using UnityEngine;

// 게임 점수를 증가시키는 아이템
public class Coin : MonoBehaviour, IItem {
    public int score = 300; // 증가할 점수

    public void Use(GameObject target) {
        if (target.CompareTag("Player"))
        {
            GameManager.instance.AddScore(score);
            Destroy(gameObject);
        }
    }
}