using System.Collections.Generic;

namespace MicroBase.Share.Models.CMS.RoboForm.UI
{
    public enum RoboTextType : byte
    {
        TextBox,
        Password,
        Email,
        Url,
        MultiText,
        RichText,
        FileUpload,
        DatePicker,
        DateTimePicker,
        DateRangePicker,
        Dropdown,
        HyperLink,
        Table,
        Buttons
    }

    public enum RoboFormType : byte
    {
        SubmitIFrame,
        MainSubmitForm,
        GirdForm,
        SetupSetting
    }

    public enum RoboGroupPosition : byte
    {
        Left,
        Right
    }

    public enum ButtonAction
    {
        Submit,
        OpenIframe,
        OpenIframeLarge,
        OpenIframeXLarge,
        OpenNewForm,
        OpenNewTab,
        GridSearch,
        ExportCsv,
        ActionAjax
    }

    public enum GirdPagePosition
    {
        TopHeader,
        FilterSection
    }

    public enum ButtonStyle
    {
        Custom,
        ExportFile,
        ImportFile,
        Search
    }

    public enum ButtonType
    {
        Submit,
        ConfirmAndSubmit,
        Button
    }

    public enum FormActionType
    {
        Nonce,
        ViewOnly,
        Edit,
        Create
    }

    public static class RoboDataType
    {
        public const string STRING = "STRING";

        public const string INT = "INT";

        public const string DECIMAL = "DECIMAL";

        public const string DATE_TIME = "DATE_TIME";

        public const string BOOLEAN = "BOOLEAN";

        public const string ARRAY_STRING = "ARRAY_STRING";

        public const string ARRAY_INT = "ARRAY_INT";

        public const string ARRAY_DECIMAL = "ARRAY_DECIMAL";

        public const string ARRAY_DATE_TIME = "ARRAY_DATE_TIME";

        public const string ARRAY_BOOLEAN = "ARRAY_BOOLEAN";

        public const string ARRAY_OBJECT = "ARRAY_OBJECT";

        public static List<NameValueModel<string>> Maps = new List<NameValueModel<string>>
        {
            new NameValueModel<string> {Name = "STRING", Value = "String (character)" },
            new NameValueModel<string> {Name = "INT", Value = "Integer" },
            new NameValueModel<string> {Name = "DECIMAL", Value = "Decimal" },
            new NameValueModel<string> {Name = "DATE_TIME", Value = "Date time" },
            new NameValueModel<string> {Name = "BOOLEAN", Value = "Boolean" },
            new NameValueModel<string> {Name = "ARRAY_STRING", Value = "Array String" },
            new NameValueModel<string> {Name = "ARRAY_INT", Value = "Array Integer" },
            new NameValueModel<string> {Name = "ARRAY_DECIMAL", Value = "Array Decimal" },
            new NameValueModel<string> {Name = "ARRAY_DATE_TIME", Value = "Array Date Time" },
            new NameValueModel<string> {Name = "ARRAY_BOOLEAN", Value = "Array Boolean" },
            new NameValueModel<string> {Name = "ARRAY_OBJECT", Value = "Array Object" }
        };
    }
}