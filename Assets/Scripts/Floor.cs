using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public bool CantMove = false;
    public Vector2 index;
    public Floor[] neighborFloors;
    public Wall[] neighborWalls;

    public void GetNeighborFloors()
    {
        if (index.y + 1 < CreateMap.Instance.cubes.GetLength(1))
        {
            neighborFloors[0] = CreateMap.Instance.cubes[(int)index.x, (int)index.y + 1];
        }
        if (index.y - 1 >= 0)
        {
            neighborFloors[1] = CreateMap.Instance.cubes[(int)index.x, (int)index.y - 1];
        }
        if (index.x - 1 >= 0)
        {
            neighborFloors[2] = CreateMap.Instance.cubes[(int)index.x - 1, (int)index.y];
        }
        if (index.x + 1 < CreateMap.Instance.cubes.GetLength(0))
        {
            neighborFloors[3] = CreateMap.Instance.cubes[(int)index.x + 1, (int)index.y];
        }
    }

    public void GetNeighborWalls()
    {
        neighborWalls[0] = CreateMap.Instance.walls1[(int)index.x, (int)index.y + 1];
        neighborWalls[1] = CreateMap.Instance.walls1[(int)index.x, (int)index.y];
        neighborWalls[2] = CreateMap.Instance.walls2[(int)index.x, (int)index.y];
        neighborWalls[3] = CreateMap.Instance.walls2[(int)index.x + 1, (int)index.y];
    }

    public List<Vector2> SetRoad(int x, int y, List<Vector2> path)
    {
        CantMove = true;
        if (index.y >= CreateMap.Instance.Y - 1)
        {
            path.Add(index);
            return path;
        }
        path.Add(index);
        List<Floor> cubes = new List<Floor>();
        foreach (var item in neighborFloors)
        {
            if (item != null)
            {
                if (item.CantMove == false)
                {
                    cubes.Add(item);
                }
            }
        }
 
        if (cubes.Count > 0)
        {
            Floor cube = null;
            cube = cubes[UnityEngine.Random.Range(0, cubes.Count)];
            return cube.SetRoad(x, y, path);
        }
        else
        {
            foreach (var item in CreateMap.Instance.cubes)
            {
                item.CantMove = false;
            }
            return CreateMap.Instance.cubes[x, y].SetRoad(x, y, new List<Vector2>());
        }

    }
}