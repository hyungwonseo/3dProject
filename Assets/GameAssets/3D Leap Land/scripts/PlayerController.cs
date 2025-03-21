using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;  // �� ������ ���� �߰�

#if !TANK
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 2f;       // �ȱ� �ӵ�
    public float jumpForce = 4f;       // ���� ��
    private Rigidbody rb;              // Rigidbody ������Ʈ ����
    private Animator animator;         // Animator ������Ʈ ����
    private bool isGrounded = true;    // �÷��̾ ���� �ִ��� ����
    private bool isDead = false;       // �÷��̾� ��� ���� üũ
    private bool isTrophy = false;       // Ʈ���� ȹ�� üũ

    private string currentAnimation = "Idle";  // ���� ��� ���� �ִϸ��̼� �̸�

    private Vector3 movementInput;     // �Է°� ���� (FixedUpdate���� ���)

    void Start()
    {
        rb = GetComponent<Rigidbody>();       // Rigidbody ������Ʈ ��������
        animator = GetComponentInChildren<Animator>();  // Animator ������Ʈ ��������

        if (rb == null)
        {
            Debug.LogError("Rigidbody ������Ʈ�� �����ϴ�!");
        }

        if (animator == null)
        {
            Debug.LogError("Animator ������Ʈ�� �����ϴ�!");
        }
    }

    void Update()
    {
        if (isDead || isTrophy) return;  // ��� ���¿����� �Է� �� �ִϸ��̼� ������Ʈ �ߴ�

        // �̵� �Է� ó��
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        movementInput = new Vector3(moveX, 0, moveZ).normalized;

        HandleJump(); // ���� ��Ʈ��
        UpdateAnimations(); // �ִϸ��̼� ��Ʈ��
    }

    void FixedUpdate()
    {
        if (isDead || isTrophy) return;  // ��� ���¿����� ���� �̵� �ߴ�

        MovePlayer();
    }

    // �÷��̾� ���� �̵� ó��
    void MovePlayer()
    {
        Vector3 movement = movementInput * moveSpeed;
        Vector3 newPosition = rb.position + (movement * Time.fixedDeltaTime);

        rb.MovePosition(newPosition);

        // �̵� �������� �÷��̾� ȸ��
        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        }
    }

    // ���� ó��
    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;  // ���� �� ���·� ����
            PlayAnimation("Jump");  // ���� �ִϸ��̼� ���
        }
    }

    // �ִϸ��̼� ���� ������Ʈ
    void UpdateAnimations()
    {
        if (!isGrounded) return;  // ���� ���� ���� �ٸ� �ִϸ��̼� ��� �� ��

        if (movementInput.magnitude > 0)
        {
            PlayAnimation("Walk");
        }
        else
        {
            PlayAnimation("Idle");
        }
    }

    // �ִϸ��̼� ��� �Լ�
    void PlayAnimation(string animationName)
    {
        if (currentAnimation == animationName) return;  // �ߺ� ��� ����

        animator.Play(animationName);
        currentAnimation = animationName;
    }

    // �浹 ó�� (Ground �� Dead �±�)
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Ground detected");
            if (!isGrounded)
            {
                isGrounded = true;  // ���� �� ���� ���� ����
                PlayAnimation("Idle");
            }
        }

        if (collision.gameObject.CompareTag("Dead") && !isDead)
        {
            Debug.Log("Dead detected");
            HandleDeath();  // ��� ó�� �Լ� ȣ��
        }

        if (collision.gameObject.CompareTag("Coin"))
        {
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Trophy"))
        {
            HandleTrophy();
            Destroy(collision.gameObject);
        }
    }

    // ��� ó�� �Լ�
    private void HandleDeath()
    {
        isDead = true;
        PlayAnimation("Dead");
        rb.velocity = Vector3.zero;  // ������ ����
        StartCoroutine(RestartSceneAfterDelay(3f));  // 3�� �� �� �����
    }

    private void HandleTrophy()
    {
        isTrophy = true;
        PlayAnimation("Trophy");
        rb.velocity = Vector3.zero;  // ������ ����
        StartCoroutine(RestartSceneAfterDelay(4f));  // 3�� �� �� �����
    }

    // 3�� �� �� ����� �ڷ�ƾ
    private IEnumerator RestartSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
#endif