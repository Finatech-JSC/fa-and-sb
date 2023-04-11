using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MicroBase.Share.Extensions
{
    public static class HttpExtensions
    {
        public static TModel IFormCollectionToModel<TModel>(IFormCollection formCollection, TModel model) where TModel : new()
        {
            try
            {
                if (formCollection == null)
                {
                    return new TModel();
                }

                StringBuilder content = new StringBuilder();
                content.Append("{");

                var properties = model.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    if (!formCollection.ContainsKey(property.Name))
                    {
                        continue;
                    }

                    var value = formCollection[property.Name];
                    if (property.PropertyType == typeof(byte))
                    {
                        property.SetValue(model, byte.Parse(value));
                    }
                    else if (property.PropertyType == typeof(sbyte))
                    {
                        property.SetValue(model, sbyte.Parse(value));
                    }
                    else if (property.PropertyType == typeof(UInt16))
                    {
                        property.SetValue(model, UInt16.Parse(value));
                    }
                    else if (property.PropertyType == typeof(UInt32))
                    {
                        property.SetValue(model, UInt32.Parse(value));
                    }
                    else if (property.PropertyType == typeof(UInt64))
                    {
                        property.SetValue(model, UInt64.Parse(value));
                    }
                    else if (property.PropertyType == typeof(Int16))
                    {
                        property.SetValue(model, Int16.Parse(value));
                    }
                    else if (property.PropertyType == typeof(Int32))
                    {
                        property.SetValue(model, Int32.Parse(value));
                    }
                    else if (property.PropertyType == typeof(Int64))
                    {
                        property.SetValue(model, Int64.Parse(value));
                    }
                    else if (property.PropertyType == typeof(Decimal))
                    {
                        property.SetValue(model, Decimal.Parse(value));
                    }
                    else if (property.PropertyType == typeof(Double))
                    {
                        property.SetValue(model, Double.Parse(value));
                    }
                    else if (property.PropertyType == typeof(Single))
                    {
                        property.SetValue(model, Single.Parse(value));
                    }
                    else if (property.PropertyType == typeof(bool))
                    {
                        var bValue = value.ToString().Split(",").FirstOrDefault();
                        property.SetValue(model, bool.Parse(bValue));
                    }
                    else if (property.PropertyType == typeof(Boolean))
                    {
                        property.SetValue(model, Boolean.Parse(value));
                    }
                    else if (property.PropertyType == typeof(DateTime))
                    {
                        property.SetValue(model, DateTime.Parse(value));
                    }
                    else if (property.PropertyType == typeof(DateTime?))
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            property.SetValue(model, DateTime.Parse(value));
                        }
                        else
                        {
                            property.SetValue(model, null);
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(value))
                    {
                        property.SetValue(model, value.ToString());
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                return new TModel();
            }
        }
    }
}