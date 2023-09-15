using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMap : MonoBehaviour
{
    public static CreateMap Instance;
    private void Awake()
    {
        Instance = this;
    }
    public int level;
    
    // size of map
    public int X = 5;
    public int Y = 5;
    
    // ground and wall
    public GameObject groundPrefab;
    public GameObject wallPrefab;
    public GameObject wallPrefab1;

    // 2d array
    public Floor[,] cubes;
    public Wall[,] walls1;
    public Wall[,] walls2;

    // start and end
    public Vector2 startPoint;
    public Vector3 start;
    public Vector3 end;
    public bool isTest;

    // store floors that can not move
    public List<Floor> cantMove = new List<Floor>();

    // create maze
    public void Create()
    {
        // clear existed maze
        Clear();

        // initialize of arrays
        Init();

        // step 1 of creating maze
        CreateStep1();

        // step 2 of creating maze
        CreateStep2();
    }

    public void Clear()
    {
        if (cubes != null)
        {
            foreach (var item in cubes)
            {
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }
        }
        if (walls1 != null)
        {
            foreach (var item in walls1)
            {
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }
        }
        if (walls2 != null)
        {
            foreach (var item in walls2)
            {
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }
        }
    }

    public void Init()
    {
        // initialize arrays
        cubes = new Floor[X, Y];
        walls1 = new Wall[X, Y + 1];
        walls2 = new Wall[X + 1, Y];

        // create ground and wall
        CreateGround();
        CreateWall1();
        CreateWall2();

        // initialize ground
        foreach (var item in cubes)
        {
            item.neighborFloors = new Floor[4];
            item.neighborWalls = new Wall[4];
            item.GetNeighborFloors();
            item.GetNeighborWalls();
        }
    }
    
    public void CreateGround()
    {
        for (int i = 0; i < X; i++)
        {
            for (int j = 0; j < Y; j++)
            {
                Floor item = Instantiate(groundPrefab).GetComponent<Floor>();
                item.transform.position = new Vector3(i, 0, j);
                cubes[i, j] = item;
                item.index = new Vector2(i, j);
            }
        }
    }
 
    public void CreateWall1()
    {
        for (int i = 0; i < X; i++)
        {
            for (int j = 0; j < Y + 1; j++)
            {

                Wall item = Instantiate(wallPrefab).GetComponent<Wall>();
                item.transform.position = new Vector3(i, 0.5f, j - 0.5f);
                walls1[i, j] = item;
                if (j == 0 || j == Y)
                {
                    item.isBorder = true;
                }
            }
        }
    }

    public void CreateWall2()
    {
        for (int i = 0; i < X + 1; i++)
        {
            for (int j = 0; j < Y; j++)
            {
                Wall item = Instantiate(wallPrefab1).GetComponent<Wall>();
                item.transform.position = new Vector3(i - 0.5f, 0.5f, j);
                walls2[i, j] = item;
                if (i == X || i == 0)
                {
                    item.isBorder = true;
                }
            }
        }
    }
    public void CreateStep1()
    {
        List<Vector2> road = cubes[(int)startPoint.x, (int)startPoint.y].
            SetRoad((int)startPoint.x, (int)startPoint.y, new List<Vector2>());

        for (int i = 0; i < road.Count - 1; i++)
        {
            DestoryWall(road[i], road[i + 1]);
            if (i == 0)
            {
                Destroy(cubes[(int)road[i].x, (int)road[i].y].neighborWalls[1].gameObject);
                cubes[(int)road[i].x, (int)road[i].y].neighborWalls[1] = null;

                start = new Vector3((float)road[i].x, (float)1, (float)road[i].y);
            }
            if (i == road.Count - 2)
            {
                Destroy(cubes[(int)road[i + 1].x, (int)road[i + 1].y].neighborWalls[0].gameObject);
                cubes[(int)road[i + 1].x, (int)road[i + 1].y].neighborWalls[0] = null;

                end = new Vector3((float)road[i + 1].x, (float)0.1, (float)road[i + 1].y);
            }
        }

        DigWall();

        for (int i = 0; i < X; i++)
        {
            for (int j = 0; j < Y; j++)
            {
                cantMove.Add(cubes[i, j]);
            }
        }
    }
    public void CreateStep2()
    {
        while (cantMove.Count > 0)
        {
            for (int i = 0; i < 5; i++)
            {
                cantMove = SelectCantMove();
            }

            Level();

        }
    }
    private void Start()
    {
        Create();
    }

    public bool NeighborCanMove(Floor item)
    {
        for (int i = 0; i < 4; i++)
        {
            if (item.neighborFloors[i] != null && item.neighborFloors[i].CantMove)
            {
                return true;
            }
        }
        return false;
    }

    public void DigWall()
    {
        foreach (var item in cubes)
        {
            bool isFull = true;

            for (int i = 0; i < 4; i++)
            {
                if (item.neighborWalls[i] == null)
                {
                    isFull = false;
                }
            }
            if (isFull)
            {
                List<Floor> roadNeighbor = new List<Floor>();

                foreach (var cube in item.neighborFloors)
                {
                    if (cube != null && cube.CantMove == true)
                    {
                        roadNeighbor.Add(cube);
                    }
                }
                if (roadNeighbor.Count > 0)
                {
                    Floor cubeItem = roadNeighbor[UnityEngine.Random.Range(0, roadNeighbor.Count)];
                    DestoryWall(item.index, cubeItem.index);
                }
                else
                {
                    List<Wall> walls = new List<Wall>();
                    foreach (var wall in item.neighborWalls)
                    {
                        if (!wall.isBorder)
                        {
                            walls.Add(wall);
                        }
                    }
                    int dir = UnityEngine.Random.Range(0, walls.Count);
                    for (int i = 0; i < 4; i++)
                    {
                        if (item.neighborWalls[i] != null && item.neighborWalls[i] == walls[dir])
                        {
                            dir = i;
                            break;
                        }
                    }
                    switch (dir)
                    {
                        case 0:
                            Destroy(item.neighborWalls[0].gameObject);
                            item.neighborWalls[0] = null;
                            if (item.neighborFloors[0] != null)
                                item.neighborFloors[0].neighborWalls[1] = null;
                            break;
                        case 1:
                            Destroy(item.neighborWalls[1].gameObject);
                            item.neighborWalls[1] = null;
                            if (item.neighborFloors[1] != null)
                                item.neighborFloors[1].neighborWalls[0] = null;
                            break;
                        case 2:
                            Destroy(item.neighborWalls[2].gameObject);
                            item.neighborWalls[2] = null;
                            if (item.neighborFloors[2] != null)
                                item.neighborFloors[2].neighborWalls[3] = null;
                            break;
                        case 3:
                            Destroy(item.neighborWalls[3].gameObject);
                            item.neighborWalls[3] = null;
                            if (item.neighborFloors[3] != null)
                                item.neighborFloors[3].neighborWalls[2] = null;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    public void DestoryWall(Vector2 front, Vector2 back)
    {
        if (Mathf.Abs(front.x - back.x) > 0.1)
        {
            if (front.x - back.x > 0.1)
            {
                Destroy(cubes[(int)front.x, (int)front.y].neighborWalls[2].gameObject);
                cubes[(int)front.x, (int)front.y].neighborWalls[2] = null;
                cubes[(int)back.x, (int)back.y].neighborWalls[3] = null;

            }
            else
            {
                Destroy(cubes[(int)front.x, (int)front.y].neighborWalls[3].gameObject);
                cubes[(int)front.x, (int)front.y].neighborWalls[3] = null;
                cubes[(int)back.x, (int)back.y].neighborWalls[2] = null;
            }
        }
        else
        {
            if (front.y - back.y > 0.1)
            {       
                Destroy(cubes[(int)front.x, (int)front.y].neighborWalls[1].gameObject);
                cubes[(int)front.x, (int)front.y].neighborWalls[1] = null;
                cubes[(int)back.x, (int)back.y].neighborWalls[0] = null;

            }
            else
            {

                Destroy(cubes[(int)front.x, (int)front.y].neighborWalls[0].gameObject);
                cubes[(int)front.x, (int)front.y].neighborWalls[0] = null;
                cubes[(int)back.x, (int)back.y].neighborWalls[1] = null;
            }
        }
    }

    public List<Floor> SelectCantMove()
    {
        List<Floor> cantMoveCubes = new List<Floor>();
        for (int i = 0; i < cantMove.Count; i++)
        {
            if (cantMove[i].CantMove)
            {
                continue;
            }
            bool canMove = false;
            for (int j = 0; j < 4; j++)
            {
                if (cantMove[i].neighborWalls[j] == null && cantMove[i].neighborFloors[j] != null && cantMove[i].neighborFloors[j].CantMove)
                {
                    canMove = true;
                    cantMove[i].CantMove = true;
                    if (isTest)
                    {
                        cantMove[i].GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0);
                    }       
                    break;
                }
            }
            if (!canMove)
            {
                cantMoveCubes.Add(cantMove[i]);
            }
        }
        return cantMoveCubes;

    }

    public void Level()
    {
        if (level <= 0)
        {
            level = 1;
        }
        if (level >= 50)
        {
            level = 50;
        }

        for (int i = 0; i < level; i++)
        {
            if (cantMove.Count > 0)
            {
                Floor item = null;
                while (true)
                {
                    int index1 = UnityEngine.Random.Range(0, cantMove.Count);
                    item = cantMove[index1];

                    if (NeighborCanMove(item))
                    {
                        break;
                    }
                };

                List<int> dirs = new List<int>();
                for (int k = 0; k < 4; k++)
                {
                    if (item.neighborWalls[k] != null && !item.neighborWalls[k].isBorder)
                    {
                        dirs.Add(k);
                    }
                }
                if (dirs.Count > 0)
                {
                    int index = UnityEngine.Random.Range(0, dirs.Count);
                    Destroy(item.neighborWalls[dirs[index]].gameObject);
                    item.neighborWalls[dirs[index]] = null;
                    switch (dirs[index])
                    {
                        case 0:
                            item.neighborFloors[dirs[index]].neighborWalls[1] = null;
                            break;
                        case 1:
                            item.neighborFloors[dirs[index]].neighborWalls[0] = null;
                            break;
                        case 2:
                            item.neighborFloors[dirs[index]].neighborWalls[3] = null;
                            break;
                        case 3:
                            item.neighborFloors[dirs[index]].neighborWalls[2] = null;
                            break;
                        default:
                            break;
                    }

                }
            }

        }
    }
}