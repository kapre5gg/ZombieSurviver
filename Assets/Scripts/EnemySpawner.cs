using System.Collections.Generic;
using System.Linq;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;

// 적 게임 오브젝트를 주기적으로 생성
public class EnemySpawner : MonoBehaviour {
    public Enemy enemyPrefab; // 생성할 적 AI

    public Transform[] spawnPoints; // 적 AI를 소환할 위치들

    public float damageMax = 40f; // 최대 공격력
    public float damageMin = 20f; // 최소 공격력

    public float healthMax = 200f; // 최대 체력
    public float healthMin = 100f; // 최소 체력

    public float speedMax = 3f; // 최대 속도
    public float speedMin = 1f; // 최소 속도

    public Color strongEnemyColor = Color.red; // 강한 적 AI가 가지게 될 피부색

    private List<Enemy> enemies = new List<Enemy>(); // 생성된 적들을 담는 리스트
    private int wave; // 현재 웨이브

    private void Update() {
        //현재 좀비를 생성해야하는지 체크, 게임오버, 웨이브 에너미가 0마리 (모두 물리쳤는지)
        if (GameManager.instance != null && GameManager.instance.isGameover == true)
        {
            return;
        }
        if (enemies.Count <= 0)
        {
            SpawnWave();
        }
        UpdateUI();
    }

    // 웨이브 정보를 UI로 표시
    private void UpdateUI() {
        // 현재 웨이브와 남은 적의 수 표시
        UIManager.instance.UpdateWaveText(wave, enemies.Count);
    }
    // 현재 웨이브에 맞춰 적을 생성
    private void SpawnWave() {
        //실행할 때 마다 난이도가 오라가게
        //웨이브 증가 적 생산 
        wave++;
        int spawnCount = Mathf.RoundToInt(wave * 3f);
        for (int i = 0; i < spawnCount; i++)
        {
            CreateEnemy(Random.value);
        }        
    }

    // 적을 생성하고 생성한 적에게 추적할 대상을 할당
    private void CreateEnemy(float intensity) {
        //Lerp : 선형 보간
        float _s = Mathf.Lerp(speedMin, speedMax, intensity);
        float _d = Mathf.Lerp(damageMin, damageMax, intensity);
        float _h = Mathf.Lerp(healthMin, healthMax, intensity);
        Color _c = Color.Lerp(Color.black, strongEnemyColor, intensity);


        Enemy enemy = Instantiate(enemyPrefab, spawnPoints[SpawnInts()].position, Quaternion.identity);
        enemy.Setup(_h, _d, _s, _c);
        //좀비 리스트에 추가
        enemies.Add(enemy);
        //좀비가 죽으면 할 행동을 action 에 추가
        enemy.onDeath += () => enemies.Remove(enemy);
        enemy.onDeath += () => Destroy(enemy.gameObject, 10f);
        //점수 상승
        enemy.onDeath += () => GameManager.instance.AddScore((int)(intensity * 100));
    }
    private int count = -1;
    public int skipPoint = 3;//특정 스폰 지역을 스킾?
    private int SpawnInts()
    {
        count++;
        if (count == skipPoint)
            count++;
        if (count >= spawnPoints.Length - 1)
            count = 0;
        return count;
    }

    private void SpecialEnemySpawn() //임시로 만듬 날아가는 애들을 위한
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnPoints[i].position = spawnPoints[skipPoint].position + new Vector3(0, 0, i * Random.value);
        }
    }
}