using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AI : MonoBehaviour
{
    [Header("战斗属性")]
    public float maxHealth = 100f;
    public float attackRange = 5f; // 攻击距离
    public float currentHealth;

    private Animator animator;
    private Transform playerTransform; // 玩家的位置

    // --- 动画参数哈希值 (优化性能) ---
    private int isAttackingHash;
    private int isDeadHash;
    private int triggerHitHash;
    private int triggerDeadHash;
    [Header("移动和重力")]
    public float gravity; 
    private Vector3 velocity; // 存储当前的垂直速度
    private CharacterController cc;
    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        currentHealth = maxHealth;

        // 预计算参数哈希值
        isAttackingHash = Animator.StringToHash("IsAttacking");
        isDeadHash = Animator.StringToHash("IsDead");
        triggerHitHash = Animator.StringToHash("TriggerHit");
        triggerDeadHash = Animator.StringToHash("triggerDeadHash");
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            HandleDeath();
            return;
        }

        // 2. 距离检查
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= attackRange)
            {
                // 进入 Attack 状态 (如果当前不是 GetHit 状态)
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("GetHit"))
                {
                    animator.SetBool(isAttackingHash, true);
                }
            }
            else
            {
                // 否则循环播放 Idle 动画
                animator.SetBool(isAttackingHash, false);
            }
        }
        if (cc != null)
        {
            if (velocity.y == 0)
            {
                velocity.y = -5f;
            }

            if (cc.isGrounded)
            {
                if (velocity.y < 0)
                {
                    velocity.y = -2f;
                }
            }
            else // 不在地面时，才应用重力
            {
                velocity.y -= gravity * Time.deltaTime;
            }

            // 3. 移动 CharacterController 以应用速度（包括重力）
            cc.Move(velocity * Time.deltaTime);
        }
    }

    // 外部调用函数 (例如玩家的攻击脚本调用这个函数)
    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return; // 已死亡，不再处理伤害

        currentHealth -= damage;
        Debug.Log(gameObject.name + " 受到伤害，当前生命值: " + currentHealth);

        if (currentHealth <= 0)
        {
            // 在 Update 中处理死亡
            return;
        }

        // 播放 GetHit 动画
        // 触发器会覆盖当前的 IsAttacking 状态
        animator.SetTrigger(triggerHitHash);

        // 可选：为了防止立即从 GetHit 跳回 Attack，可以暂时关闭 IsAttacking
        // animator.SetBool(isAttackingHash, false);
    }

    // 处理死亡逻辑
    private void HandleDeath()
    {
        // 确保只设置一次 IsDead
        if (!animator.GetBool(isDeadHash))
        {
            animator.SetBool(isDeadHash, true);
            animator.SetTrigger(triggerDeadHash);//死亡必须使用trigger方法不可使用bool方法，否则death会反复进入any state，然后重复播放死亡的前几帧动画。
            Debug.Log(gameObject.name + " 已死亡。");

            // 可选：在这里添加死亡后的清理逻辑，如销毁对象、掉落物品等
            //Destroy(gameObject, 5f); // 5秒后销毁

            // 禁用 Update 循环，避免继续执行
            this.enabled = false;
        }
    }
}