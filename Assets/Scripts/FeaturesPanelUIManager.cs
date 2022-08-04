namespace Chess
{
    public class FeaturesPanelUIManager : ToggleGroupPanelUIManager
    {
        public enum Identity
        {
            VFX,
            SFX,
            Haptics
        }

        public delegate void OnCheckEvent(Identity identity);
        public static event OnCheckEvent EventReceived;
    }
}