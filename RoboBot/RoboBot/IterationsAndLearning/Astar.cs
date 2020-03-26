﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;


namespace BCDK
{
    public class Astar
    {
        private const int STRAIGHT_MOVEMENT_COST = 10;
        private const int DIAGONAAL_MOVEMENT_COST = 14;

        private Grid<PathNode> myGrid;
        private List<PathNode> openList;
        private List<PathNode> closedList;

        public Grid<PathNode> GetGrid()
        {
            return myGrid;
        }

        private List<PathNode> GetNeighbourList(PathNode currentNode, Grid<PathNode> grid)
        {
            myGrid = grid;
            List<PathNode> neighbourList = new List<PathNode>();

            if (currentNode.x - 1 >= 0)
            {
                if (grid.GetGridObject(currentNode.x - 1, currentNode.y) != null)
                {
                    if (grid.GetGridObject(currentNode.x - 1, currentNode.y).isWalkable)
                    {
                        // Left
                        neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
                    }
                }
            }
            if (currentNode.x + 1 < grid.GetWidth())
            {
                if (grid.GetGridObject(currentNode.x + 1, currentNode.y) != null)
                {
                    if (grid.GetGridObject(currentNode.x + 1, currentNode.y).isWalkable)
                    {
                        // Right
                        neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
                    }
                }
            }
            if (grid.GetGridObject(currentNode.x, currentNode.y - 1) != null)
            {
                if (grid.GetGridObject(currentNode.x, currentNode.y - 1).isWalkable)
                {
                    // Down
                    if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
                }
            }
            if (grid.GetGridObject(currentNode.x, currentNode.y + 1) != null)
            {
                if (grid.GetGridObject(currentNode.x, currentNode.y + 1).isWalkable)
                {
                    // Up
                    if (currentNode.y + 1 < myGrid.GetHeight()) neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));
                }
            }

            return neighbourList;
        }

        public PathNode GetNode(int x, int y)
        {
            return myGrid.GetGridObject(x, y);
        }

        private List<PathNode> CalculatePath(PathNode endNode)
        {
            List<PathNode> path = new List<PathNode>();
            path.Add(endNode);
            PathNode currentNode = endNode;
            while (currentNode.cameFromNode != null)
            {
                path.Add(currentNode.cameFromNode);
                currentNode = currentNode.cameFromNode;
            }
            path.Reverse();
            return path;
        }

        private int CalculateDistanceCost(PathNode a, PathNode b)
        {
            int xDistance = Math.Abs(a.x - b.x);
            int yDistance = Math.Abs(a.y - b.y);
            int remaining = Math.Abs(xDistance - yDistance);
            return DIAGONAAL_MOVEMENT_COST * Math.Min(xDistance, yDistance) + STRAIGHT_MOVEMENT_COST * remaining;
        }

        private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
        {
            PathNode lowestFCostNode = pathNodeList[0];
            for (int i = 1; i < pathNodeList.Count; i++)
            {
                if (pathNodeList[i].fCost < lowestFCostNode.fCost)
                {
                    lowestFCostNode = pathNodeList[i];
                }
            }
            return lowestFCostNode;
        }


        public List<PathNode> FindPathNodes(PathNode startNode, PathNode endNode, Grid<PathNode> myGrid)
        {
            //myGrid = new Grid<PathNode>(, grid.GetLength(1), 10f, Vector3.zero, (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));
            openList = new List<PathNode> { startNode };
            closedList = new List<PathNode>();

            for (int x = 0; x < myGrid.GetWidth(); x++)
            {
                for (int y = 0; y < myGrid.GetHeight(); y++)
                {
                    PathNode pathNode = myGrid.GetGridObject(x, y);
                    pathNode.gCost = int.MaxValue;
                    pathNode.CalculateFCost();
                    pathNode.cameFromNode = null;
                }
            }

            startNode.gCost = 0;
            //Heuristic
            startNode.hCost = CalculateDistanceCost(startNode, endNode);
            startNode.CalculateFCost();


            while (openList.Count > 0)
            {
                PathNode currentNode = GetLowestFCostNode(openList);
                if (currentNode == endNode)
                {
                    //Reached final node
                    return CalculatePath(endNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (PathNode neibhourNode in GetNeighbourList(currentNode, myGrid))
                {
                    if (closedList.Contains(neibhourNode)) continue;
                    if (!neibhourNode.isWalkable)
                    {
                        closedList.Add(neibhourNode);
                        continue;
                    }

                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neibhourNode);
                    if (tentativeGCost < neibhourNode.gCost)
                    {
                        neibhourNode.cameFromNode = currentNode;
                        neibhourNode.gCost = tentativeGCost;
                        neibhourNode.hCost = CalculateDistanceCost(neibhourNode, endNode);
                        neibhourNode.CalculateFCost();

                        if (!openList.Contains(neibhourNode))
                        {
                            openList.Add(neibhourNode);
                        }
                    }
                }
            }

            //Out of nodes on the openlist
            return null;
        }

        private List<PathNode> CalculatePathNodes(PathNode endNode)
        {
            List<PathNode> path = new List<PathNode>();
            path.Add(endNode);
            PathNode currentNode = endNode;
            while (currentNode.cameFromNode != null)
            {
                path.Add(currentNode.cameFromNode);
                currentNode = currentNode.cameFromNode;
            }
            path.Reverse();
            return path;
        }

        /*
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
        {
            List<PathNode> path = FindPathNodes(startPos, endPos, grid);

            if(path != null)
            {
                return ConvertToVector2Int(path);
            }
            else
            {
                Debug.Log("Path is null");
                return new List<Vector2Int>();
            }
        }

        public List<Vector2Int> ConvertToVector2Int(List<PathNode> list)
        {
            List<Vector2Int> newPath = new List<Vector2Int>();
            foreach(PathNode node in list)
            {
                Vector2Int nodePosition = new Vector2Int(node.x, node.y);
                newPath.Add(nodePosition);
            }
            return newPath;
        }*/

        public float Heuristic(PathNode a, PathNode b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
        }

        
    }

    public class PathNode
    {
        private Grid<PathNode> grid;
        public int x;
        public int y;

        public int gCost;
        public int hCost;
        public int fCost;

        public bool isWalkable;
        public PathNode cameFromNode;

        public PathNode(Grid<PathNode> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
            isWalkable = true;
        }

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }

        public void SetIsWalkable(bool isWalkable)
        {
            this.isWalkable = isWalkable;
            grid.TriggerGridObjectChanged(x, y);
        }

        public override string ToString()
        {
            return x + "," + y;
        }
    }

    public class Grid<TGridObject>
    {

        public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
        public class OnGridObjectChangedEventArgs : EventArgs
        {
            public int x;
            public int y;
        }

        private int width;
        private int height;
        private float cellSize;
        private TGridObject[,] gridArray;

        public Grid(int width, int height, float cellSize, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;

            gridArray = new TGridObject[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    gridArray[x, y] = createGridObject(this, x, y);
                }
            }
        }

        public int GetWidth()
        {
            return width;
        }

        public int GetHeight()
        {
            return height;
        }

        public float GetCellSize()
        {
            return cellSize;
        }

        public void SetGridObject(int x, int y, TGridObject value)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                gridArray[x, y] = value;
                if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });
            }
        }

        public void TriggerGridObjectChanged(int x, int y)
        {
            if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });
        }

        public TGridObject GetGridObject(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                return gridArray[x, y];
            }
            else
            {
                return default(TGridObject);
            }
        }
    }
}
