using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_controller : MonoBehaviour
{
    private CharacterController cc;
    public float moveSpeed;
    public float jumpSpeed;
    private float horizontalMove, verticalMove;
    private Vector3 dir;
    public float gravity;
    private Vector3 velocity;
    [Header("攻击设置")]
    public float attackRange;   // 攻击的射程（Raycast/射线检测的距离）
    public float attackDamage; // 每次攻击造成的伤害
    public LayerMask enemyLayer;      // 敌人所在的层级（用于Raycast过滤）
    private void Start()
    {
        cc = GetComponent<CharacterController>();
        // 锁定鼠标光标，提供更好的FPS体验
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        horizontalMove = Input.GetAxis("Horizontal") * moveSpeed;
        verticalMove = Input.GetAxis("Vertical") * moveSpeed;

        dir = transform.forward * verticalMove + transform.right * horizontalMove;
        cc.Move(dir * Time.deltaTime);
        if (Input.GetButtonDown("Jump"))
        {
            velocity.y = jumpSpeed;
        }
        velocity.y -= gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
        if (Input.GetButtonDown("Fire1")) // Fire1 默认是鼠标左键
        {
            Attack();
        }
        // -------------------------
    }
        void Attack()
        {
            // 使用 Raycast (射线投射) 从摄像机/玩家的视线方向进行攻击检测
            // 假设摄像机是玩家的第一个子物体，或者使用一个专门的 Raycast 起始点
            Transform cameraTransform = transform.GetComponentInChildren<Camera>().transform;

            RaycastHit hit;

            // Raycast(起点, 方向, out 命中的信息, 射程, 目标层级)
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, attackRange, enemyLayer))
            {
                Debug.Log("击中了: " + hit.collider.name);

                // 尝试获取敌人身上的 Enemy_AI 脚本
                Enemy_AI enemy = hit.collider.GetComponent<Enemy_AI>();

                if (enemy != null)
                {
                    // 调用敌人的 TakeDamage 方法造成伤害
                    enemy.TakeDamage(attackDamage);
                }
            }
        }
}
