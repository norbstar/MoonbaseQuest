namespace Chess
{
    public class ConfirmationPanelUIManager : BasePanelUIManager
    {
        public enum Button
        {
            Yes,
            No
        }

        public delegate void OnClickEvent(Button button);
        public static event OnClickEvent EventReceived;

        public override void OnClickButton()
        {
            base.OnClickButton();

            if (selectedButton.name.Equals("No Button"))
            {
                EventReceived?.Invoke(Button.No);
            }
            else if (selectedButton.name.Equals("Yes Button"))
            {
                EventReceived?.Invoke(Button.Yes);
            }
        }
    }
}