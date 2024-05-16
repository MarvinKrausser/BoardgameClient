using System;
using UnityEngine;

namespace entities
{
    /// <summary>
    /// Class for the projectile behaviour. Snitches get stiches but its always worse to get shot by gandalf
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        private Transform targetTransform;
        private Player target;
        private float speed;

        /// <summary>
        /// Initializes the projectile that set the speed and target.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="speed"></param>
        public void Initialize(Player target, float speed)
        {
            this.targetTransform = target.transform;
            this.target = target;
            this.speed = speed;
        }

        /// <summary>
        /// Lets the projectile fly towards the target until it hits the player.
        /// </summary>
        private void Update()
        {
            if (targetTransform is null || Vector3.Distance(this.transform.position, targetTransform.transform.position) <= 0.1f )
            {
                // Destroy the projectile when reached and plays hit animation 
                Destroy(gameObject);

            }
            else
            {
                // Move the projectile towards the target
                transform.position = Vector2.MoveTowards(transform.position, targetTransform.position, speed * Time.deltaTime);
            }
        }
    }
}