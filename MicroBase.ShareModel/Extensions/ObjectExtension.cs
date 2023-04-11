using Newtonsoft.Json;
using System;
using System.Linq.Expressions;

namespace MicroBase.Share.Extensions
{
    public static class ObjectExtension
    {
        public static object GetPropValue(this object src, string propName)
        {
            if (src.GetType().GetProperty(propName) == null)
            {
                return string.Empty;
            }

            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        public static string GetFieldName<T>(Expression<Func<T, object>> expression)
        {
            var member = expression.Body as MemberExpression;
            if (member != null)
                return member.Member.Name;

            return string.Empty;
        }

        public static string GetRoboFieldName<T>(Expression<Func<T, object>> expression)
        {
            var body = expression.Body;
            if (body.NodeType == ExpressionType.Convert)
            {
                body = ((UnaryExpression)body).Operand;
            }

            var member = body as MemberExpression;
            if (member != null)
                return member.Member.Name.FirstCharToLowwer();

            return string.Empty;
        }

        public static T Clone<T>(this T source)
        {
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            var deserializeSettings = new JsonSerializerSettings
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace
            };
            
            var serializeSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source, serializeSettings), deserializeSettings);
        }
    }
}