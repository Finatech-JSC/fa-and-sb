using System.Collections.Generic;

namespace MicroBase.Share.Models.CMS.RoboForm.UI
{
    public class RoboButton
    {
        public string Name { get; set; }

        public string Label { get; set; }

        public string ExtraClass { get; set; }

        public ButtonAction Action { get; set; }

        public string Link { get; set; }

        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();

        public GirdPagePosition Position { get; set; }

        public ButtonIcon ButtonIcon { get; set; }

        public int Order { get; set; }

        public bool Disabled { get; set; }

        public ButtonType ButtonType { get; set; }

        public void SetSubmitConfirmMsg(string msg)
        {
            if (Parameters == null)
            {
                Parameters = new Dictionary<string, string>();
            }

            if (Parameters.ContainsKey(ButtonParameters.CONFIRM_AND_SUBMIT_MESSAGE))
            {
                Parameters[ButtonParameters.CONFIRM_AND_SUBMIT_MESSAGE] = msg;
            }
            else
            {
                Parameters.Add(ButtonParameters.CONFIRM_AND_SUBMIT_MESSAGE, msg);
            }
        }
    }

    public class ButtonIcon
    {
        public ButtonStyle ButtonStyle { get; set; }

        public string Fontawesome { get; set; }

        public string BackgroundColor { get; set; }
    }

    public static class ButtonParameters
    {
        public static string CONFIRM_AND_SUBMIT_MESSAGE = "CONFIRM_AND_SUBMIT_MESSAGE";
    }
}