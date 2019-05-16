namespace Emigre.Data
{
    using System;
    using ObjectEditor.Json;

    [Category("Text")]
    public class TextEvent : SpeakingEvent, ICustomScriptable
    {

        [FieldTag(FieldTags.Multiline)]
        public string text = "";
        public bool closeOnClick = true;

        public TextEvent() { }

        public TextEvent(string text)
        {
            this.text = text;
        }

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            text = fields.add(text, "text");
            closeOnClick = fields.add(closeOnClick, "closeOnClick");
        }

        public override string ToString()
        {
            return "\"" + text + "\"";
        }

        public override Action GetTransitionAction()
        {
            if (!closeOnClick) return Action.Immediately;
            return base.GetTransitionAction();
        }

        public string Write()
        {
            return "**" + (speaker.IsNull ? "Narration" : speaker.ToString()) + "**: " + text;
        }

        public void Read(string data)
        {
            int speakerIndex = data.IndexOf(": ");
            if (speakerIndex < 0)
            {
                text = data;
                return;
            }

            string content = data.Substring(speakerIndex + 2);
            text = content;

            string speakerName = data.Substring(0, speakerIndex);
            speakerName = speakerName.Replace("*", "");
            if (speakerName != "Narration" && speakerName != speaker.ToString())
            {
                Console.WriteLine("Speaker change: " + speaker + " => " + speakerName);
            }

        }

        public string Indent()
        {
            return null;
        }
    }
}