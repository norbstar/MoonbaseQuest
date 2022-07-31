namespace Chess
{
    public class PlayModePanelUIManager : BasePanelUIManager
    {
        public enum Button
        {
            PVP,
            PVB,
            BVB
        }

        public delegate void OnClickEvent(Button button);
        public static event OnClickEvent EventReceived;

        public override void OnClickButton()
        {
            base.OnClickButton();

            if (selectedButton.name.Equals("PVP Button"))
            {
                EventReceived?.Invoke(Button.PVP);
            }
            else if (selectedButton.name.Equals("PVB Button"))
            {
                EventReceived?.Invoke(Button.PVB);
            }
            else if (selectedButton.name.Equals("BVB Button"))
            {
                EventReceived?.Invoke(Button.BVB);
            }
        }
    }
}