using UnityButton = UnityEngine.UI.Button;

namespace Chess
{
    public class PlayModePanelUIManager : ButtonGroupPanelUIManager
    {
        public enum Identity
        {
            PVP,
            PVB,
            BVB
        }
        
        public delegate void OnClickEvent(Identity identity);
        public static event OnClickEvent EventReceived;

        public override void OnClickButton(UnityButton button)
        {
            base.OnClickButton(button);

            var name = button.name;

            if (name.Equals("PVP Button"))
            {
                EventReceived?.Invoke(Identity.PVP);
            }
            else if (name.Equals("PVB Button"))
            {
                EventReceived?.Invoke(Identity.PVB);
            }
            else if (name.Equals("BVB Button"))
            {
                EventReceived?.Invoke(Identity.BVB);
            }
        }
    }
}