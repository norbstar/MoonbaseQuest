namespace Chess
{
    public class PlayAsPanelUIManager : ToggleGroupPanelUIManager
    {
        public enum Identity
        {
            Light,
            Dark
        }

        public delegate void OnCheckEvent(Identity identity);
        public static event OnCheckEvent EventReceived;
    }
}