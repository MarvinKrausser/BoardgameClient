using System;
using System.Collections;
using communication;
using UnityEngine;

namespace entities
{
    /// <summary>
    /// This class is representative for each player on the gameboard. It sets the sprites, handles the animation, movement and current stats.
    /// </summary>
    public class Player : MonoBehaviour
    {
        private float _speed = 3f;
        private Coroutine _moveCoroutine;

        public int Lifes { get; set; }
        public int Lembas { get; set; }
        [SerializeField] public Vector2Int spawnPosition;
        [SerializeField] public GameObject projectilePrefab;

        private Vector2Int _currPosition;

        public int Height;

        /// <summary>
        /// When the current position is changed a coroutine for animating the player to the new position is initialized
        /// </summary>
        public Vector2Int currPosition
        {
            get { return _currPosition; }
            set
            {
                _currPosition = value;

                // If a movement coroutine is already running, stop it.
                if (_moveCoroutine != null)
                {
                    StopCoroutine(_moveCoroutine);
                }

                // Start a new movement coroutine if alive and not at creation Position
                if (!isDead || (_currPosition.x != -1 && _currPosition.y != -1))
                {
                    _moveCoroutine =
                        StartCoroutine(MoveToPosition(new Vector3(_currPosition.x + 0.5f,
                            Height - _currPosition.y-1)));
                }
                else
                {
                    transform.position = new Vector3(_currPosition.x, _currPosition.y);
                }
            }
        }

        public Direction direction { get; set; }
        public int reachedCheckpoints { get; set; }
        public int turnOrder { get; set; }
        public bool isDead { get; set; }
        public bool getsKilled { get; set; }

        [Header("Player sprites")]

        //all the needed sprites
        public CharacterObject Character;
        public SpriteRenderer charakterSprite;
        public Animator animator;

        //variables for death animation 
        private float fadeTime = 1f;
        private float moveUpSpeed = 1f;
        private float moveUpDistance = 0.5f;

        private float PROJECTILE_SPEED = 5f;
        private static readonly int Up = Animator.StringToHash("up");

        //for all roles: 
        public Role role { get; set; }
        public bool ready { get; set; }
        public string name { get; set; }
        public int suspended { get; set; }

        /// <summary>
        /// This initializes a player that does not need to be visually displayed, like all spectators
        /// </summary>
        /// <param name="name"></param>
        /// <param name="role"></param>
        public void Initialize(string name, Role role)
        {
            this.name = name;
            this.role = role;
            ready = false;
            suspended = 0;
            reachedCheckpoints = 0;
            isDead = false;

        }

        /// <summary>
        /// This initializes the players that need to be visually displayed, like players and AIs.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="role"></param>
        /// <param name="lembas"></param>
        public void Initialize(string name, Role role, int lembas)
        {
            this.name = name;
            this.role = role;
            ready = false;
            suspended = 0;
            reachedCheckpoints = 0;
            charakterSprite = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();



            this.Lembas = lembas;
            this.Lifes = 3;
            this.currPosition = new Vector2Int(-1, -1);
            this.spawnPosition = new Vector2Int(-1, -1);
            this.reachedCheckpoints = 0;
            this.turnOrder = 0;
            this.direction = Direction.EAST;

        }


        /// <summary>
        /// Move the player to a new Position. This triggers the SET function for the currPosition variable. 
        /// </summary>
        /// <param name="newPosition"></param>
        public void MovePlayer(Vector2Int newPosition)
        {
            currPosition = newPosition;
        }

        /// <summary>
        /// Set the new direction of a player. 
        /// </summary>
        /// <param name="newDirection"></param>
        public void TurnPlayer(Direction newDirection)
        {
            direction = newDirection;
            setSpriteOnDirection();
        }

        /// <summary>
        /// Calls the triggers for the animator, which sets the correct sprite based on the current direction of the player.
        /// </summary>
        private void setSpriteOnDirection()
        {
            switch (direction)
            {
                case Direction.NORTH:
                    animator.SetTrigger("turnup");
                    break;
                case Direction.EAST:
                    animator.SetTrigger("turnright");
                    break;
                case Direction.SOUTH:
                    animator.SetTrigger("turndown");                
                    break;
                case Direction.WEST:
                    animator.SetTrigger("turnleft");                
                    break;
            }
        
        }
    
        /// <summary>
        /// Triggers the walking animation for the current direction of the player.
        /// Moves the player on the gameboard to the newly set position.
        /// If the player has died in this round its death animation gets triggered and the necessary values are set.
        /// </summary>
        /// <param name="newPosition"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private IEnumerator MoveToPosition(Vector3 newPosition)
        {
            //the animator uses triggers to set the states of the animation (aragornController)
            switch (direction)
            {
                case Direction.NORTH:
                    animator.ResetTrigger("stop");
                    animator.SetTrigger("up");
                    break;
                case Direction.EAST:
                    animator.ResetTrigger("stop");
                    animator.SetTrigger("right");
                    break;
                case Direction.SOUTH:
                    animator.ResetTrigger("stop");
                    animator.SetTrigger("down");
                    break;
                case Direction.WEST:
                    animator.ResetTrigger("stop");
                    animator.SetTrigger("left");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            while (Vector3.Distance(transform.position, newPosition) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, newPosition, _speed * Time.deltaTime);
                yield return null;
            }

            transform.position = newPosition;
        
            if (getsKilled)
            {
                //killPlayer();
                Debug.Log("Death animation trigger has been set");
                animator.SetTrigger("hasDied");
                isDead = true;
                animator.SetBool("dead", true);
                getsKilled = false;
            }
            else
            {
                animator.ResetTrigger("up");
                animator.ResetTrigger("right");
                animator.ResetTrigger("down");
                animator.ResetTrigger("left");
            
                animator.SetTrigger("stop");
            }

        
        }

        /// <summary>
        /// Triggers the shoot animation for the Player. Instantiates a projectile and sets the correct sprite corresponding to the current character.
        /// </summary>
        /// <param name="target"></param>
        public void shoot(Player target)
        {
            //todo: execute animation for player shooting
            //initialize projectile with correct settings and sprite
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            SpriteRenderer sr = projectile.GetComponent<SpriteRenderer>();
            sr.sprite = Character.weapon; 

            Projectile proj = projectile.AddComponent<Projectile>();

            proj.Initialize(target, PROJECTILE_SPEED);

        }

        /// <summary>
        /// Sets necessary values for the animator to display the character again.
        /// </summary>
        public void revivePlayer()
        {
            isDead = false;
            animator.SetBool("dead",false);
            //make the player visible again
            Color currentColor = charakterSprite.color;
            currentColor.a = 1f;
            charakterSprite.color = currentColor;
        }

        public void teleportPlayer(Vector3 newPosition)
        {
            var adjVector = new Vector3(newPosition.x +0.5f,Height - newPosition.y-1);
            transform.position = adjVector;
            _currPosition = new Vector2Int((int)newPosition.x,(int)newPosition.y);
        }
    }
}