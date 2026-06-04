using System.Collections.Generic;
using UnityEngine;

public enum RoomType { Start, Basement, Normal, Boss }
public enum RoomEventType { None, PowerGain }

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class DungeonRoom : MonoBehaviour
{
    public int roomId;
    public Vector2Int gridPosition;
    public RoomType roomType = RoomType.Normal;
    public RoomEventType roomEventType = RoomEventType.None;
    public int eventPowerAmount = 3;
    public bool eventCompleted = false;
    public List<DungeonRoom> neighbors = new List<DungeonRoom>();
    public bool isVisited = false;
    public bool isAvailable = false;
    public bool isRevealed = false;

    private SpriteRenderer spriteRenderer;
    private Collider2D roomCollider;

    private static readonly Color startColor = new Color(0.4f, 0.9f, 0.4f);
    private static readonly Color basementColor = new Color(0.3f, 0.6f, 1f);
    private static readonly Color normalColor = new Color(0.9f, 0.9f, 0.9f);
    private static readonly Color availableColor = new Color(1f, 0.85f, 0.4f);
    private static readonly Color visitedColor = new Color(0.5f, 0.5f, 0.5f);
    private static readonly Color bossColor = new Color(0.9f, 0.3f, 0.3f);

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        roomCollider = GetComponent<Collider2D>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning($"{name} requires a SpriteRenderer component.");
        }
        if (roomCollider == null)
        {
            Debug.LogWarning($"{name} requires a Collider2D component.");
        }
    }

    private void OnMouseDown()
    {
        if (!isAvailable) return;

        DungeonGenerator generator = FindObjectOfType<DungeonGenerator>();
        if (generator != null && generator.eventInProgress) return;

        if (generator != null)
        {
            generator.MoveToRoom(this);
        }
    }

    public void RefreshView(bool isCurrent)
    {
        if (spriteRenderer == null) return;

        bool visible = isCurrent || isRevealed;
        spriteRenderer.enabled = visible;
        if (roomCollider != null)
        {
            roomCollider.enabled = visible && isAvailable;
        }

        if (!visible)
        {
            return;
        }

        if (roomType == RoomType.Boss)
        {
            spriteRenderer.color = bossColor;
        }
        else if (roomType == RoomType.Basement)
        {
            spriteRenderer.color = basementColor;
        }
        else if (isCurrent)
        {
            spriteRenderer.color = startColor;
        }
        else if (isAvailable)
        {
            spriteRenderer.color = availableColor;
        }
        else if (isVisited)
        {
            spriteRenderer.color = visitedColor;
        }
        else
        {
            spriteRenderer.color = normalColor;
        }
    }
}
