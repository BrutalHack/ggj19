using System.Collections;
using BrutalHack.ggj19.Music;
using UnityEngine;
using UnityEngine.Experimental.Input;
using UnityEngine.Serialization;

namespace BrutalHack.ggj19.General
{
    public class PlayerManager : MonoBehaviour
    {
        public GameGraphGenerator GameGraphGenerator;
        public GameObject PlayerMarker;
        public Node playerPosition;
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

        void Start()
        {
            StartCoroutine(PlacePlayer());
            musicController.OnBass += OnBass;
            musicController.OnSnare += OnSnare;
        }

        private void OnSnare(TimedNote note)
        {
            snareTimer = inputTimeout;
            verticalMovementEnabled = true;
            ActivateLine(DirectionEnum.North, ActivateVertical);
            ActivateLine(DirectionEnum.South, ActivateVertical);
        }

        private void OnBass(TimedNote note)
        {
            bassTimer = inputTimeout;
            horizontalMovementEnabled = true;
            
            ActivateLine(DirectionEnum.East, ActivateHorizontal);
            ActivateLine(DirectionEnum.West, ActivateHorizontal);
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
                if (playerPosition.Neighbours.ContainsKey(DirectionEnum.North))
                {
                    playerPosition = playerPosition.Neighbours[DirectionEnum.North];
                    moved = true;
                }
            }

            if (moveSouth)
            {
                moveSouth = false;
                if (playerPosition.Neighbours.ContainsKey(DirectionEnum.South))
                {
                    playerPosition = playerPosition.Neighbours[DirectionEnum.South];
                    moved = true;
                }
            }

            if (moveWest)
            {
                moveWest = false;
                if (playerPosition.Neighbours.ContainsKey(DirectionEnum.West))
                {
                    playerPosition = playerPosition.Neighbours[DirectionEnum.West];
                    moved = true;
                }
            }

            if (moveEast)
            {
                moveEast = false;
                if (playerPosition.Neighbours.ContainsKey(DirectionEnum.East))
                {
                    playerPosition = playerPosition.Neighbours[DirectionEnum.East];
                    moved = true;
                }
            }

            if (moved)
            {
                moved = false;
                PlayerMarker.transform.position =
                    GameGraphGenerator.nodesToGameObjects[playerPosition].transform.position;
                horizontalMovementEnabled = false;
                verticalMovementEnabled = false;
            }
        }

        private void UpdateInput()
        {
            Keyboard keyboard = InputSystem.GetDevice<Keyboard>();
            if (verticalMovementEnabled)
            {
                if (keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame)
                {
                    MoveNorth();
                }

                if (keyboard.sKey.wasPressedThisFrame || keyboard.downArrowKey.wasPressedThisFrame)
                {
                    MoveSouth();
                }
            }

            if (horizontalMovementEnabled)
            {
                if (keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame)
                {
                    MoveWest();
                }

                if (keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame)
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