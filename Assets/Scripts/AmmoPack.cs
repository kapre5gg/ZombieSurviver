using UnityEngine;

// 총알을 충전하는 아이템
public class AmmoPack : MonoBehaviour, IItem {
    public int ammo = 25; // 충전할 총알 수

    public void Use(GameObject target) {
        if (target.CompareTag("Player"))
        {
            Gun playerGun = target.GetComponentInChildren<Gun>();
            if (playerGun == null) return;
            playerGun.ammoRemain += ammo;
            Destroy(gameObject);

        }
    }
}