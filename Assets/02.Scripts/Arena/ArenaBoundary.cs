using UnityEngine;

public class ArenaBoundary : MonoBehaviour
{
    [SerializeField] private Vector2 _arenaSize = new Vector2(20f, 12f);
    [SerializeField] private float _wallThickness = 1f;

    public Vector2 ArenaSize => _arenaSize;

    private void Awake()
    {
        CreateWalls();
    }

    public Vector3 GetRandomEdgePosition()
    {
        float halfWidth = _arenaSize.x / 2f;
        float halfHeight = _arenaSize.y / 2f;
        int edge = Random.Range(0, 4);

        return edge switch
        {
            0 => new Vector3(Random.Range(-halfWidth, halfWidth), halfHeight, 0),
            1 => new Vector3(Random.Range(-halfWidth, halfWidth), -halfHeight, 0),
            2 => new Vector3(-halfWidth, Random.Range(-halfHeight, halfHeight), 0),
            3 => new Vector3(halfWidth, Random.Range(-halfHeight, halfHeight), 0),
            _ => Vector3.zero
        };
    }

    private void CreateWalls()
    {
        float halfWidth = _arenaSize.x / 2f;
        float halfHeight = _arenaSize.y / 2f;
        float t = _wallThickness;

        CreateWall("Wall_Top",
            new Vector3(0, halfHeight + t / 2f, 0),
            new Vector2(_arenaSize.x + t * 2, t));

        CreateWall("Wall_Bottom",
            new Vector3(0, -halfHeight - t / 2f, 0),
            new Vector2(_arenaSize.x + t * 2, t));

        CreateWall("Wall_Left",
            new Vector3(-halfWidth - t / 2f, 0, 0),
            new Vector2(t, _arenaSize.y + t * 2));

        CreateWall("Wall_Right",
            new Vector3(halfWidth + t / 2f, 0, 0),
            new Vector2(t, _arenaSize.y + t * 2));
    }

    private void CreateWall(string wallName, Vector3 position, Vector2 size)
    {
        var wall = new GameObject(wallName);
        wall.transform.parent = transform;
        wall.transform.localPosition = position;
        var col = wall.AddComponent<BoxCollider2D>();
        col.size = size;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(_arenaSize.x, _arenaSize.y, 0));
    }
}