using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Conways
{
    public class ConwaysGame : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] GameObject cellPrefab;

        [Header("Config")]
        [SerializeField] float intervalDelaySec = 0.5f;

        private List<GameObject> cells;
        private Coroutine coroutine;

        void Awake() => cells = new List<GameObject>();

        public void Init(List<ConwaysCellUIManager> cells)
        {
            for (int itr = 0; itr < (cells.Count); itr++)
            {
                var cell = cells[itr];

                if (cell.IsSelected)
                {
                    // var instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    // instance.transform.localScale = Vector3.one;
                    // instance.transform.position = new Vector3(cell.Coord.x, 0.5f, cell.Coord.y);

                    var instance = GameObject.Instantiate(cellPrefab, new Vector3(cell.Coord.x, 0.5f, cell.Coord.y), Quaternion.identity, transform);
                    this.cells.Add(instance);
                }
            }

            DestroyCell(0, 0);

            coroutine = StartCoroutine(StartGameCoroutine());
        }

        private IEnumerator StartGameCoroutine()
        {
            // RULES OF THE GAME
            // Any live cell with two or three live neighbours survives.
            // Any dead cell with three live neighbours becomes a live cell.
            // All other live cells die in the next generation. Similarly, all other dead cells stay dead.

            while (true)
            {
                yield return new WaitForSeconds(intervalDelaySec);
            }
        }

        public void DestroyCell(int x, int y) =>  DestroyCell(new Vector2(x, y));

        public void DestroyCell(Vector2 position)
        {
            for (int itr = 0; itr < (cells.Count); itr++)
            {
                var cell = cells[itr].gameObject;

                if ((cell.transform.position.x == position.x) && (cell.transform.position.z == position.y))
                {
                    Destroy(cell.gameObject);
                }
            }
        }
    }
}