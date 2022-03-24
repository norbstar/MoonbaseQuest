using System.Collections.Generic;
using System.Linq;

namespace Interactables.Gun
{
    public class HUDsManager
    {
        private List<int> activeHUDs;
        private int activeIdx;

        public HUDsManager()
        {
            activeHUDs = new List<int>();
        }

        public List<int> ActiveHUDs { get { return activeHUDs; } }

        public bool AddHUD(int idx)
        {
            var matches = activeHUDs.Where(i => i == idx);
            
            if (matches.Count() == 0)
            {
                activeHUDs.Add(idx);
                return true;
            }

            return false;
        }

        public bool RemoveHUD(int idx)
        {
            var matches = activeHUDs.Where(i => i == idx);
            
            if (matches.Count() == 1)
            {
                activeHUDs.Remove(idx);
                return true;
            }

            return false;
        }

        public int ActiveIdx { get { return activeIdx; } set { activeIdx = value; } }

        public bool TryGetPreviousIndex(out int idx)
        {
            UnityEngine.Debug.Log($"1 ActiveIdx : {activeIdx}");

            int searchIdx = activeIdx;
            bool success = false;

            idx = default(int);

            do
            {
                --searchIdx;
                UnityEngine.Debug.Log($"2 Searchdx : {searchIdx}");

                if (searchIdx < 0)
                {
                    searchIdx = activeHUDs.Count - 1;
                }

                UnityEngine.Debug.Log($"3 Adjusted Searchdx : {searchIdx}");

                if (searchIdx != activeIdx)
                {
                    success = true;
                    idx = activeHUDs[searchIdx];
                    UnityEngine.Debug.Log($"4 Found New Idx : {idx}");
                }
            } while (!success && searchIdx != activeIdx);

            UnityEngine.Debug.Log($"5 Success : {success} Idx : {idx}");

            return success;
        }

        public bool TryGetNextIndex(out int idx)
        {
            int searchIdx = activeIdx;
            bool success = false;

            idx = default(int);

            do
            {
                ++searchIdx;

                if (searchIdx > activeHUDs.Count - 1)
                {
                    searchIdx = 0;
                }

                if (searchIdx != activeIdx)
                {
                    success = true;
                    idx = activeHUDs[searchIdx];
                }
            } while (!success && searchIdx != activeIdx);

            return success;
        }
    }
}