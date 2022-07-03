using UnityEngine;

namespace Chess
{
    public class SettingsManager : CachedObject<SettingsManager>
    {
        public static class Interaction
        {
            public enum Mode
            {
                Remote,
                Direct
            }
        }

        [SerializeField] Interaction.Mode interactionMode;

        public Interaction.Mode InteractionMode
        {
            get
            {
                return interactionMode;
            }

            set
            {
                interactionMode = value;
            }
        }

        public void OnInteractionModeChange(int index)
        {
            switch (index)
            {
                case (int) Interaction.Mode.Remote:
                    interactionMode = Interaction.Mode.Remote;
                    break;

                case (int) Interaction.Mode.Direct:
                    interactionMode = Interaction.Mode.Direct;
                    break;
            }
        }
    }
}