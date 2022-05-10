namespace Chess
{
    public class StageManager
    {
        public enum Stage
        {
            Evaluating,
            PendingSelect,
            Selected,
            Moving,
            MoveComplete,
            Promoting,
            Resetting
        }
        
        private Stage stage;

        public Stage LiveStage
        {
            get
            {
                return stage;
            }

            set
            {
                stage = value;
            }
        }
    }
}