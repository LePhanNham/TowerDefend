using System.Collections;
using UnityEngine;

public class EnemyAnimations : MonoBehaviour
{
    private Animator _animator;
    private Enemy _enemy;
    private EnemyHealth _enemyHealth;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _enemy = GetComponent<Enemy>();
        _enemyHealth = GetComponent<EnemyHealth>();
    }

    public void PlayHurtAnimation()
    {
        _animator.SetTrigger("Hurt");
    }

    private float GetCurrentAnimationLength()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).length;
    }

    private IEnumerator PlayHurt()
    {
        _enemy.StopMovement();
        PlayHurtAnimation();
        yield return new WaitForSeconds(GetCurrentAnimationLength() + 0.1f);
        _enemy.ResumeMovement();
    }

    private void EnemyHit(Enemy enemy)
    {
        if (_enemy == enemy)
        {
            StartCoroutine(PlayHurt());
        }
    }

    private void OnEnable()
    {
        EnemyHealth.OnEnemyHit += EnemyHit;
    }

    private void OnDisable()
    {
        EnemyHealth.OnEnemyHit -= EnemyHit;
    }
}
