using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    public class Cell
    {
        public bool[] status = new bool[4]; // 0: Arriba, 1: Abajo, 2: Derecha, 3: Izquierda
        public bool visited = false;
    }
    

    public GameObject coinPrefab; // Prefab de la moneda
    public GameObject[] roomPrefabs; // Array de prefabs de salas
    public Vector2Int size;
    public int startPos = 0;
    public Vector2 offset;
    public int startRoomIndex = 0; // Índice de la sala de inicio dentro del array

    private List<Cell> board;

    void Start()
    {
        if (size.x <= 0 || size.y <= 0)
        {
            Debug.LogError("Size values must be greater than zero!");
            return;
        }

        if (roomPrefabs == null || roomPrefabs.Length == 0)
        {
            Debug.LogError("No room prefabs assigned!");
            return;
        }

        if (startRoomIndex < 0 || startRoomIndex >= roomPrefabs.Length)
        {
            Debug.LogError("Start room index is out of bounds!");
            return;
        }

        GenerateMaze();
    }

    void GenerateMaze()
    {
        board = new List<Cell>();
        for (int i = 0; i < size.x * size.y; i++)
        {
            board.Add(new Cell());
        }

        if (startPos < 0 || startPos >= board.Count)
        {
            Debug.LogError($"Start position is out of bounds! startPos: {startPos}, board.Count: {board.Count}");
            return;
        }

        int currentCell = startPos;
        Stack<int> path = new Stack<int>();

        board[currentCell].visited = true;
        path.Push(currentCell);

        while (path.Count > 0)
        {
            currentCell = path.Pop();
            List<int> neighbours = GetNeighbours(currentCell);

            if (neighbours.Count > 0)
            {
                path.Push(currentCell);
                int newCell = neighbours[Random.Range(0, neighbours.Count)];

                ConnectCells(currentCell, newCell);
                board[newCell].visited = true;
                path.Push(newCell);
            }
        }

        GenerateDungeon();
    }

    void ConnectCells(int currentCell, int newCell)
    {
        int difference = newCell - currentCell;

        if (difference == size.x) // Arriba
        {
            board[currentCell].status[0] = true;
            board[newCell].status[1] = true;
        }
        else if (difference == -size.x) // Abajo
        {
            board[currentCell].status[1] = true;
            board[newCell].status[0] = true;
        }
        else if (difference == 1 && (currentCell % size.x) != (size.x - 1)) // Derecha
        {
            board[currentCell].status[2] = true;
            board[newCell].status[3] = true;
        }
        else if (difference == -1 && (currentCell % size.x) != 0) // Izquierda
        {
            board[currentCell].status[3] = true;
            board[newCell].status[2] = true;
        }
    }

    List<int> GetNeighbours(int cell)
    {
        List<int> neighbours = new List<int>();
        int x = cell % size.x;
        int y = cell / size.x;

        if (y > 0 && !board[cell - size.x].visited) // Arriba
            neighbours.Add(cell - size.x);
        if (y < size.y - 1 && !board[cell + size.x].visited) // Abajo
            neighbours.Add(cell + size.x);
        if (x < size.x - 1 && !board[cell + 1].visited) // Derecha
            neighbours.Add(cell + 1);
        if (x > 0 && !board[cell - 1].visited) // Izquierda
            neighbours.Add(cell - 1);

        return neighbours;
    }

    void GenerateDungeon()
{
    List<GameObject> availablePrefabs = new List<GameObject>(roomPrefabs);
    int startX = startPos % size.x;
    int startY = startPos / size.x;

    // Asegurar que la sala inicial siempre tiene las mismas paredes
    board[startPos].status[0] = false; // Arriba cerrada
    board[startPos].status[1] = false; // Abajo cerrada
    board[startPos].status[2] = true;  // Derecha abierta (entrada)
    board[startPos].status[3] = false; // Izquierda cerrada

    // Generamos la sala de inicio
    GameObject startRoom = Instantiate(roomPrefabs[startRoomIndex], new Vector3(startX * offset.x, startY * offset.y, 0), Quaternion.identity, transform);
    RoomBehaviour startRoomBehaviour = startRoom.GetComponent<RoomBehaviour>();
    startRoomBehaviour.UpdateRoom(board[startPos].status);
    startRoom.name = "SalaInicial";

    // Asegurar que la sala a la derecha de la inicial tenga la izquierda abierta
    int rightIndex = startPos + 1;
    if (rightIndex % size.x != 0 && rightIndex < board.Count)
    {
        board[rightIndex].status[3] = true; // Abre la izquierda de la sala a la derecha
    }

    // Removemos la sala de inicio para que no se repita
    availablePrefabs.RemoveAt(startRoomIndex);

    Vector3 lastRoomPosition = Vector3.zero;

    for (int j = 0; j < size.y; j++)
    {
        for (int i = 0; i < size.x; i++)
        {
            int index = i + j * size.x;
            if (index == startPos || index < 0 || index >= board.Count) continue;

            GameObject selectedPrefab = availablePrefabs[Random.Range(0, availablePrefabs.Count)];

            var newRoom = Instantiate(selectedPrefab, new Vector3(i * offset.x, j * offset.y, 0), Quaternion.identity, transform)
                .GetComponent<RoomBehaviour>();

            newRoom.UpdateRoom(board[index].status);
            newRoom.name = $"Room {i}-{j}";

            lastRoomPosition = new Vector3(i * offset.x, j * offset.y, 0);

            if (i == 0) board[index].status[3] = true;
            if (i == size.x - 1) board[index].status[2] = true;
            if (j == 0) board[index].status[0] = true;
            if (j == size.y - 1) board[index].status[1] = true;
        }
    }

    // Spawn de la moneda en la última sala generada
    if (coinPrefab != null)
    {
        Instantiate(coinPrefab, lastRoomPosition, Quaternion.identity);
        Debug.Log("Moneda generada en la última sala.");
    }
    else
    {
        Debug.LogError("No se ha asignado un prefab de moneda.");
    }
}

}
