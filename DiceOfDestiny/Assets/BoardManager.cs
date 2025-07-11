using System.Collections.Generic;
using UnityEngine;

public enum TileColor
{
    Red,
    Green,
    Blue,
    Yellow,
    Purple,
    Gray,
    None
}

public class BoardManager : Singletone<BoardManager>
{ 
    [Header("Board Size Settings")]
    [SerializeField] public int boardSize = 11;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] public Transform boardTransform;
    public Tile[,] Board { get; set; }

    [Header("Tile Colors Settings")]
    [SerializeField]
    public Color[] tileColors = new Color[] {
        new Color(1f, 0f, 0f), // 빨강
        new Color(0f, 1f, 0f), // 초록
        new Color(0f, 0f, 1f), // 파랑
        new Color(1f, 1f, 0f), // 노랑
        new Color(1f, 0f, 1f), // 보라
        new Color(0.9f, 0.9f, 0.9f) // 회색
    };

    List<int> colorIndices = new List<int>();
    List<ObstacleType> obstacleIndices = new List<ObstacleType>();


    void Start()
    {
        GenerateBoard();
    }

    private void Update()
    {

    }

    private void GenerateBoard()
    {
        Board = new Tile[boardSize, boardSize];

        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                GameObject tileObject = Instantiate(tilePrefab, new Vector3(boardTransform.position.x + x, boardTransform.position.y + y, 0), Quaternion.identity, boardTransform);
                tileObject.name = $"Tile_{x}_{y}";
                Tile tile = tileObject.GetComponent<Tile>();
                tile.TileColor = TileColor.None;
                tile.Obstacle = ObstacleType.None;
                Board[x, y] = tile;
            }
        }
    }

    public void SetBoard(StageDifficultyProfile profile)
    {
        // 가중치에 맞게 색을 설정하는 부분
        colorIndices = new List<int>();
        for (int color = 0; color < tileColors.Length; color++)
        {
            for (int i = 0; i < profile.minimumColorEnsure; i++)
                colorIndices.Add(color);
        }

        int[] colorWeights = new int[]
        {
        profile.redWeight * profile.weightPower,
        profile.greenWeight * profile.weightPower,
        profile.blueWeight * profile.weightPower,
        profile.yellowWeight * profile.weightPower,
        profile.purpleWeight * profile.weightPower,
        profile.orangeWeight * profile.weightPower
        };

        int[] cumulativeWeights = new int[colorWeights.Length];
        cumulativeWeights[0] = colorWeights[0];

        for (int i = 1; i < colorWeights.Length; i++)
            cumulativeWeights[i] = cumulativeWeights[i - 1] + colorWeights[i];

        int total = cumulativeWeights[cumulativeWeights.Length - 1];


        for (int i = 0; i < (boardSize * boardSize) - (tileColors.Length * profile.minimumColorEnsure); i++)
        {
            int rand = Random.Range(0, total);
            for (int j = 0; j < cumulativeWeights.Length; j++)
            {
                if (rand < cumulativeWeights[j])
                {
                    colorIndices.Add(j);
                    break;
                }
            }
        }

        for (int i = colorIndices.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (colorIndices[i], colorIndices[j]) = (colorIndices[j], colorIndices[i]);
        }

        // 색칠하는 부분
        int idx = 0;
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                Board[x, y].SetTileColor(tileColors[colorIndices[idx]]);
                switch (colorIndices[idx])
                {
                    case 0: // 빨강
                        Board[x, y].TileColor = TileColor.Red;
                        break;
                    case 1: // 초록
                        Board[x, y].TileColor = TileColor.Green;
                        break;
                    case 2: // 파랑
                        Board[x, y].TileColor = TileColor.Blue;
                        break;
                    case 3: // 노랑
                        Board[x, y].TileColor = TileColor.Yellow;
                        break;
                    case 4: // 보라
                        Board[x, y].TileColor = TileColor.Purple;
                        break;
                    case 5: // 회색
                        Board[x, y].TileColor = TileColor.Gray;
                        break;
                }
                idx++;
            }
        }


        // 장애물 배치 부분
        obstacleIndices = new List<ObstacleType>();
        int tileCount = boardSize * boardSize;
        int obstacleCount = Mathf.RoundToInt(tileCount * profile.obstacleDensity);
        for (int i = 0; i < tileCount - obstacleCount; i++) // 장애물이 없는 타일 추가
        {
            obstacleIndices.Add(ObstacleType.None);
        }

        List<ObstacleType> availableObstacleWeight = new List<ObstacleType>();
        for (int i = 0; i < profile.availableObstacle.Count; i++) // 장애물 타입을 인덱스에 추가
        {
            for (int j = 0; j < profile.availableObstacle[i].weight * 10; j++)
            {
                availableObstacleWeight.Add(profile.availableObstacle[i].type);
            }
        }

        for (int i = 0; i < obstacleCount; i++) // 장애물이 있는 타일
        {
            int randIndex = Random.Range(0, availableObstacleWeight.Count);
            obstacleIndices.Add(availableObstacleWeight[randIndex]);
        }

        for (int i = obstacleIndices.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (obstacleIndices[i], obstacleIndices[j]) = (obstacleIndices[j], obstacleIndices[i]);
        }

        idx = 0;
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                Board[x, y].Obstacle = obstacleIndices[idx];
                // 장애물 생성
                if (obstacleIndices[idx] != ObstacleType.None)
                {
                    GameObject obstacle = Instantiate(ObstacleManager.Instance.obstaclePrefabs[obstacleIndices[idx]],
                        new Vector3(boardTransform.position.x + x, boardTransform.position.y + y, 0), Quaternion.identity, boardTransform);
                    obstacle.GetComponent<Obstacle>().obstaclePosition = new Vector2Int(x, y);
                    ObstacleManager.Instance.SetObstacle(obstacle);
                }
                idx++;
            }
        }
    }



    public Obstacle ReturnObstacleByPosition(Vector2Int position)
    {
        if (position.x < 0 || position.x >= boardSize || position.y < 0 || position.y >= boardSize)
        {
            Debug.LogError("Position out of bounds");
            return null;
        }
        Tile tile = Board[position.x, position.y];
        if (tile.Obstacle == ObstacleType.None)
        {
            return null;
        }
        foreach (GameObject obstacle in ObstacleManager.Instance.currentObstacles)
        {
            if (obstacle.GetComponent<Obstacle>().obstaclePosition == position)
            {
                return obstacle.GetComponent<Obstacle>();
            }
        }
        return null;
    }

    public bool IsEmptyTile(Vector2Int position)
    {
        if (position.x < 0 || position.x >= boardSize || position.y < 0 || position.y >= boardSize)
        {
            // Debug.LogError("Position out of bounds");
            return false;
        }
        return Board[position.x, position.y].Obstacle == ObstacleType.None;
    }

    public void MoveObstacle(Obstacle obstacle, Vector2Int nextPos)
    {
        Vector2Int currentPos = obstacle.obstaclePosition;
        Board[currentPos.x, currentPos.y].Obstacle = ObstacleType.None; // 현재 타일의 장애물 제거
        Board[nextPos.x, nextPos.y].Obstacle = obstacle.obstacleType; // 다음 타일에 장애물 설정
        obstacle.obstaclePosition = nextPos; // 장애물의 위치 업데이트
        // obstacle.transform.position = new Vector3(boardTransform.position.x + nextPos.x, boardTransform.position.y + nextPos.y, 0); // 장애물 위치 이동
    }

    // 주변 8칸 중 윗면과 같은 색이 몇개인지 카운팅하는 함수
    public int CountMatchingColors(Vector2Int position, TileColor targetColor)
    {
        int matchCount = 0;
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(-1, -1), // 좌상
            new Vector2Int(-1, 0),  // 좌
            new Vector2Int(-1, 1),  // 좌하
            new Vector2Int(0, -1),  // 상
            new Vector2Int(0, 1),   // 하
            new Vector2Int(1, -1),  // 우상
            new Vector2Int(1, 0),   // 우
            new Vector2Int(1, 1)    // 우하
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int checkPos = position + dir;
            if (checkPos.x >= 0 && checkPos.x < boardSize &&
                checkPos.y >= 0 && checkPos.y < boardSize)
            {
                if (Board[checkPos.x, checkPos.y] != null &&
                    Board[checkPos.x, checkPos.y].TileColor == targetColor)
                {
                    // 체크된게 어느 타일인지 보여주면 굿.
                    matchCount++;
                }
            }
        }

        return matchCount;
    }

    // 주변 8칸 중 윗면과 같은 색의 위치를 가져오는 함수
    public List<Vector2Int> GetMatchingColorTiles(Vector2Int position, TileColor targetColor)
    {
        List<Vector2Int> matchingTiles = new List<Vector2Int>();
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(-1, -1), // 좌상
            new Vector2Int(-1, 0),  // 좌
            new Vector2Int(-1, 1),  // 좌하
            new Vector2Int(0, -1),  // 상
            new Vector2Int(0, 1),   // 하
            new Vector2Int(1, -1),  // 우상
            new Vector2Int(1, 0),   // 우
            new Vector2Int(1, 1)    // 우하
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int checkPos = position + dir;
            if (checkPos.x >= 0 && checkPos.x < boardSize &&
                checkPos.y >= 0 && checkPos.y < boardSize)
            {
                if (Board[checkPos.x, checkPos.y] != null &&
                    Board[checkPos.x, checkPos.y].TileColor == targetColor)
                {
                    matchingTiles.Add(checkPos);
                }
            }
        }

        return matchingTiles;
    }

    // 주변 8칸 중 특정 색과 일치하는 칸만 재배정하는 함수
    public void ReassignMatchingColorTiles(Vector2Int position, TileColor targetColor)
    {
        Vector2Int[] directions = new Vector2Int[]
        {
        new Vector2Int(-1, -1), // 좌상
        new Vector2Int(-1, 0),  // 좌
        new Vector2Int(-1, 1),  // 좌하
        new Vector2Int(0, -1),  // 상
        new Vector2Int(0, 1),   // 하
        new Vector2Int(1, -1),  // 우상
        new Vector2Int(1, 0),   // 우
        new Vector2Int(1, 1)    // 우하
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int checkPos = position + dir;
            if (checkPos.x >= 0 && checkPos.x < boardSize &&
                checkPos.y >= 0 && checkPos.y < boardSize &&
                Board[checkPos.x, checkPos.y] != null &&
                Board[checkPos.x, checkPos.y].TileColor == targetColor)
            {
                // 각 타일마다 독립적으로 무작위 색상 인덱스 선택 (None 제외)
                int randomColorIndex = Random.Range(0, tileColors.Length); // tileColors.Length는 6 (Red, Green, Blue, Yellow, Purple, Gray)

                // 타일 색상 설정
                Board[checkPos.x, checkPos.y].SetTileColor(tileColors[randomColorIndex]);
                // TileColor 열거형 값 설정
                switch (randomColorIndex)
                {
                    case 0: Board[checkPos.x, checkPos.y].TileColor = TileColor.Red; break;
                    case 1: Board[checkPos.x, checkPos.y].TileColor = TileColor.Green; break;
                    case 2: Board[checkPos.x, checkPos.y].TileColor = TileColor.Blue; break;
                    case 3: Board[checkPos.x, checkPos.y].TileColor = TileColor.Yellow; break;
                    case 4: Board[checkPos.x, checkPos.y].TileColor = TileColor.Purple; break;
                    case 5: Board[checkPos.x, checkPos.y].TileColor = TileColor.Gray; break;
                }
            }
        }
    }

    // 주변 8칸 색상 재배정하는 함수
    public void ReassignSurroundingColors(Vector2Int position)
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(-1, -1), // 좌상
            new Vector2Int(-1, 0),  // 좌
            new Vector2Int(-1, 1),  // 좌하
            new Vector2Int(0, -1),  // 상
            new Vector2Int(0, 1),   // 하
            new Vector2Int(1, -1),  // 우상
            new Vector2Int(1, 0),   // 우
            new Vector2Int(1, 1)    // 우하
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int checkPos = position + dir;
            if (checkPos.x >= 0 && checkPos.x < boardSize &&
                checkPos.y >= 0 && checkPos.y < boardSize &&
                Board[checkPos.x, checkPos.y] != null)
            {
                // 무작위 색상 인덱스 선택
                int randomColorIndex = Random.Range(0, tileColors.Length);
                // 타일 색상 설정
                Board[checkPos.x, checkPos.y].SetTileColor(tileColors[randomColorIndex]);
                // TileColor 열거형 값 설정
                switch (randomColorIndex)
                {
                    case 0: Board[checkPos.x, checkPos.y].TileColor = TileColor.Red; break;
                    case 1: Board[checkPos.x, checkPos.y].TileColor = TileColor.Green; break;
                    case 2: Board[checkPos.x, checkPos.y].TileColor = TileColor.Blue; break;
                    case 3: Board[checkPos.x, checkPos.y].TileColor = TileColor.Yellow; break;
                    case 4: Board[checkPos.x, checkPos.y].TileColor = TileColor.Purple; break;
                    case 5: Board[checkPos.x, checkPos.y].TileColor = TileColor.Gray; break;
                }
            }
    public Tile GetTile(Vector2Int position)
    {
        if(position.x < 0 || position.x >= boardSize || position.y < 0 || position.y >= boardSize)
        {
            return null;
        }
        else
        {
            return Board[position.x, position.y];
        }
    }
}
