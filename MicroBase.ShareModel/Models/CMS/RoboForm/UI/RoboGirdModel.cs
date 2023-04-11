using System;
using System.Collections.Generic;

namespace MicroBase.Share.Models.CMS.RoboForm.UI
{
    public class RoboGirdModel
    {
        public int RecordPerPage { get; set; } = 20;

        public List<RoboGirdField> RoboGirdFields { get; set; }

        public string FetchDataUrl { get; set; }

        public RoboButton CreateButton { get; set; }

        public RoboButton EditButton { get; set; }
        
        public RoboButton ViewButton { get; set; }

        public RoboButton RedirectButton { get; set; }

        public string ActiveUrl { get; set; }

        public string DeleteUrl { get; set; }

        public GridHeader GridHeader { get; set; }

        public List<MobileField> ShowMobileFields { get; set; }
    }

    public class MobileField
    {
        public string Title { get; set; }

        public string FieldName { get; set; }

        public bool IsHyperLink { get; set; }

        public string HyperLink { get; set; }

        public List<string> FieldParameters { get; set; }

        public string FancyboxType { get; set; }
    }

    public enum RoboGirdFieldType
    {
        Text,
        Boolean,
        Image,
        DateTime,
        HyperLink,
        Fancybox,
        Number
    }

    public enum TextAlign
    {
        Undefined,
        Left,
        Right,
        Center
    }

    public static class RoboFancyboxType
    {
        public static string Small = "iframe-fancybox";
        public static string Large = "iframe-fancybox-large";
        public static string XLarge = "iframe-fancybox-xlarge";
    }

    public class RoboGirdField
    {
        public string FieldName { get; set; }

        public string Title { get; set; }

        public int Width { get; set; }

        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string> HeaderAttributes { get; set; } = new Dictionary<string, string>();

        public bool Filterable { get; set; }

        public RoboGirdFieldType RoboGirdFieldType { get; set; }

        public string HyperLink { get; set; }

        public string HyperLinkKendo { get; set; }

        public bool OpenLinkInNewTab { get; set; }

        public string TextLink { get; set; }

        public List<QueryStringModel> FieldParameters { get; set; }

        public string FancyboxType { get; set; }

        public TextAlign TextAlign { get; set; }
    }

    public class GridHeaderField
    {
        public string Name { get; set; }

        public string Label { get; set; }

        public int Col { get; set; }

        public string ExtraClass { get; set; }

        public RoboTextType RoboTextType { get; set; }

        public string PlaceHolder { get; set; }

        public List<NameValueModel<string>> DropDownDataSource { get; set; }

        public RoboDateRangePickerModel DateRangePickerModel { get; set; }
    }

    public class GridHeader
    {
        public List<GridHeaderField> GridHeaderFields { get; set; }

        public List<RoboButton> RoboButtons { get; set; }
    }

    public class QueryStringModel
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public QueryType Type { get; set; }
    }

    public enum QueryType
    {
        QueryString,
        Param
    }

    public class RoboDateRangePickerModel
    {
        public string FromDateFieldName { get; set; }

        public string ToDateFieldName { get; set; }
    }
}