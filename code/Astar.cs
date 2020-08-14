using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar
{

    List<Node> close;
    List<Node> open;
    List<Vector3> path;

    public void InitList()
    {
        close = new List<Node>();
        open = new List<Node>();

    }
    //startAStar都會創造一個新的hashtable
    public List<Vector3> startAStar(Node start, Node end)
    {

        path = new List<Vector3>();
        close.Clear();
        open.Clear();

        Node startN = start;
        Node endN = end;

        open.Add(startN);
        while (open.Count != 0)
        {
            Node now = open[0];
            for (int i = 0; i < open.Count; i++)
            {
                if (open[i].f < now.f)
                {
                    now = open[i];
                }
            }

            open.Remove(now);
            close.Add(now);
            if (now == endN)
            {
                buildPath(startN, endN, path);
                break;
            }
            foreach (Node n in now.neighbor)
            {
                if (close.Contains(n))
                {
                    continue;
                }
                float newCost = now.g + getLength(now, n);

                if (newCost < n.g || !open.Contains(n))
                {
                    n.g = newCost;
                    n.h = getLength(n, endN);
                    n.parent = now;

                    if (!open.Contains(n))
                    {
                        open.Add(n);
                    }
                }
            }
        }
        return path;
    }
    private float getLength(Node start, Node target)
    {
        Vector3 v = start.pos - target.pos;
        float dist = v.magnitude;
        return dist;
    }

    private void buildPath(Node start, Node end, List<Vector3> path)
    {
        Node now = end;
        while (now != start)
        {
            path.Add(now.pos);
            now = now.parent;
            if (now == start)
            {
                path.Add(now.pos);
            }
        }
        path.Reverse();

    }
}
