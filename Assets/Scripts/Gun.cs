using System.Collections;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

// 총을 구현한다
public class Gun : MonoBehaviour {
    // 총의 상태를 표현하는데 사용할 타입을 선언한다
    public enum State {
        Ready, // 발사 준비됨
        Empty, // 탄창이 빔
        Reloading // 재장전 중
    }

    public State state { get; private set; } // 현재 총의 상태

    public Transform fireTransform; // 총알이 발사될 위치

    public ParticleSystem muzzleFlashEffect; // 총구 화염 효과
    public ParticleSystem shellEjectEffect; // 탄피 배출 효과

    private LineRenderer bulletLineRenderer; // 총알 궤적을 그리기 위한 렌더러
    public LineRenderer bulletLineRenderer5;

    private AudioSource gunAudioPlayer; // 총 소리 재생기
    public AudioClip shotClip; // 발사 소리
    public AudioClip reloadClip; // 재장전 소리

    public float damage = 25; // 공격력
    private float fireDistance = 50f; // 사정거리

    public int ammoRemain = 100; // 남은 전체 탄약
    public int magCapacity = 25; // 탄창 용량
    public int magAmmo; // 현재 탄창에 남아있는 탄약


    public float timeBetFire = 0.12f; // 총알 발사 간격
    public float reloadTime = 1.2f; // 재장전 소요 시간
    private float lastFireTime; // 총을 마지막으로 발사한 시점


    private void Awake() {
        // 사용할 컴포넌트들의 참조를 가져오기
        bulletLineRenderer = GetComponent<LineRenderer>();
        gunAudioPlayer = GetComponent<AudioSource>();
        //그외 초기화 : 라인렌더러 초기화
        //선을그리는데 사용할 점
        //프로그램 시작시 2개로 늘려준다
        bulletLineRenderer.positionCount = 2;
        bulletLineRenderer.enabled = false;
    }

    private void OnEnable() {
        // 총 상태 초기화 탄창가득, 총의쏠준비 상태, 마지막으로 총을 쏜시점 0
        magAmmo = magCapacity;   
        state = State.Ready;
        lastFireTime = 0;
    }

    // 발사 시도
    public void Fire() {
        if (Time.time - lastFireTime > timeBetFire && state == State.Ready)
        {
            lastFireTime = Time.time;
            Shot();
        }
        GunUpgrade();
        
    }

    // 실제 발사 처리
    private void Shot() {
        //총이 맞는 위치를 계산 레이캐스트 이용
        //광선이 닿는 위치에 좀비가 있을경우 좀비가 피격당한 것으로 계산
        RaycastHit hit;
        Vector3 hitPos = Vector3.zero;
        if (Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, fireDistance))
        {
            //부딪힌 위치 저장 (좀비체크, 광선 선그리는 용도)
            hitPos = hit.point;
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if (target != null)
            {
                target.OnDamage(damage, hitPos, hit.normal);//맞은 위치, 맞은 방향
             }
        }
        else 
        {
            hitPos = fireTransform.position + fireTransform.forward * fireDistance;
        }

        //광선에 맞든 안맞든 동작하는것 : 이펙트, 궤적, 총알수 감소, 탄알수가 0이면 상태갱신
        StartCoroutine(ShotEffect(hitPos));
        magAmmo--;
        if ( magAmmo <= 0 )
        {
            magAmmo = 0;
            state = State.Empty;
        }
    }

    // 발사 이펙트와 소리를 재생하고 총알 궤적을 그린다
    private IEnumerator ShotEffect(Vector3 hitPosition) {
        if (bulletLineRenderer5 == null)
        {
            muzzleFlashEffect.Play();
            shellEjectEffect.Play();
            gunAudioPlayer.PlayOneShot(shotClip);
            bulletLineRenderer.SetPositions(new Vector3[2] { fireTransform.position, hitPosition });
            bulletLineRenderer.enabled = true;
            yield return new WaitForSeconds(0.03f);
            bulletLineRenderer.enabled = false;
        }        
        else
        {
            muzzleFlashEffect.Play();
            shellEjectEffect.Play();
            gunAudioPlayer.PlayOneShot(shotClip);
            bulletLineRenderer5.SetPositions(new Vector3[6] { fireTransform.position, hitPosition + new Vector3(0, 0, -0.14f), fireTransform.position, hitPosition, fireTransform.position, hitPosition + new Vector3(-0.14f, 0, 0) });
            bulletLineRenderer5.enabled = true;
            yield return new WaitForSeconds(0.03f);
            bulletLineRenderer5.enabled = false;
        }
    }

    // 재장전 시도, 재장전이 이미 실행중이면 무시, 남은 전체탄알 개수가 0, 현재 탄창이 가득차 있을 때
    public bool Reload() {
        if (state == State.Reloading || ammoRemain <= 0 || magAmmo >= magCapacity) 
        {
            return false;
        }
        else
        {
            StartCoroutine(ReloadRoutine());
            return true;
        }        
    }

    // 실제 재장전 처리를 진행
    private IEnumerator ReloadRoutine() {
        state = State.Reloading;
        gunAudioPlayer.PlayOneShot(reloadClip);        
        yield return new WaitForSeconds(reloadTime);

        if (ammoRemain >= magCapacity - magAmmo)
        {
            ammoRemain -= magCapacity - magAmmo;
            magAmmo += magCapacity - magAmmo;
        }
        else
        {
            magAmmo += ammoRemain;
            ammoRemain = 0;
        }
        state = State.Ready;
    }
    //킬카운트마다 무기가 강해진다
    public Material[] gunBody;
    private void GunUpgrade()
    {
        for (int i = 0; i < gunBody.Length; i++)
        {
            Color lerpedColor = Color.Lerp(Color.black, Color.blue, Mathf.Min(UIManager.instance.killcount / 15f, 1f));
            gunBody[i].EnableKeyword("_EMISSION");
            gunBody[i].SetColor("_EmissionColor", lerpedColor);
        }
        if (UIManager.instance.killcount == 5)
        {
            magCapacity = 30;
        }
        if (UIManager.instance.killcount == 10)
        {
            magCapacity = 35;
            damage = 30;
        }
        if (UIManager.instance.killcount == 15)
        {
            magCapacity = 40;
            timeBetFire = 0.08f;
        }
    }

}