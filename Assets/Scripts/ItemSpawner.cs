using UnityEngine;
using UnityEngine.AI; // 내비메쉬 관련 코드

// 주기적으로 아이템을 플레이어 근처에 생성하는 스크립트
public class ItemSpawner : MonoBehaviour {
    public GameObject[] items; // 생성할 아이템들
    public Transform playerTransform; // 플레이어의 트랜스폼

    public float maxDistance = 5f; // 플레이어 위치로부터 아이템이 배치될 최대 반경

    public float timeBetSpawnMax = 7f; // 최대 시간 간격
    public float timeBetSpawnMin = 2f; // 최소 시간 간격
    private float timeBetSpawn; // 생성 간격

    private float lastSpawnTime; // 마지막 생성 시점

    private void Start() {
        // 생성 간격과 마지막 생성 시점 초기화
        timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
        lastSpawnTime = 0;
    }

    // 주기적으로 아이템 생성 처리 실행
    private void Update() {
        //현재시간과 생성주기 비교, 캐릭터가 있을 때, 
        //아이템 생성 간격이 매번 달라짐
        if (lastSpawnTime + timeBetSpawn < Time.time && playerTransform != null)
        {
            lastSpawnTime = Time.time;
            Spawn();
            timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
        }
        
    }

    // 실제 아이템 생성 처리
    private void Spawn() {
        GameObject spawnedItem = Instantiate(items[Random.Range(0, items.Length)], GetRandomPointOnNavMesh(playerTransform.position, maxDistance) + (Vector3.up * 0.5f), Quaternion.identity);
        Destroy(spawnedItem, 5f);
    }

    // 내비메시 위의 랜덤한 위치를 반환하는 메서드
    // center를 중심으로 distance 반경 안에서 랜덤한 위치를 찾는 함수를 만들거
    private Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance) {        
        //insideUnitSphere 'or' insideUnitCircle 은 구 'or' 원 안에 랜덤한 좌표를 가져온다 
        Vector3 randPos = Random.insideUnitSphere * distance + center;
        // 맵구석에 플레이어가 있다면 아이템이 맵 밖에 나올수도 있음 그래서 내비메시위에만 생기도록 할거임
        //SamplePosition 이라는 함수가 있다. bool값을 반환함?
        NavMeshHit hit;
        NavMesh.SamplePosition(randPos, out hit, distance, NavMesh.AllAreas);

        return hit.position;
    }
}