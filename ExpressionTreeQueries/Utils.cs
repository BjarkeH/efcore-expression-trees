using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Newtonsoft.Json;

namespace ExpressionTreeQueries
{
    public static class Printer
    {
        private static JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented
        };


        public static void Print(Object input)
        {
            Console.WriteLine(JsonConvert.SerializeObject(input, settings));
        }

        public static void Print(string input)
        {
            Console.WriteLine(input);
        }
    }


    public class ExpressionTreeBuilder<T>
    {

        public Expression<Func<T, bool>> GetDynamicQueryWithExpressionTrees(string propertyName, string filterValue, string operand)
        {
            // x => 
            var param = Expression.Parameter(typeof(T), "x");

            
            MemberExpression member = Expression.Property(param, propertyName);
            UnaryExpression valueExpression = GetValueExpression(propertyName, filterValue, param);

            Expression body;

            switch (operand)
            {
                case ">":
                    body = Expression.GreaterThan(member, valueExpression);
                    break;
                case ">=":
                    body = Expression.GreaterThanOrEqual(member, valueExpression);
                    break;
                case "<":
                    body = Expression.LessThan(member, valueExpression);
                    break;
                case "<=":
                    body = Expression.LessThanOrEqual(member, valueExpression);
                    break;
                case "==":
                    body = Expression.Equal(member, valueExpression);
                    break;
                case "!=":
                    body = Expression.NotEqual(member, valueExpression);
                    break;
                default:
                    throw new ArgumentException(nameof(operand));
            }

            var final = Expression.Lambda<Func<T, bool>>(body: body, parameters: param);
            return final;

        }


        private UnaryExpression GetValueExpression(string propertyName, string filterValue, ParameterExpression param)
        {
            var member = Expression.Property(param, propertyName);
            var propertyType = ((PropertyInfo)member.Member).PropertyType;
            var converter = TypeDescriptor.GetConverter(propertyType);

            if (!converter.CanConvertFrom(typeof(string)))
                throw new NotSupportedException(nameof(propertyName));

            var propertyValue = converter.ConvertFromInvariantString(filterValue);
            var constant = Expression.Constant(propertyValue);
            return Expression.Convert(constant, propertyType);
        }
    }


    public static class IQueryableExtensions
    {
        // Stackoverflow article - https://stackoverflow.com/questions/37527783/get-sql-code-from-an-entity-framework-core-iqueryablet, Answer from Kiesewetter
        public static string ToSql<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        {
            var enumerator = query.Provider.Execute<IEnumerable<TEntity>>(query.Expression).GetEnumerator();
            var relationalCommandCache = enumerator.Private("_relationalCommandCache");
            var selectExpression = relationalCommandCache.Private<SelectExpression>("_selectExpression");
            var factory = relationalCommandCache.Private<IQuerySqlGeneratorFactory>("_querySqlGeneratorFactory");

            var sqlGenerator = factory.Create();
            var command = sqlGenerator.GetCommand(selectExpression);

            string sql = command.CommandText;
            return sql;
        }

        private static object Private(this object obj, string privateField) => obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
        private static T Private<T>(this object obj, string privateField) => (T)obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
    }
}
