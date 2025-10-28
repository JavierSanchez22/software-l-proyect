using UnityEngine;
using RedRunner.Interfaces;

namespace RedRunner.MVC.Controllers.Enemy
{
    public class EyeController : MonoBehaviour
    {
        [SerializeField] private Transform pupilTransform;
        [SerializeField] private float detectionRadius = 8f;
        [SerializeField] private float pupilMoveRadius = 0.5f;
        [SerializeField] private float followSpeed = 5f;
        [SerializeField] private float returnSpeed = 2f;
        [SerializeField] private float deadFollowSpeed = 1f;

        private Transform playerTransform;
        private Vector3 eyeCenterPosition;
        private Vector3 targetPupilLocalPosition;
        private IDamageable playerDamageable;

        private void Start()
        {
            if (pupilTransform == null)
            {
                Debug.LogError($"Pupil Transform not assigned to {gameObject.name}");
                enabled = false;
                return;
            }
            eyeCenterPosition = pupilTransform.localPosition;

            GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null)
            {
                playerTransform = playerGO.transform;
                playerDamageable = playerGO.GetComponent<IDamageable>();
            }
        }

        private void Update()
        {
            Vector3 targetDirection = Vector3.zero;
            float currentSpeed = returnSpeed;

            if (playerTransform != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

                if (distanceToPlayer <= detectionRadius)
                {
                    targetDirection = (playerTransform.position - transform.position).normalized;
                    currentSpeed = (playerDamageable != null && !playerDamageable.IsAlive()) ? deadFollowSpeed : followSpeed;
                }
            }

            targetPupilLocalPosition = eyeCenterPosition + Vector3.ClampMagnitude(targetDirection, pupilMoveRadius);
            pupilTransform.localPosition = Vector3.Lerp(pupilTransform.localPosition, targetPupilLocalPosition, currentSpeed * Time.deltaTime);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            if (pupilTransform != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position + eyeCenterPosition, pupilMoveRadius);
            }
        }
    }
}