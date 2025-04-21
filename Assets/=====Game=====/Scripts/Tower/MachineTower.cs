using static UnityEngine.GraphicsBuffer;
using UnityEngine;

public class MachineTower : TowerProjectile
{
    [SerializeField] private bool isDualMachine;
    [SerializeField] private float spreadRange;

    protected override void Update()
    {
        if (Time.time > nextAttackTime)
        {

            if (_tower.currentEnemyTarget != null)
            {
                Vector3 dirToTarget = _tower.currentEnemyTarget.transform.position - transform.position;
                FireProjectile(dirToTarget);

                nextAttackTime = Time.time + delayBtwAttacks;

            }
        }
    }


    protected override void LoadProjectile() { }

    private void FireProjectile(Vector3 direction)
    {
        GameObject instance = _pooler.GetInstanceFromPool();
        instance.transform.position = projectileSpawnPosition.position;

        //MachineProjectile projectile = instance.GetComponent<MachineProjectile>();
        //projectile.Direction = direction;
        //projectile.Damage = Damage;
        //AudioManager.Instance.PlaySound(AudioManager.Instance.);

        if (isDualMachine)
        {
            float randomSpread = Random.Range(-spreadRange, spreadRange);
            Vector3 spread = new Vector3(0f, 0f, randomSpread);
            Quaternion spreadValue = Quaternion.Euler(spread);
            Vector2 newDirection = spreadValue * direction;
            //AudioManager.Instance.PlayerSound(AudioManager.Sound.machineBullet);
        }

    }
  

}