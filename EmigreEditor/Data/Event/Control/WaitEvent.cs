using ObjectEditor.Json;

namespace Emigre.Data
{
    [Category("Control")]
    public class WaitEvent : StoryEvent
    {
        public Action waitForAction;
        public float timeout;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            waitForAction = fields.addEnum(waitForAction, "waitForAction");
            timeout = fields.add(timeout, "timeout");
        }

        public override Action GetTransitionAction()
        {
            return waitForAction;
        }

        public override string ToString()
        {
            return string.Format("Wait for {0} ({1})", waitForAction, timeout);
        }
    }
}
