using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Dungeon Settings")]
    public int roomCount = 18;
    public Vector2Int gridSize = new Vector2Int(7, 7);
    public Vector2Int startPosition = new Vector2Int(0, 0);
    public float roomSpacing = 2.5f;
    public GameObject roomPrefab;
    public bool generateOnStart = true;

    [Header("Map Visualization")]
    public Transform roomParent;

    [Header("Resource")]
    public int maxPower = 10;
    public int currentPower;
    public Text powerText;

    [Header("Events")]
    public RoomEventManager eventManager;
    public bool eventInProgress = false;
    public float eventSpawnChance = 0.2f;

    public DungeonRoom currentRoom { get; private set; }
    public List<DungeonRoom> allRooms { get; private set; } = new List<DungeonRoom>();
    private List<RoomEventData> eventPool = new List<RoomEventData>();

    private Dictionary<Vector2Int, DungeonRoom> roomsByPosition = new Dictionary<Vector2Int, DungeonRoom>();

    private class RoomEventData
    {
        public RoomEventType eventType;
        public int powerAmount;

        public RoomEventData(RoomEventType eventType, int powerAmount)
        {
            this.eventType = eventType;
            this.powerAmount = powerAmount;
        }
    }

    private readonly Vector2Int[] directions = new[]
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    private void Start()
    {
        if (generateOnStart)
        {
            GenerateDungeon();
        }
    }

    [ContextMenu("Generate Dungeon")]
    public void GenerateDungeon()
    {
        if (roomPrefab == null)
        {
            Debug.LogError("DungeonGenerator requires a roomPrefab.");
            return;
        }

        EnsurePowerUI();
        ClearDungeon();
        CreateRoomPositions();
        InstantiateRooms();
        BuildNeighbors();
        InitializePower();
        InitializeEventManager();
        InitializeEventPool();
        SelectStartRoom();
        RevealAvailableRooms();
    }

    private void ClearDungeon()
    {
        allRooms.Clear();
        roomsByPosition.Clear();

        if (roomParent != null)
        {
            for (int i = roomParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(roomParent.GetChild(i).gameObject);
            }
        }
    }

    private void InitializePower()
    {
        currentPower = maxPower;
        UpdatePowerDisplay();
    }

    private void InitializeEventPool()
    {
        eventPool.Clear();
        eventPool.Add(new RoomEventData(RoomEventType.PowerGain, 3));
    }

    private void UpdatePowerDisplay()
    {
        if (powerText != null)
        {
            powerText.text = currentPower > 0 ? $"Power: {currentPower}/{maxPower}" : "Game Over";
        }
    }

    private void InitializeEventManager()
    {
        if (eventManager != null) return;
        eventManager = FindObjectOfType<RoomEventManager>();
        if (eventManager == null)
        {
            GameObject managerObject = new GameObject("RoomEventManager");
            eventManager = managerObject.AddComponent<RoomEventManager>();
        }
    }

    private void AssignEventToRoom(DungeonRoom room, bool guarantee)
    {
        if (room == null || room.roomEventType != RoomEventType.None || eventPool.Count == 0) return;

        bool assign = guarantee || Random.value < eventSpawnChance;
        if (!assign) return;

        int index = Random.Range(0, eventPool.Count);
        RoomEventData eventData = eventPool[index];
        room.roomEventType = eventData.eventType;
        room.eventPowerAmount = eventData.powerAmount;
        eventPool.RemoveAt(index);
    }

    private void EnsurePowerUI()
    {
        if (powerText != null) return;

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("DungeonUI_Canvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        if (canvas != null)
        {
            GameObject textObject = new GameObject("PowerText");
            textObject.transform.SetParent(canvas.transform, false);
            Text text = textObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 24;
            text.color = Color.white;
            text.alignment = TextAnchor.UpperLeft;

            RectTransform rectTransform = text.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.anchoredPosition = new Vector2(10, -10);
            rectTransform.sizeDelta = new Vector2(300, 50);

            powerText = text;
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over: Power depleted.");
        currentPower = 0;
        UpdatePowerDisplay();
    }

    private void CreateRoomPositions()
    {
        var positions = new HashSet<Vector2Int> { startPosition };
        var frontier = new List<Vector2Int> { startPosition };
        int maxRooms = Mathf.Min(roomCount, gridSize.x * gridSize.y);

        while (positions.Count < maxRooms && frontier.Count > 0)
        {
            Vector2Int current = frontier[Random.Range(0, frontier.Count)];
            Vector2Int[] shuffled = directions.OrderBy(_ => Random.value).ToArray();

            bool added = false;
            foreach (Vector2Int dir in shuffled)
            {
                Vector2Int next = current + dir;
                if (IsInsideGrid(next) && !positions.Contains(next))
                {
                    positions.Add(next);
                    frontier.Add(next);
                    added = true;
                    break;
                }
            }

            if (!added)
            {
                frontier.Remove(current);
            }
        }

        foreach (Vector2Int position in positions)
        {
            roomsByPosition[position] = null;
        }
    }

    private bool IsInsideGrid(Vector2Int position)
    {
        int halfWidth = gridSize.x / 2;
        int halfHeight = gridSize.y / 2;
        return position.x >= -halfWidth && position.x <= halfWidth
            && position.y >= -halfHeight && position.y <= halfHeight;
    }

    private void InstantiateRooms()
    {
        int id = 0;
        List<Vector2Int> roomPositions = new List<Vector2Int>(roomsByPosition.Keys);

        foreach (Vector2Int position in roomPositions)
        {
            Vector3 worldPosition = new Vector3(position.x * roomSpacing, position.y * roomSpacing, 0f);
            GameObject roomInstance = Instantiate(roomPrefab, worldPosition, Quaternion.identity, roomParent);
            roomInstance.name = $"Room_{id}";
            DungeonRoom room = roomInstance.GetComponent<DungeonRoom>();
            if (room == null)
            {
                room = roomInstance.AddComponent<DungeonRoom>();
            }

            room.roomId = id;
            room.gridPosition = position;
            room.roomType = RoomType.Normal;
            room.isVisited = false;
            room.isAvailable = false;
            room.neighbors = new List<DungeonRoom>();
            allRooms.Add(room);
            roomsByPosition[position] = room;
            id++;
        }
    }

    private void BuildNeighbors()
    {
        foreach (KeyValuePair<Vector2Int, DungeonRoom> pair in roomsByPosition)
        {
            DungeonRoom room = pair.Value;
            foreach (Vector2Int direction in directions)
            {
                Vector2Int neighborPosition = pair.Key + direction;
                if (roomsByPosition.TryGetValue(neighborPosition, out DungeonRoom neighbor) && neighbor != null)
                {
                    if (!room.neighbors.Contains(neighbor))
                    {
                        room.neighbors.Add(neighbor);
                    }
                }
            }
        }

        MarkBossRoom();
    }

    private void MarkBossRoom()
    {
        DungeonRoom bestBoss = null;
        int bestDistance = -1;
        foreach (DungeonRoom room in allRooms)
        {
            int distance = Mathf.Abs(room.gridPosition.x - startPosition.x) + Mathf.Abs(room.gridPosition.y - startPosition.y);
            if (distance > bestDistance)
            {
                bestDistance = distance;
                bestBoss = room;
            }
        }

        if (bestBoss != null && bestBoss.gridPosition != startPosition)
        {
            bestBoss.roomType = RoomType.Boss;
        }
    }

    private void SelectStartRoom()
    {
        if (!roomsByPosition.TryGetValue(startPosition, out DungeonRoom startRoom) || startRoom == null)
        {
            currentRoom = allRooms.Count > 0 ? allRooms[0] : null;
        }
        else
        {
            currentRoom = startRoom;
        }

        if (currentRoom != null)
        {
            currentRoom.roomType = RoomType.Basement;
            currentRoom.isVisited = true;
            currentRoom.isRevealed = true;
            currentRoom.isAvailable = false;
            UpdateCameraToCurrentRoom();
        }
    }

    private void UpdateCameraToCurrentRoom()
    {
        if (currentRoom == null) return;

        CameraFollow follow = FindObjectOfType<CameraFollow>();
        if (follow != null)
        {
            follow.target = currentRoom.transform;
            return;
        }

        if (Camera.main != null)
        {
            Vector3 cameraPosition = Camera.main.transform.position;
            cameraPosition.x = currentRoom.transform.position.x;
            cameraPosition.y = currentRoom.transform.position.y;
            Camera.main.transform.position = cameraPosition;
        }
    }

    private void RevealAvailableRooms()
    {
        foreach (DungeonRoom room in allRooms)
        {
            room.isAvailable = currentRoom != null && currentRoom.neighbors.Contains(room) && currentPower > 0;
            room.isRevealed = room.isRevealed || room.isVisited || room.isAvailable || room == currentRoom;
            room.RefreshView(room == currentRoom);
        }
    }

    public void MoveToRoom(DungeonRoom targetRoom)
    {
        if (eventInProgress || targetRoom == null || !targetRoom.isAvailable || currentPower <= 0) return;
        StartCoroutine(MoveToRoomCoroutine(targetRoom));
    }

    private System.Collections.IEnumerator MoveToRoomCoroutine(DungeonRoom targetRoom)
    {
        bool cameFromBasement = currentRoom != null && currentRoom.roomType == RoomType.Basement;

        if (currentRoom != null)
        {
            currentRoom.isVisited = true;
            currentRoom.isAvailable = false;
            currentRoom.RefreshView(false);
        }

        currentRoom = targetRoom;
        currentRoom.isVisited = true;
        currentRoom.isAvailable = false;
        currentRoom.RefreshView(true);

        if (cameFromBasement)
        {
            AssignEventToRoom(currentRoom, true);
        }
        else
        {
            AssignEventToRoom(currentRoom, false);
        }

        currentPower = Mathf.Max(0, currentPower - 1);
        UpdatePowerDisplay();

        if (currentRoom.roomType == RoomType.Basement)
        {
            currentPower = maxPower;
            UpdatePowerDisplay();
            Debug.Log("Basement reached: Power fully restored.");
        }

        if (currentPower <= 0)
        {
            GameOver();
            yield break;
        }

        UpdateCameraToCurrentRoom();
        RevealAvailableRooms();

        if (currentRoom.roomEventType == RoomEventType.PowerGain && !currentRoom.eventCompleted && eventManager != null)
        {
            eventInProgress = true;
            currentRoom.eventCompleted = true;
            yield return eventManager.PlayPowerGainEvent(currentRoom.eventPowerAmount, () =>
            {
                currentPower = Mathf.Min(maxPower, currentPower + currentRoom.eventPowerAmount);
                UpdatePowerDisplay();
            });
            eventInProgress = false;
            UpdateCameraToCurrentRoom();
            RevealAvailableRooms();
        }

        if (currentRoom.roomType == RoomType.Boss)
        {
            Debug.Log("Boss room reached! 次の戦闘へ...");
        }
        else
        {
            Debug.Log($"Moved to room {currentRoom.roomId} at {currentRoom.gridPosition}");
        }
    }
}
