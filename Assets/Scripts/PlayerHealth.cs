using UnityEngine;
using UnityEngine.UI; // UI 관련 코드

// 플레이어 캐릭터의 생명체로서의 동작을 담당
public class PlayerHealth : LivingEntity {
    public Slider healthSlider; // 체력을 표시할 UI 슬라이더
    public Image shieldSlider;

    public AudioClip deathClip; // 사망 소리
    public AudioClip hitClip; // 피격 소리
    public AudioClip itemPickupClip; // 아이템 습득 소리

    private AudioSource playerAudioPlayer; // 플레이어 소리 재생기
    private Animator playerAnimator; // 플레이어의 애니메이터

    private PlayerMovement playerMovement; // 플레이어 움직임 컴포넌트
    private PlayerShooter playerShooter; // 플레이어 슈터 컴포넌트

    private void Awake() {
        // 사용할 컴포넌트를 가져오기
        playerAudioPlayer = GetComponent<AudioSource>();
        playerAnimator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooter = GetComponent<PlayerShooter>();
        shieldSlider.fillAmount = 0;
    }

    protected override void OnEnable() {
        // LivingEntity의 OnEnable() 실행 (상태 초기화)
        base.OnEnable(); //사망 = false, 체력초기화
        //오류를 대비한 작업 체력슬라이더 활성화, 슬라디더 최댓값을 기본값으로, 슬라이더 체력을 현재값으로
        healthSlider.gameObject.SetActive(true);
        healthSlider.maxValue = 100f;
        healthSlider.value = healthSlider.maxValue;

        playerMovement.enabled = true;
        playerShooter.enabled = true;
    }

    // 체력 회복
    public override void RestoreHealth(float newHealth) {
        // LivingEntity의 RestoreHealth() 실행 (체력 증가)
        base.RestoreHealth(newHealth);
        //회복한걸 슬라이더에 반영
        healthSlider.value = health;
        
    }

    // 데미지 처리
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitDirection) {
        if (shieldSlider.fillAmount >= 0.1f)
        {
            shieldSlider.fillAmount -= 0.1f;
            return;
        }
        // LivingEntity의 OnDamage() 실행(데미지 적용)
        base.OnDamage(damage, hitPoint, hitDirection); //체력감소 , 사망코드가 있음
        if(!dead)
        {
            playerAudioPlayer.PlayOneShot(hitClip);
        }
        healthSlider.value = health;
    }

    // 사망 처리
    public override void Die() {
        // LivingEntity의 Die() 실행(사망 적용)
        base.Die();
        playerAudioPlayer.PlayOneShot(deathClip);
        healthSlider.gameObject.SetActive(false);
        playerAnimator.SetTrigger("Die");
        playerMovement.enabled = false;
        playerShooter.enabled = false;
    }

    private void OnTriggerEnter(Collider other) {
        // 아이템과 충돌한 경우 해당 아이템을 사용하는 처리
        IItem collItem = other.GetComponent<IItem>();
        if (collItem != null && dead == false)
        {
            if (other.CompareTag("healthpack2"))
            {
                shieldSlider.fillAmount += 0.1f;
            }
            collItem.Use(gameObject);
            playerAudioPlayer.PlayOneShot(itemPickupClip);
        }
    }
}