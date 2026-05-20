using UnityEngine;

namespace DungeonCrawler.Combat
{
    /// <summary>
    /// Spawns hit/attack particles and plays SFX via AudioManager.
    /// </summary>
    public class CombatVFX : MonoBehaviour
    {
        [SerializeField] private ParticleSystem hitParticles;
        [SerializeField] private AudioClip hitSfx;

        public void PlayHit(Vector3 position)
        {
            if (hitParticles != null)
            {
                hitParticles.transform.position = position;
                hitParticles.Play();
            }

            if (hitSfx != null && AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(hitSfx);
        }
    }
}
