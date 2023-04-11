using MicroBase.Share.Models.CMS.RoboForm.UI;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MicroBase.Share.Models.SiteSettings
{
    public class SiteSettingModel : BaseModel
    {
        [RoboText(LabelText = "Group Key", Name = "GroupKey", MaxLength = 255, Cols = 3, Order = 1)]
        public string GroupKey { get; set; }

        [RoboText(LabelText = "Key", Name = "Key", IsRequired = true, MaxLength = 255, Cols = 3, Order = 2)]
        public string Key { get; set; }

        [RoboText(LabelText = "Number Value", Name = "NumberValue", Cols = 3, Order = 3)]
        public decimal? NumberValue { get; set; }

        [RoboCheckbox(LabelText = "Bool Value", Name = "BoolValue", CheckboxLabel = "true|false", Cols = 3, Order = 4)]
        public bool? BoolValue { get; set; }

        [RoboText(Type = RoboTextType.RichText, LabelText = "Rich Text (html)", Name = "RichText", Cols = 12, Order = 5)]
        public string RichText { get; set; }

        [RoboText(Type = RoboTextType.MultiText, LabelText = "Raw Text (raw)", Name = "RawText", Cols = 12, Order = 6)]
        public string RawText { get; set; }

        [RoboText(LabelText = "Description", Name = "Description", MaxLength = 255, Cols = 6, Order = 7)]
        public string Description { get; set; }

        [RoboText(LabelText = "Order", Name = "Order", Cols = 6, Order = 8)]
        public int Order { get; set; }

        [RoboCheckbox(LabelText = "Is Secret", Name = "IsSecret", Cols = 6, Order = 9)]
        public bool IsSecret { get; set; }
    }

    public class SiteSettingBuilderModel : BaseModel
    {
        public string SettingKey { get; set; }

        public bool ModelIsArray { get; set; }

        public string Description { get; set; }

        public bool IsSecret { get; set; }
        
        public List<ModelField> ModelFields { get; set; }
    }

    public class ModelField
    {
        [RoboText(LabelText = "Field Name", Name = "FieldName", IsRequired = true, MaxLength = 255, Cols = 3, Order = 1)]
        public string FieldName { get; set; }
        
        [RoboText(LabelText = "Type", Name = "Type", IsRequired = true, MaxLength = 255, Cols = 3, Order = 2)]
        public string Type { get; set; }
        
        [RoboText(LabelText = "Value", Name = "Value", IsRequired = true, MaxLength = 255, Cols = 3, Order = 3)]
        public string Value { get; set; }

        public List<SubModelField> SubModelFields { get; set; }
    }

    public class SubModelField
    {
        public string FieldName { get; set; }
        
        public string Type { get; set; }

        public string Value { get; set; }
    }
}