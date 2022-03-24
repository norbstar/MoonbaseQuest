using System.Collections.Generic;

namespace Interactables.Gun
{
    public class HUDsManager
    {
        private List<bool> hudTrackers;
        private int activeIdx;

        public HUDsManager(List<Interactables.Gun.HUDManager> hudManagers)
        {
            hudTrackers = new List<bool>();

            foreach (Interactables.Gun.HUDManager manager in hudManagers)
            {
                bool isPrimaryHUD = manager.GetType().IsAssignableFrom(typeof(Interactables.Gun.PrimaryHUDManager));
                hudTrackers.Add(isPrimaryHUD);
            }
        }

        public void SetActive(int idx) => hudTrackers[idx] = true;

        public void SetInactive(int idx) => hudTrackers[idx] = false;

        public List<bool> HUDTrackers { get { return hudTrackers; } }

        public int ActiveIdx { get { return activeIdx; } set { activeIdx = value; } }

        private bool IsActive(int idx)
        {
            return hudTrackers[idx];
        }

        public bool TryGetPreviousIndex(out int idx)
        {
            int searchIdx = idx = activeIdx;
            bool success = false;

            do
            {
                --searchIdx;

                if (searchIdx < 0)
                {
                    searchIdx = hudTrackers.Count - 1;
                }

                if (success = (searchIdx != activeIdx) && IsActive(searchIdx))
                {
                    idx = searchIdx;
                }
            } while (!success && searchIdx != activeIdx);

            return success;
        }

        public bool TryGetNextIndex(out int idx)
        {
            int searchIdx = idx = activeIdx;
            bool success = false;

            do
            {
                ++searchIdx;

                if (searchIdx > hudTrackers.Count - 1)
                {
                    searchIdx = 0;
                }

                if (success = (searchIdx != activeIdx) && IsActive(searchIdx))
                {
                    idx = searchIdx;
                }
            } while (!success && searchIdx != activeIdx);

            return success;
        }
    }
}