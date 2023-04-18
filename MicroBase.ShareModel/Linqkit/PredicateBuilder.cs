using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MicroBase.Share.Linqkit
{
    /// <summary>
    /// Enables the efficient, dynamic composition of query predicates.
    /// </summary>
    public static class PredicateBuilder
    {
        /// <summary>
        /// Creates a predicate that evaluates to true.
        /// </summary>
        public static Expression<Func<T, bool>> True<T>()
        {
            return param => true;
        }

        /// <summary>
        /// Creates a predicate that evaluates to false.
        /// </summary>
        public static Expression<Func<T, bool>> False<T>()
        {
            return param => false;
        }

        /// <summary>
        /// Creates a predicate expression from the specified lambda expression.
        /// </summary>
        public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> predicate)
        {
            return predicate;
        }

        /// <summary>
        /// Build dynamic expression from SearchTermModel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="searchTerms"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> CombineFromDynamicTerm<T>(Expression<Func<T, bool>> predicate,
            IEnumerable<SearchTermModel> searchTerms)
        {
            var objectInstance = (T)Activator.CreateInstance(typeof(T));
            var targetObject = Expression.Parameter(typeof(T));

            foreach (var searchTerm in searchTerms)
            {
                // Column name in the table
                var memberExpression = Expression.PropertyOrField(targetObject, searchTerm.FieldName);

                var field = objectInstance.GetType().GetProperty(searchTerm.FieldName);
                if (field == null)
                {
                    continue;
                }

                // Value to compare
                Expression buildExpression = null;
                dynamic fieldVal = null;
                Type type = null;

                if (field.PropertyType == typeof(DateTime) || field.PropertyType == typeof(DateTime?))
                {
                    fieldVal = Convert.ToDateTime(searchTerm.FieldValue);
                }
                else if (field.PropertyType == typeof(int) || field.PropertyType == typeof(int?))
                {
                    var isSuccess = int.TryParse(searchTerm.FieldValue, out var val);
                    if (isSuccess)
                    {
                        fieldVal = val;
                    }
                }
                else if (field.PropertyType == typeof(float) || field.PropertyType == typeof(float?))
                {
                    var isSuccess = float.TryParse(searchTerm.FieldValue, out var val);
                    if (isSuccess)
                    {
                        fieldVal = val;
                    }
                }
                else if (field.PropertyType == typeof(decimal) || field.PropertyType == typeof(decimal?))
                {
                    var isSuccess = decimal.TryParse(searchTerm.FieldValue, out var val);
                    if (isSuccess)
                    {
                        fieldVal = val;
                    }
                }
                else if (field.PropertyType == typeof(bool) || field.PropertyType == typeof(bool?))
                {
                    var isSuccess = bool.TryParse(searchTerm.FieldValue, out var val);
                    if (isSuccess)
                    {
                        fieldVal = val;
                    }
                }
                else if (field.PropertyType == typeof(Guid) || field.PropertyType == typeof(Guid?))
                {
                    var isSuccess = Guid.TryParse(searchTerm.FieldValue, out var val);
                    if (isSuccess)
                    {
                        fieldVal = val;
                    }
                }
                else
                {
                    fieldVal = searchTerm.FieldValue;
                }

                if (fieldVal == null)
                {
                    continue;
                }

                if (searchTerm.Condition == Constants.SearchConstants.SearchCondition.EQUAL.ToString())
                {
                    buildExpression = Expression.Constant(fieldVal);
                }
                else if (searchTerm.Condition == Constants.SearchConstants.SearchCondition.GREATER_THAN.ToString())
                {
                    buildExpression = Expression.GreaterThan(memberExpression, Expression.Constant(fieldVal));
                }
                else if (searchTerm.Condition == Constants.SearchConstants.SearchCondition.GREATER_THAN_OR_EQUAL.ToString())
                {
                    buildExpression = Expression.GreaterThanOrEqual(memberExpression, Expression.Constant(fieldVal));
                }
                else if (searchTerm.Condition == Constants.SearchConstants.SearchCondition.LESS_THAN.ToString())
                {
                    buildExpression = Expression.LessThan(memberExpression, Expression.Constant(fieldVal));
                }
                else if (searchTerm.Condition == Constants.SearchConstants.SearchCondition.LESS_THAN_OR_EQUAL.ToString())
                {
                    buildExpression = Expression.LessThanOrEqual(memberExpression, Expression.Constant(fieldVal));
                }

                var binaryExpression = Expression.Equal(memberExpression, buildExpression);
                var lambda = Expression.Lambda<Func<T, bool>>(binaryExpression, targetObject);

                predicate = predicate.And(lambda);
            }

            return predicate;
        }

        /// <summary>
        /// Combines the first predicate with the second using the logical "and".
        /// </summary>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }

        /// <summary>
        /// Combines the first predicate with the second using the logical "or".
        /// </summary>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }

        /// <summary>
        /// Negates the predicate.
        /// </summary>
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
        {
            var negated = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(negated, expression.Parameters);
        }

        /// <summary>
        /// Combines the first expression with the second using the specified merge function.
        /// </summary>
        private static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // zip parameters (map from parameters of second to parameters of first)
            var map = first.Parameters
                .Select((f, i) => new { f, s = second.Parameters[i] })
                .ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with the parameters in the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // create a merged lambda expression with parameters from the first expression
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        private class ParameterRebinder : ExpressionVisitor
        {
            private readonly Dictionary<ParameterExpression, ParameterExpression> map;

            private ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
            {
                this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
            }

            public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
            {
                return new ParameterRebinder(map).Visit(exp);
            }

            protected override Expression VisitParameter(ParameterExpression p)
            {
                ParameterExpression replacement;

                if (map.TryGetValue(p, out replacement))
                {
                    p = replacement;
                }

                return base.VisitParameter(p);
            }
        }
    }
}