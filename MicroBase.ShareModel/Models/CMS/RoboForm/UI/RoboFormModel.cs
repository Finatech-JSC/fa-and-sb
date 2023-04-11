using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MicroBase.Share.Models.CMS.RoboForm.UI
{
    public class RoboFormModel
    {
        public string Title { get; set; }

        public string IndexFormTitle { get; set; }

        public string FormSubmitUrl { get; set; }

        public string BaseRoute { get; set; }

        public RoboFormType RoboFormType { get; set; }

        public FormActionType FormActionType { get; set; }

        public RoboButton CreateButton { get; set; }

        public RoboGirdModel RoboGirdModel { get; set; }

        public Dictionary<string, IEnumerable<SelectListItem>> AutoCompleteDropdowns = new Dictionary<string, IEnumerable<SelectListItem>>();

        public void RegisterDropdownDataSource<TModel, TProperty>(Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> source,
            NameValueModel<string> firstOption = null)
        {
            if (firstOption != null)
            {
                source.ToList().Insert(0, new SelectListItem
                {
                    Text = firstOption.Name,
                    Value = firstOption.Value
                });
            }

            var memberExpression = (MemberExpression)expression.Body;
            string field = memberExpression.Member.Name;

            if (!AutoCompleteDropdowns.ContainsKey(field))
            {
                AutoCompleteDropdowns.Add(field, source);
            }
            else
            {
                AutoCompleteDropdowns[field] = source;
            }
        }
    }
}