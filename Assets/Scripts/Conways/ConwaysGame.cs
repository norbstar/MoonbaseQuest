using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

// using ExtensionMethods;

namespace Conways
{
    public class ConwaysGame : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] GameObject cellPrefab;

        [Header("Config")]
        [SerializeField] Material liveCellMaterial;
        [SerializeField] Material deadCellMaterial;
        [SerializeField] float intervalDelaySec = 0.5f;

        public class Cell
        {
            public enum Status
            {
                Live,
                Dead,
                Destroyed
            }

            public GameObject gameObject;
            public Vector3 position;
            public Status status;
            public Status? pendingStatus;
        }

        private List<Cell> cells;
        private Coroutine coroutine;

        public void Init(List<ConwaysCellUIManager> cells)
        {
            Reset();

            foreach (ConwaysCellUIManager cell in cells)
            {
                if (cell.IsSelected)
                {
                    AddCell(Instantiate(new Vector3(cell.Coord.x, 0.5f, cell.Coord.y), Cell.Status.Live), Cell.Status.Live);
                }
            }
            
            CreateDeadCells();
            coroutine = StartCoroutine(StartGameCoroutine());
        }

        public void Reset() => cells = new List<Cell>();

        private GameObject Instantiate(Vector3 coord, Cell.Status status)
        {
            var instance = GameObject.Instantiate(cellPrefab, coord, Quaternion.identity, transform);
            instance.GetComponent<MeshRenderer>().material = (status == Cell.Status.Live) ? liveCellMaterial : deadCellMaterial;

            return instance;
        }

        private Cell AddCell(GameObject instance, Cell.Status status)
        {
            var cell = new Cell
            {
                gameObject = instance,
                position = gameObject.transform.position,
                status = status
            };

            cells.Insert(cells.Count, cell);
            return cell;
        }

        private List<Vector3> GetAdjacentPositions(Vector3 position)
        {
            return new List<Vector3>
            {
                new Vector3(position.x - 1, position.y, position.z),
                new Vector3(position.x + 1, position.y, position.z),
                new Vector3(position.x, position.y, position.z - 1),
                new Vector3(position.x, position.y, position.z + 1),
                new Vector3(position.x - 1, position.y, position.z - 1),
                new Vector3(position.x + 1, position.y, position.z - 1),
                new Vector3(position.x - 1, position.y, position.z + 1),
                new Vector3(position.x + 1, position.y, position.z + 1),
            };
        }

        private bool CellExists(Vector3 position)
        {
            foreach (Cell cell in cells)
            {
                if (cell.gameObject.transform.position == position)
                {
                    return true;
                }
            }

            return false;
        }

        private bool TryGetCell(Vector3 position, out Cell result)
        {
            foreach (Cell cell in cells)
            {
                if (cell.gameObject.transform.position == position)
                {
                    result = cell;
                    return true;
                }
            }

            result = null;
            return false;
        }

        private List<Cell> GetAdjacentCells(Vector3 position)
        {
            List<Cell> instances = new List<Cell>();
            List<Vector3> adjacentPoisitions = GetAdjacentPositions(position);

            foreach (Vector3 adjacentPoisition in adjacentPoisitions)
            {
                if (TryGetCell(adjacentPoisition, out Cell cell))
                {
                    instances.Add(cell);
                }
            }

            return instances;
        }

        private List<Cell> GetAdjacentLiveCells(Vector3 position) => GetAdjacentCells(position).Where(c => c.status == Cell.Status.Live).ToList<Cell>();

        private List<Cell> GetAdjacentLiveCells(List<Cell> cells) => cells.Where(c => c.status == Cell.Status.Live).ToList<Cell>();

        private List<Cell> GetAdjacentDeadCells(Vector3 position) => GetAdjacentCells(position).Where(c => c.status == Cell.Status.Dead).ToList<Cell>();

        private List<Cell> GetAdjacentDeadCells(List<Cell> cells) => cells.Where(c => c.status == Cell.Status.Dead).ToList<Cell>();


        private void CreateDeadCells(Vector3 position)
        {
            if (TryGetCell(position, out Cell cell))
            {
                if (cell.status == Cell.Status.Dead)
                {
                    return;
                }
            }

            List<Vector3> adjacentPositions = GetAdjacentPositions(position);

            foreach (Vector3 adjacentPosition in adjacentPositions)
            {
                if (!CellExists(adjacentPosition))
                {
                    AddCell(Instantiate(adjacentPosition, Cell.Status.Dead), Cell.Status.Dead);
                }
            }
        }

        private void CreateDeadCells()
        {
            foreach (Cell cell in cells.ToList<Cell>())
            {
                CreateDeadCells(cell.gameObject.transform.position);
            }
        }

        // private List<Cell> UpdateWithDeadCells(List<Cell> cells)
        // {
        //     var copy = cells.ToList<Cell>();

        //     foreach (Cell cell in copy)
        //     {
        //         CreateDeadCells(cell.gameObject.transform.position);
        //     }

        //     return copy;
        // }

        private void ProcessPendingStatusChange()
        {
            for (int itr = 0; itr < cells.Count; itr++)
            {
                var cell = cells[itr];

                if ((cell.pendingStatus != null) && (cell.pendingStatus != cell.status))
                {
                    if (cell.pendingStatus == Cell.Status.Live)
                    {
                        cell.gameObject.GetComponent<MeshRenderer>().material = liveCellMaterial;
                        cell.status = Cell.Status.Live;
                    }
                    else if (cell.pendingStatus == Cell.Status.Dead)
                    {
                        cell.gameObject.GetComponent<MeshRenderer>().material = deadCellMaterial;
                        cell.status = Cell.Status.Dead;
                    }
                    else if (cell.pendingStatus == Cell.Status.Destroyed)
                    {
                        cells.RemoveAt(itr);
                        Destroy(cell.gameObject);
                    }

                    cell.pendingStatus = null;
                }
            }
        }

        private IEnumerator StartGameCoroutine()
        {
            while (true)
            {
                ProcessPendingStatusChange();
                CreateDeadCells();
                
                List<Cell> generation = cells.ToList<Cell>();

                for (int itr = 0; itr < generation.Count; itr++)
                {
                    var cell = generation[itr];

                    List<Cell> adjacentCells = GetAdjacentCells(cell.gameObject.transform.position);

                    List<Cell> liveCells = GetAdjacentLiveCells(adjacentCells);
                    // Debug.Log($"{cell.gameObject.transform.position.ToVector2()} Adjacent Live Cells : {liveCells.Count}");

                    List<Cell> deadCells = GetAdjacentDeadCells(adjacentCells);
                    // Debug.Log($"{cell.gameObject.transform.position.ToVector2()} Adjacent Dead Cells : {deadCells.Count}");

                    if (cell.status == Cell.Status.Live)
                    {
                        // Any live cell with fewer than two live neighbours dies, as if by underpopulation
                        // Any live cell with two or three live neighbours lives on to the next generation
                        // Any live cell with more than three live neighbours dies, as if by overpopulation
                        if ((liveCells.Count < 2) || (liveCells.Count > 3))
                        {
                            // cell.gameObject.GetComponent<MeshRenderer>().material = deadCellMaterial;
                            // cell.status = Cell.Status.Dead;
                            cell.pendingStatus = Cell.Status.Dead;
                        }
                    }
                    else if (cell.status == Cell.Status.Dead)
                    {
                        // Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction
                        if (liveCells.Count == 3)
                        {
                            // cell.gameObject.GetComponent<MeshRenderer>().material = liveCellMaterial;
                            // cell.status = Cell.Status.Live;
                            cell.pendingStatus = Cell.Status.Live;
                        }
                        else
                        {
                            // generation.RemoveAt(itr);
                            // Destroy(cell.gameObject);
                            // cell.pendingStatus = Cell.Status.Destroyed;
                        }
                    }
                }

                cells = generation;

                yield return new WaitForSeconds(intervalDelaySec);
            }
        }
    }
}