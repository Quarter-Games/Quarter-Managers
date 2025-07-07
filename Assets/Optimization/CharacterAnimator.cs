using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    public Transform lookTarget;
    private Animator animator;
    private Vector3 lastTargetPosition;
    [SerializeField] float lookThreshold = 0.5f;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetFloat("Speed", Input.GetAxis("Vertical"));
        animator.SetFloat("Direction", Input.GetAxis("Horizontal"));
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (lookTarget == null) return;

        if (Vector3.Distance(lastTargetPosition, lookTarget.position) < lookThreshold) return;

        lastTargetPosition = lookTarget.position;
        animator.SetLookAtWeight(1);
        animator.SetLookAtPosition(lookTarget.position);
    }
}
