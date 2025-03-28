using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IsometricMapGenerator : BaseSingleton<IsometricMapGenerator>
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject waterPrefab;
    [SerializeField] private int mapWidth = 10;
    [SerializeField] private int mapHeight = 10;
    [SerializeField] private float tileWidth = 1f;
    [SerializeField] private float tileHeight = 0.5f;
    [SerializeField] private float waterClusterSize = 3;
    [SerializeField] private int seed = 0;
    private List<SlotController> slotControllers = new List<SlotController>();
    public Transform centerTransform;

    public bool isPaint = false;
    public bool isLine = false;
    private bool[,] waterMap;
    public void CreateMap(Transform centerTransform, List<SlotController> slotControllers)
    {
        //slotControllers = GetComponentsInChildren<SlotController>().ToList();
        if (seed == 0)
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
        }
        Random.InitState(seed);
        this.slotControllers = new List<SlotController>(slotControllers);
        this.centerTransform = centerTransform;
    }

    public void GenerateWaterClusters()
    {
        waterMap = new bool[mapWidth, mapHeight];
        int waterClusters = Random.Range(1, (mapWidth * mapHeight) / (int)(waterClusterSize * waterClusterSize));

        for (int i = 0; i < waterClusters; i++)
        {
            int startX = Random.Range(0, mapWidth);
            int startY = Random.Range(0, mapHeight);

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int nx = startX + x;
                    int ny = startY + y;
                    if (nx >= 0 && nx < mapWidth && ny >= 0 && ny < mapHeight)
                    {
                        waterMap[nx, ny] = true;
                    }
                }
            }
        }
    }

    public void SmoothWaterClusters()
    {
        bool[,] newWaterMap = new bool[mapWidth, mapHeight];

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                int waterNeighbors = CountWaterNeighbors(x, y);

                if (waterMap[x, y])
                {
                    newWaterMap[x, y] = waterNeighbors >= 2;
                }
                else
                {
                    newWaterMap[x, y] = waterNeighbors >= 4;
                }
            }
        }

        waterMap = newWaterMap;
    }

    public int CountWaterNeighbors(int x, int y)
    {
        int count = 0;
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                int nx = x + dx;
                int ny = y + dy;
                if (nx >= 0 && nx < mapWidth && ny >= 0 && ny < mapHeight && waterMap[nx, ny])
                {
                    count++;
                }
            }
        }
        return count;
    }

    private void GenerateMap()
    {
        Vector3 centerPos = centerTransform != null ? centerTransform.position : Vector3.zero;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3 isoPosition = GridToIsometric(x - mapWidth / 2, y - mapHeight / 2) + centerPos;
                isoPosition.z = isoPosition.y * 0.001f;
                GameObject prefabToSpawn = waterMap[x, y] ? waterPrefab : tilePrefab;
                var newItem = PoolManager.Instance.Get(prefabToSpawn, isoPosition);
                newItem.transform.parent = this.centerTransform;

            }
        }
    }
    public IEnumerator GenerateMapCoroutine()
    {
        Vector3 centerPos = centerTransform != null ? centerTransform.position : Vector3.zero;
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3 isoPosition = GridToIsometric(x - mapWidth / 2, y - mapHeight / 2) + centerPos;
                isoPosition.z = isoPosition.y * 0.001f;

                if (IsPositionOccupied(isoPosition)) continue;

                GameObject prefabToSpawn = waterMap[x, y] ? waterPrefab : tilePrefab;
                var newItem = PoolManager.Instance.Get(prefabToSpawn, isoPosition);
                newItem.transform.parent = this.centerTransform;

                if ((x * mapHeight + y) % 10 == 0)
                    yield return new WaitForSeconds(.01f);
            }
        }
    }
    private bool IsPositionOccupied(Vector3 position)
    {
        foreach (var slot in slotControllers)
        {
            if (Mathf.Approximately(slot.transform.position.x, position.x) &&
                Mathf.Approximately(slot.transform.position.y, position.y))
            {
                return true;
            }
        }
        return false;
    }

    Vector3 GridToIsometric(int x, int y)
    {
        float isoX = (x - y) * tileWidth / 2;
        float isoY = (x + y) * tileHeight / 2;
        return new Vector3(isoX, isoY, 0);
    }
    private void OnDrawGizmos()
    {
        if (!isLine) return;
        if (waterMap == null || centerTransform == null)
        {
            if (slotControllers == null || slotControllers.Count() == 0) {
                slotControllers = GetComponentsInChildren<SlotController>().ToList();
            }
            CreateMap(centerTransform,slotControllers);
            GenerateWaterClusters();
            SmoothWaterClusters();
            return;
        }

        Vector3 centerPos = centerTransform.position;
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3 isoPosition = GridToIsometric(x - mapWidth / 2, y - mapHeight / 2) + centerPos;
                isoPosition.z = 0;

                Gizmos.color = waterMap[x, y] ? Color.cyan : Color.green;

                Vector3 top = isoPosition + new Vector3(0, tileHeight / 2, 0);
                Vector3 bottom = isoPosition + new Vector3(0, -tileHeight / 2, 0);
                Vector3 left = isoPosition + new Vector3(-tileWidth / 2, 0, 0);
                Vector3 right = isoPosition + new Vector3(tileWidth / 2, 0, 0);

                Gizmos.DrawLine(top, left);
                Gizmos.DrawLine(left, bottom);
                Gizmos.DrawLine(bottom, right);
                Gizmos.DrawLine(right, top);

                if (!isPaint) continue;
                Vector3[] vertices = { top, right, bottom, left };
                Color tileColor = waterMap[x, y] ? new Color(0, 0.5f, 1f, 0.5f) : new Color(0, 1f, 0, 0.5f);
                //Handles.DrawSolidRectangleWithOutline(vertices, tileColor, Color.black);
            }
        }
    }
}