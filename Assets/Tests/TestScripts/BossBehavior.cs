using Spine;
using UnityEngine;

namespace Tests.TestScripts
{
    public class BossBehavior : BaseEnemyBehavior
    {
        [SerializeField] private int hitsUntilDeath = 3;
        [SerializeField] private GameObject enemyToSpawnAfterDeath;
        [SerializeField] private int numberOfEnemiesToSpawn = 3;
        [SerializeField] private int enemiesSpawnForce = 3;

        private int _hitsReceived;
        private Vector3 _deathPoint;
        
        private void Start()
        {
            base.Start();
            _hitsReceived = 0;
        }
        
        public override void ReceiveAttack(float attackDamage, Vector3 attackDirection, float attackForce)
        {
            if (!_isDead)
            {
                ++_hitsReceived;
                if (hitsUntilDeath <= _hitsReceived)
                {
                    ChangeState(EnemyStateEnum.Death);
                    _deathPoint = transform.position;
                    SpawnSmallEnemies();
                }
                else
                {
                    ChangeState(EnemyStateEnum.ReceivingHit);
                }
            }
        }

        private void SpawnSmallEnemies()
        {
            for (int i = 0; i < numberOfEnemiesToSpawn; i++)
            {
                var newEnemy = Instantiate(enemyToSpawnAfterDeath,
                    _deathPoint + new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f)), Quaternion.identity);
                newEnemy.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f)) * enemiesSpawnForce);
            }
        }
    }
}