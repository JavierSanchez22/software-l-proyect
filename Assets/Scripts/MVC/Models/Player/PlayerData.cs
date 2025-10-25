using UnityEngine;

namespace Assets.Scripts.MVC.Models.Player
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "DinoRunner/Player Data")]
    public class PlayerData : ScriptableObject
    {
        [Header("Estadísticas Base")]
        public float maxHealth = 100f;
        public int initialLives = 3;

        [Header("Movimiento")]
        public float moveSpeed = 6f;
        public float jumpForce = 13f;
        public float groundCheckRadius = 0.2f;
        public LayerMask groundLayer;

        [Header("Combate & Daño")]
        public float invulnerabilityDuration = 1.5f;
        public float knockbackForce = 5f;

        [Header("Física (Opcional - Puede controlarse en Rigidbody)")]
        public float gravityScale = 3f;
        public float maxFallSpeed = 20f; 
    }
}