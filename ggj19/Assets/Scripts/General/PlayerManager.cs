﻿using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Input;

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

        void Start()
        {
            StartCoroutine(PlacePlayer());
        }

        IEnumerator PlacePlayer()
        {
            yield return new WaitForSeconds(0.5f);
            playerPosition = GameGraphGenerator.graphGenerator.nodes[Vector2.zero];
            moved = true;
        }

        void Update()
        {
            Keyboard keyboard = InputSystem.GetDevice<Keyboard>();
            if (keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame)
            {
                MoveNorth();
            }

            if (keyboard.sKey.wasPressedThisFrame || keyboard.downArrowKey.wasPressedThisFrame)
            {
                MoveSouth();
            }

            if (keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame)
            {
                MoveWest();
            }

            if (keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame)
            {
                MoveEast();
            }

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