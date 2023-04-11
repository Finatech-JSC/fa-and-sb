using System;

namespace MicroBase.Share.Models.CMS.RoboForm.UI
{
    public class RoboControlAttribute : Attribute
    {
        public bool IsReadOnly { get; set; }

        public bool IsRequired { get; set; }

        public string LabelCssClass { get; set; }

        public string LabelText { get; set; }

        public string Name { get; set; }

        public short Order { get; set; }

        public bool IsHidden { get; set; }

        public int Cols { get; set; }

        public bool IsBreakLine { get; set; }

        public string MessageValidate { get; set; }

        /// <summary>
        /// The fields has same GroupTitle will be display in the same section
        /// </summary>
        public string GroupTitle { get; set; }

        public RoboGroupPosition GroupPosition { get; set; }

        public int GroupCol { get; set; }

        public RoboGirdFieldType FieldType { get; set; }

        public int Width { get; set; }

        public bool DisableOnEdit { get; set; }

        public RoboTextType Type { get; set; }
    }
}