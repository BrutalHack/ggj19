using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BrutalHack.ggj19.General.Music;
using UnityEngine;
using static BrutalHack.ggj19.General.DirectionEnum;

namespace BrutalHack.ggj19.General
{
    public class PlayerManager : MonoBehaviour
    {
        public GameGraphGenerator GameGraphGenerator;
        public GameObject PlayerMarker;
        public Node playerPosition;
        private Node desiredPlayerPosition;
        public bool moved = false;
        public bool moveNorth = false;
        public bool moveSouth = false;
        public bool moveWest = false;
        public bool moveEast = false;

        public MusicController musicController;
        private bool horizontalMovementEnabled;
        private bool verticalMovementEnabled;

        public float inputTimeout = 0.3f;

        private float snareTimer;
        private float bassTimer;
        private static readonly int ActivateVertical = Animator.StringToHash("activateVertical");
        private static readonly int ActivateHorizontal = Animator.StringToHash("activateHorizontal");
        private static readonly int Jump = Animator.StringToHash("jump");
        private NodeCollectionLogic nodeCollectionLogic;
        
        public GameObject polygonColliderPrefab;

        public AudioClip[] joySounds;
        private AudioSource audioSource;

        void Start()
        {
            nodeCollectionLogic = NodeCollectionLogic.Instance;
            StartCoroutine(PlacePlayer());
            musicController.OnBass += OnBass;
            musicController.OnSnare += OnSnare;
            audioSource = GetComponent<AudioSource>();
        }

        private void OnSnare(TimedNote note)
        {
            snareTimer = inputTimeout;
            verticalMovementEnabled = true;
            ActivateLine(North, ActivateVertical);
            ActivateLine(South, ActivateVertical);
        }

        private void OnBass(TimedNote note)
        {
            bassTimer = inputTimeout;
            horizontalMovementEnabled = true;

            ActivateLine(East, ActivateHorizontal);
            ActivateLine(West, ActivateHorizontal);
        }

        private void ActivateLine(DirectionEnum direction, int trigger)
        {
            if (playerPosition?.Neighbours == null || !playerPosition.Neighbours.ContainsKey(direction))
            {
                return;
            }

            GameObject lineObject =
                GameGraphGenerator.GetLineBetweenNodesIfExists(playerPosition,
                    playerPosition.Neighbours[direction]);
            if (lineObject != null)
            {
                lineObject.GetComponent<Animator>().SetTrigger(trigger);
            }
        }

        IEnumerator PlacePlayer()
        {
            yield return new WaitForSeconds(0.5f);
            playerPosition = GameGraphGenerator.graphGenerator.nodes[Vector2.zero];
            desiredPlayerPosition = GameGraphGenerator.graphGenerator.nodes[Vector2.zero];
            moved = true;
        }

        void Update()
        {
            UpdateSnareTimer();
            UpdateBassTimer();
            UpdateInput();
            UpdateMovement();
        }

        private void UpdateSnareTimer()
        {
            if (snareTimer > 0f)
            {
                snareTimer -= Time.deltaTime;
            }
            else
            {
                verticalMovementEnabled = false;
            }
        }

        private void UpdateBassTimer()
        {
            if (bassTimer > 0f)
            {
                bassTimer -= Time.deltaTime;
            }
            else
            {
                horizontalMovementEnabled = false;
            }
        }

        private void UpdateMovement()
        {
            if (playerPosition == null)
            {
                return;
            }

            if (moveNorth)
            {
                moveNorth = false;
                if (playerPosition.Neighbours.ContainsKey(North))
                {
                    desiredPlayerPosition = playerPosition.Neighbours[North];
                    moved = true;
                }
            }

            if (moveSouth)
            {
                moveSouth = false;
                if (playerPosition.Neighbours.ContainsKey(South))
                {
                    desiredPlayerPosition = playerPosition.Neighbours[South];
                    moved = true;
                }
            }

            if (moveWest)
            {
                moveWest = false;
                if (playerPosition.Neighbours.ContainsKey(West))
                {
                    desiredPlayerPosition = playerPosition.Neighbours[West];
                    moved = true;
                }
            }

            if (moveEast)
            {
                moveEast = false;
                if (playerPosition.Neighbours.ContainsKey(East))
                {
                    desiredPlayerPosition = playerPosition.Neighbours[East];
                    moved = true;
                }
            }

            if (moved)
            {
                moved = false;
                Node oldPosition = playerPosition;
                playerPosition = desiredPlayerPosition;
                PlayerMarker.transform.position =
                    GameGraphGenerator.nodesToGameObjects[playerPosition].transform.position;
                GameGraphGenerator.nodesToGameObjects[playerPosition].GetComponent<Animator>().SetTrigger(Jump);
                audioSource.clip = joySounds[Random.Range(0, joySounds.Length - 1)];
                audioSource.Play();
                horizontalMovementEnabled = false;
                verticalMovementEnabled = false;

                List<Node> path = nodeCollectionLogic.TrackAndHandleMove(oldPosition, playerPosition);
                if (path != null && path.Count > 0)
                {
                    FillCycle(path);
                }
            }
        }

        public void FillCycle(List<Node> cycleNodes)
        {
            var colliderObject = Instantiate(polygonColliderPrefab);
            PolygonCollider2D polygonCollider2D = colliderObject.GetComponent<PolygonCollider2D>();
            Vector3[] cycleCoordinates =
                cycleNodes.Select(node => GameGraphGenerator.nodesToGameObjects[node].transform.position).ToArray();
            polygonCollider2D.points = cycleCoordinates.Select(vector3 => new Vector2(vector3.x, vector3.y)).ToArray();
            colliderObject.GetComponent<MarkGraphElements>().outerNodeCycle = cycleNodes;
        }
        
        private void UpdateInput()
        {
            float horizontal1 = Input.GetAxis("Horizontal 1");
            float horizontal2 = Input.GetAxis("Horizontal 2");
            float vertical1 = Input.GetAxis("Vertical 1");
            float vertical2 = Input.GetAxis("Vertical 2");

            if (verticalMovementEnabled)
            {
                if (vertical1  > 0f)
                {
                    MoveNorth();
                }

                if (vertical1 < 0f)
                {
                    MoveSouth();
                }
            }

            if (horizontalMovementEnabled)
            {
                if (horizontal1 < 0f)
                {
                    MoveWest();
                }

                if (horizontal1 > 0f)
                {
                    MoveEast();
                }
            }
        }

        private void MoveNorth()
        {
            moveNorth = true;
        }

        private void MoveSouth()
        {
            moveSouth = true;
        }

        private void MoveWest()
        {
            moveWest = true;
        }

        private void MoveEast()
        {
            moveEast = true;
        }
    }
}