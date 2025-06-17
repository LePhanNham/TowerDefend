using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimations : MonoBehaviour
{
    public GameObject deathParticles;
    public Animator _animator;
    public Enemy _enemy;
    public EnemyHealth _enemyHealth;
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

    public float GetCurrentAnimationLength()
    {
        float animationLength = _animator.GetCurrentAnimatorStateInfo(0).length;
        return animationLength;
    }

    private IEnumerator PlayHurt()
    {
        _enemy.StopMovement();
        PlayHurtAnimation();
        yield return new WaitForSeconds(GetCurrentAnimationLength() + 0.1f);
        _enemy.ResumeMovement();
    }

    private IEnumerator PlayDead()
    {
        _enemy.StopMovement();

        // Tạo instance của hiệu ứng hạt
        GameObject particlesInstance = Instantiate(deathParticles, transform.position, Quaternion.identity);
        ParticleSystem ps = particlesInstance.GetComponent<ParticleSystem>();

        float particleDuration = 0f;
        if (ps != null)
        {
            // Lấy tổng thời gian phát của hệ thống hạt (bao gồm cả Start Lifetime nếu không phải Looping)
            particleDuration = ps.main.duration + ps.main.startLifetime.constantMax; 
            if (ps.main.loop) // Nếu particle system đang looping, hãy cảnh báo
            {
                Debug.LogWarning("Death particles prefab is set to loop! Consider unchecking 'Looping' in the Particle System component.");
                particleDuration = 1.0f; // Sử dụng thời gian mặc định để tránh chờ vô hạn
            }
        }
        else
        {
            Debug.LogWarning("Death particles prefab does not have a ParticleSystem component!");
            particleDuration = 1.0f; // Thời gian mặc định nếu không tìm thấy ParticleSystem
        }

        yield return new WaitForSeconds(particleDuration); // Chờ hiệu ứng hạt phát xong

        Destroy(particlesInstance); // Hủy GameObject hiệu ứng hạt sau khi phát xong

        _enemy.gameObject.SetActive(false); // Vô hiệu hóa enemy (trả về pool)
        _enemyHealth.ResetHealth();
    }
    private void EnemyHit(Enemy enemy)
    {
        if (_enemy == enemy)
        {
            StartCoroutine(PlayHurt());
        }
    }

    private void EnemyDead(Enemy enemy)
    {
        if (_enemy == enemy)
        {
            StartCoroutine(PlayDead());
        }
    }
    private void OnEnable()
    {
        EnemyHealth.OnEnemyHit += EnemyHit;
        EnemyHealth.OnEnemyKilled += EnemyDead;
    }

    private void OnDisable()
    {
        EnemyHealth.OnEnemyHit -= EnemyHit;
        EnemyHealth.OnEnemyKilled -= EnemyDead;
    }
}
