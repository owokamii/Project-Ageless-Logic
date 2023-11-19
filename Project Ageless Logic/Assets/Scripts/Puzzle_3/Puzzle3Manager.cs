using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Puzzle3Manager : MonoBehaviour
{
    [SerializeField] private int width = 4;
    [SerializeField] private int height = 4;

    [SerializeField] private Node nodePrefab;
    [SerializeField] private Block blockPrefab;
    [SerializeField] private SpriteRenderer boardPrefab;

    [SerializeField] private float travelTime = 0.2f;

    [SerializeField] private int winCondition = 2048;

    [SerializeField] private GameObject winScreen, loseScreen;

    [SerializeField] private List<BlockType> types = new List<BlockType>();

    private List<Node> nodes = new List<Node>();
    private List<Block> blocks = new List<Block>();

    private BlockType getBlockTypeByValue(int value) => types.First(t => t.value == value);

    private GameState gameState;

    private int round;

    void Start()
    {
        ChangeState(GameState.GenerateLevel);
    }

    private void ChangeState(GameState newState)
    {
        gameState = newState;

        switch (gameState)
        {
            case GameState.GenerateLevel:
                GenerateGrid();
                break;
            case GameState.SpawningBlocks:
                SpawnBlocks(round++ == 0 ? 2 : 1);
                break;
            case GameState.WaitingInput:
                break;
            case GameState.Moving:
                break;
            case GameState.Win:
                winScreen.SetActive(true);
                //Invoke(nameof(DelayedWinScreenText), 1.5f);
                break;
            case GameState.Lose:
                loseScreen.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    private void Update()
    {
        if (gameState != GameState.WaitingInput) 
        { 
            return; 
        }

        if (SwipeManager.swipeUp || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            ShiftBlocks(Vector2.up);
        }
        if (SwipeManager.swipeDown || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            ShiftBlocks(Vector2.down);
        }
        if (SwipeManager.swipeLeft || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ShiftBlocks(Vector2.left);
        }
        if (SwipeManager.swipeRight || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            ShiftBlocks(Vector2.right);
        }
    }

    void GenerateGrid()
    {
        round = 0;

        for (int x = 0; x < width; x++) 
        {
            for (int y = 0; y < width; y++)
            {
                var node = Instantiate(nodePrefab, new Vector2(x, y), Quaternion.identity);
                
                nodes.Add(node);
            }
        }

        var center = new Vector2((float) width / 2 - 0.5f, (float) height / 2 -0.5f);

        var board = Instantiate(boardPrefab, center, Quaternion.identity);
        board.size = new Vector2(width, height);

        Camera.main.transform.position = new Vector3(center.x, center.y, -10);

        ChangeState(GameState.SpawningBlocks);
    }

    void SpawnBlocks(int amount)
    {
        var freeNodes = nodes.Where(n => n.occupiedBlock == null).OrderBy(b => Random.value);

        foreach (var node in freeNodes.Take(amount))
        {
            SpawnBlock(node, Random.value > 0.8 ? 4 : 2);
        }

        if (freeNodes.Count() == 1)
        {
            Debug.Log("Lost Game");

            ChangeState(GameState.Lose);

            return;
        }

        ChangeState(blocks.Any(b => b.value == winCondition) ? GameState.Win : GameState.WaitingInput);
    }

    void SpawnBlock(Node node, int value)
    {
        var block = Instantiate(blockPrefab, node.pos, Quaternion.identity);
        block.Init(getBlockTypeByValue(value));
        block.SetBlock(node);
        blocks.Add(block);
    }

    void ShiftBlocks(Vector2 dir)
    {
        ChangeState(GameState.Moving);

        var orderedBlocks = blocks.OrderBy(b => b.pos.x).ThenBy(b => b.pos.y).ToList();

        if (dir == Vector2.right || dir == Vector2.up)
        {
            orderedBlocks.Reverse();
        }

        foreach (var block in orderedBlocks)
        {
            var next = block.node; 

            do
            {
                block.SetBlock(next);

                var possibleNode = GetNodeAtPos(next.pos + dir);
                if ( possibleNode != null)
                {
                    // If can merge, merge
                    if (possibleNode.occupiedBlock != null && possibleNode.occupiedBlock.CanMerge(block.value))
                    {
                        block.mergingBlock = possibleNode.occupiedBlock;

                    }
                    // Else, can we move here
                    else if ( possibleNode.occupiedBlock == null)
                    {
                        next = possibleNode;
                    }
                }
            } while (next != block.node);
        }

        var sequence = DOTween.Sequence();

        foreach (var block in orderedBlocks)
        {
            var movePoint = block.mergingBlock != null ? block.mergingBlock.node.pos : block.node.pos;

            sequence.Insert(0, block.transform.DOMove(movePoint, travelTime));
        }

        sequence.OnComplete(() =>
        {
            foreach (var block in orderedBlocks. Where(b => b.mergingBlock != null))
            {
                MergeBlocks(block.mergingBlock, block);
            }

            ChangeState(GameState.SpawningBlocks);
        });
    }

    void MergeBlocks(Block baseBlock, Block mergingBlock)
    {
        SpawnBlock(baseBlock.node, baseBlock.value * 2);

        RemoveBlock(baseBlock);
        RemoveBlock(mergingBlock);
    }

    void RemoveBlock(Block block)
    {
        blocks.Remove(block);
        Destroy(block.gameObject);
    }

    Node GetNodeAtPos(Vector2 pos)
    {
        return nodes.FirstOrDefault(n => n.pos == pos);
    }
}

[Serializable]
public struct BlockType
{
    public int value;
    public Color color;
}

public enum GameState
{
    GenerateLevel,
    SpawningBlocks,
    WaitingInput,
    Moving,
    Win,
    Lose
}