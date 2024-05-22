using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 적 게임 오브젝트를 주기적으로 생성
public class BossSpawner : MonoBehaviour
{
    public Enemy[] bossPrefab; // 생성할 적 AI

    public Transform[] spawnPoints; // 적 AI를 소환할 위치들

    public float damageMax = 40f; // 최대 공격력
    public float damageMin = 20f; // 최소 공격력

    public float healthMax = 200f; // 최대 체력
    public float healthMin = 100f; // 최소 체력

    public float speedMax = 3f; // 최대 속도
    public float speedMin = 1f; // 최소 속도


    public Color strongEnemyColor = Color.blue; // 보스에게는 필요없음

    private List<Enemy> bossList = new List<Enemy>(); // 생성된 보스를 담는 리스트?
    public int stage = 0; // 현재 웨이브

    public int[] bossTimerList;

    private void Start()
    {
        StartCoroutine(BossTimer());
    }
    private IEnumerator BossTimer()
    {
        while (bossList.Count == 0)
        {
            yield return new WaitForSeconds(1);
            bossTimerList[stage]--;
            print(bossTimerList[stage]);

            if (bossTimerList[stage] == 0)
            {
                SpawnStageBoss();
            }
        } 
    }

    private void UpdateUI()
    {
        // 현재 보스를 표시
        //UIManager.instance.UpdateWaveText(stage, enemies.Count);
    }
    // 현재 웨이브에 맞춰 적을 생성
    private void SpawnStageBoss()
    {
        CreateBoss(Random.value);
    }

    // 적을 생성하고 생성한 적에게 추적할 대상을 할당
    private void CreateBoss(float intensity)
    {
        //Lerp : 선형 보간
        float _s = Mathf.Lerp(speedMin, speedMax, intensity);
        float _d = Mathf.Lerp(damageMin, damageMax, intensity);
        float _h = Mathf.Lerp(healthMin, healthMax, intensity);
        Color _c = Color.blue;
        Enemy boss = Instantiate(bossPrefab[stage], spawnPoints[SpawnInts()].position, Quaternion.identity);
        boss.Setup(_h, _d, _s, _c);
        //보스를 리스트에 추가
        bossList.Add(boss);

        boss.onDeath += () => bossList.Remove(boss);
        boss.onDeath += () => Destroy(boss.gameObject, 10f);
        //보스가 죽으면 스테이지가 올라간다.
        boss.onDeath += () => stage++;
        boss.onDeath += () => StartCoroutine(BossTimer());

        boss.onDeath += () => GameManager.instance.AddScore((int)(intensity * 100));
    }
    private int count = -1;
    public int skipPoint = 3;//특정 스폰 지역을 스킾
    private int SpawnInts()
    {
        if (count == skipPoint)
            count++;

        if (count >= spawnPoints.Length - 1)
            count = -1;


        count++;
        return count;
    }
}