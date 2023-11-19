using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int value;
    public Node node;
    public Block mergingBlock;
    public bool merging;

    public Vector2 pos => transform.position;

    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private TextMeshPro text;

    public void Init(BlockType type)
    {
        value = type.value;
        sr.color = type.color;
        text.text = type.value.ToString();
    }

    public void SetBlock(Node node)
    {
        if (this.node != null) this.node.occupiedBlock = null;
        this.node = node;
        this.node.occupiedBlock = this;
    }

    public void MergeBlock(Block blockToMergeWith)
    {
        // Set the block we are merging with
        mergingBlock = blockToMergeWith;

        // Set current node as unoccupied to allow blocks to use it
        node.occupiedBlock = null;

        // Set the base block as merging, so it does not get used twice.
        blockToMergeWith.merging = true;
    }

    public bool CanMerge(int value) => value == this.value && !merging && mergingBlock == null;
}
