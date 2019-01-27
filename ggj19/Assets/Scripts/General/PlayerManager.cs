using System;
using System.Collections;
using BrutalHack.ggj19.General.Music;
using UnityEngine;
using UnityEngine.SceneManagement;
using static BrutalHack.ggj19.General.DirectionEnum;
using Random = UnityEngine.Random;

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

        public AudioClip[] joySounds;
        private AudioSource audioSource;

        private NodeCollectionLogic nodeCollectionLogic;
        
        float oldHorizontal1;
        float oldHorizontal2;
        float oldVertical1;
        float oldVertical2;

        private float inputAxisDeadzone = 0.02f;
        private static readonly int Happy = Animator.StringToHash("happy");

        void Start()
        {
            nodeCollectionLogic = NodeCollectionLogic.Instance;
            StartCoroutine(PlacePlayer());
            musicController.OnBass += OnBass;
            musicController.OnSnare += OnSnare;
            musicController.OnSongFinished += nodeCollectionLogic.CountScore;
            musicController.OnSongFinished += ShowScoreScene;
            audioSource = GetComponent<AudioSource>();
        }

        private void ShowScoreScene()
        {
            SceneManager.LoadScene((int) Scenes.ScoreScreen, LoadSceneMode.Additive);
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
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Keypad7))
            {
                ShowScoreScene();
            }

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
                GameGraphGenerator.nodesToGameObjects[playerPosition].GetComponent<Animator>().SetBool(Happy, true);
                audioSource.clip = joySounds[Random.Range(0, joySounds.Length - 1)];
                audioSource.Play();
                horizontalMovementEnabled = false;
                verticalMovementEnabled = false;

                nodeCollectionLogic.TrackMove(oldPosition, playerPosition);
            }
        }

        private void UpdateInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Keypad6))
            {
                Debug.Log("Application Quit");
                Application.Quit();
            }
            
            float horizontal1 = Input.GetAxis("Horizontal 1");
            float horizontal2 = Input.GetAxis("Horizontal 2");
            float vertical1 = Input.GetAxis("Vertical 1");
            float vertical2 = Input.GetAxis("Vertical 2");

            if (verticalMovementEnabled)
            {
                HandlePlayerVerticalInput(vertical1, oldVertical1);
//                HandlePlayerVerticalInput(vertical2, oldVertical2);
            }

            if (horizontalMovementEnabled)
            {
                HandlePlayerHorizontalInput(horizontal1, oldHorizontal1);
            }

            oldHorizontal1 = horizontal1;
            oldHorizontal2 = horizontal2;
            oldVertical1 = vertical1;
            oldVertical2 = vertical2;
        }

        private void HandlePlayerVerticalInput(float newInput, float oldInput)
        {
            if (Math.Abs(oldInput) < inputAxisDeadzone && Math.Abs(newInput) > inputAxisDeadzone)
            {
                if (newInput < 0f)
                {
                    MoveSouth();
                }
                else
                {
                    MoveNorth();
                }
            }
        }

        private void HandlePlayerHorizontalInput(float newInput, float oldInput)
        {
            if (Math.Abs(oldInput) < inputAxisDeadzone && Math.Abs(newInput) > inputAxisDeadzone)
            {
                if (newInput < 0f)
                {
                    MoveWest();
                }
                else
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