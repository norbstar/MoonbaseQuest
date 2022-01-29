using System.Collections;

namespace FX
{
    public class BounceFX : BaseFX
    {
        public override IEnumerator Apply(object config = null)
        {
            while (true)
            {
                OnApply(null);
                yield return null;
            }
        }

        protected override void OnApply(object arg1, object arg2 = null)
        {
            // TODO
        }
    }
}