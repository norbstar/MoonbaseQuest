    using UnityEngine;

    namespace Chess
    {
        public class Cell
        {
            public Coord coord;
            public Vector3 localPosition;
            public PieceManagerWrapper wrapper;

            public bool IsOccupied { get { return wrapper.manager != null; } }

            public Cell Clone()
            {
                return new Cell
                {
                    coord = new Coord
                    {
                        x = coord.x,
                        y = coord.y
                    },
                    localPosition = new Vector3(localPosition.x, localPosition.y, localPosition.z),
                    wrapper = new PieceManagerWrapper
                    {
                        manager = wrapper.manager
                    }
                };
            }
        }
    }