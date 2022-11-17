using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace MudBlazor.Forms
{
    public enum ModelFormStyle
    {
        Normal, Compact
    }

    public static class MudModelFormTools
    {
        public static Expression<Func<TS>> GetExpression<TS>(object instance, PropertyInfo propertyInfo)
        {
            var constant = Expression.Constant(instance);
            //var asDeclardType = Expression.Convert(constant, propertyInfo.DeclaringType);
            var memberExpression = Expression.Property(constant, propertyInfo);
            return Expression.Lambda<Func<TS>>(memberExpression);
        }

        public static Expression GetExpressionObject(object instance, PropertyInfo propertyInfo)
        {
            var constant = Expression.Constant(instance);
            //var asDeclardType = Expression.Convert(constant, propertyInfo.DeclaringType);
            var memberExpression = Expression.Property(constant, propertyInfo);
            return Expression.Lambda(memberExpression);
        }

        public static bool IsNullable(PropertyInfo propertyInfo)
        {
            return IsNullable(propertyInfo.PropertyType);
        }

        public static string? GetStringFormat(this PropertyInfo propertyInfo)
        {
            return Attribute.IsDefined(propertyInfo, typeof(DisplayFormatAttribute))
                ? (Attribute.GetCustomAttribute(propertyInfo, typeof(DisplayFormatAttribute)) as DisplayFormatAttribute)?.DataFormatString
                : null;
        }

        public static bool IsEditable(this PropertyInfo propertyInfo)
        {
            return !Attribute.IsDefined(propertyInfo, typeof(EditableAttribute)) 
                   || ((Attribute.GetCustomAttribute(propertyInfo, typeof(EditableAttribute)) as EditableAttribute)!).AllowEdit;
        }

        public static IEnumerable<string>? DropdownValues(this PropertyInfo propertyInfo)
        {
            return (Attribute.GetCustomAttribute(propertyInfo, typeof(MudFormAttribute)) as MudFormAttribute)?.ValidValues;
        }

        public static bool IsNullable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static Type? GetNonNullableType(PropertyInfo prop)
        {
            var type = prop.PropertyType;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return Nullable.GetUnderlyingType(type);
            }
            return type;
        }

        public static object GetNonNullableValue(PropertyInfo prop, object instance)
        {
            var type = prop.PropertyType;
            var value = prop.GetValue(instance);
            if (value != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var underlyingType = Nullable.GetUnderlyingType(type);
                return Convert.ChangeType(value, underlyingType);
            }
            return value;
        }

        public static T? AsNullableValue<T>(PropertyInfo prop, object instance) where T : struct
        {
            if (IsNullable(prop))
            {
                return prop.GetValue(instance) as T?;
            }

            return (T)prop.GetValue(instance);
        }

        public static bool IsRequired(PropertyInfo propertyInfo)
        {
            return Attribute.IsDefined(propertyInfo, typeof(RequiredAttribute));
        }

        public static int? GetSize(PropertyInfo propertyInfo)
        {
            if (Attribute.IsDefined(propertyInfo, typeof(StringLengthAttribute)))
            {
                var attr =
                    Attribute.GetCustomAttribute(propertyInfo, typeof(StringLengthAttribute)) as StringLengthAttribute;
                return attr?.MaximumLength;
            }

            var stringLength = Attribute.IsDefined(propertyInfo, typeof(MudFormAttribute))
                ? (Attribute.GetCustomAttribute(propertyInfo, typeof(MudFormAttribute)) as MudFormAttribute)?.StringLength
                : null;
            
            return stringLength == 0 ? null : stringLength;
        }

        public static int? GetLineCount(PropertyInfo propertyInfo)
        {
            return Attribute.IsDefined(propertyInfo, typeof(MudFormAttribute))
                ? (Attribute.GetCustomAttribute(propertyInfo, typeof(MudFormAttribute)) as MudFormAttribute)?.LineCount
                : null;
        }

        public static string? GetPlaceHolder(PropertyInfo propertyInfo)
        {
            return Attribute.IsDefined(propertyInfo, typeof(MudFormAttribute))
                ? (Attribute.GetCustomAttribute(propertyInfo, typeof(MudFormAttribute)) as MudFormAttribute)?.Placeholder
                : null;
        }

        public static bool IsPasswordField(PropertyInfo propertyInfo)
        {
            return Attribute.IsDefined(propertyInfo, typeof(MudFormAttribute)) 
                   && ((Attribute.GetCustomAttribute(propertyInfo, typeof(MudFormAttribute)) as MudFormAttribute)!).IsPasswordField;
        }

        public static string GetLabel(PropertyInfo propertyInfo, Func<string, string> labelFunc, bool includeOptional = true)
        {
            var label = Attribute.IsDefined(propertyInfo, typeof(MudFormAttribute))
                ? (Attribute.GetCustomAttribute(propertyInfo, typeof(MudFormAttribute)) as MudFormAttribute)?.Label
                : null;
            if (label is null)
            {
                if (!(labelFunc is null))
                {
                    label = labelFunc(propertyInfo.Name);
                }
                else
                {
                    label = Labelize(propertyInfo.Name);
                }
            }
            return label;
        }

        public static List<PropertyInfo> GetAeModelProperties(this Type type)
        {
            return type.GetProperties().Where(p => !Attribute.IsDefined(p, typeof(AeFormIgnoreAttribute))).ToList();
        }

        public static List<(string category, List<List<PropertyInfo>> properties)> GetMudModelFormCategories(this Type type)
        {
            var allProps = GetAeModelProperties(type);
            var propsNoCat = allProps.Where(p => !Attribute.IsDefined(p, typeof(AeFormCategoryAttribute)))
                                     .GroupBy(p => GetRow(p))
                                     .OrderBy(tp => tp.Key)
                                     .Select(g => g.OrderBy(tp => GetColumn(tp)).Select(tp => tp).ToList()).ToList();

            var result = new List<(string category, List<List<PropertyInfo>> properties)> { (null, propsNoCat) };
            result.AddRange(allProps.Where(p => Attribute.IsDefined(p, typeof(AeFormCategoryAttribute)))
                .Select(property => (((Attribute.GetCustomAttribute(property, typeof(AeFormCategoryAttribute)) as AeFormCategoryAttribute).Category,
                (Attribute.GetCustomAttribute(property, typeof(AeFormCategoryAttribute)) as AeFormCategoryAttribute).CategoryOrder),
                property))
                .GroupBy(p => p.Item1)
                .OrderBy(gp => gp.Key.CategoryOrder)
                .Select(gp => (gp.Key.Category, gp.Select(tp => tp.property)
                                                  .GroupBy(p => GetRow(p))
                                                  .OrderBy(tp => tp.Key)
                                                  .Select(g => g.OrderBy(tp => GetColumn(tp)).Select(tp => tp).ToList()).ToList())).ToList());
            return result;
        }

        private static int GetRow(PropertyInfo p)
        {
            return int.TryParse((Attribute.GetCustomAttribute(p, typeof(MudFormAttribute)) as MudFormAttribute)?.Row, out var result) ? result : 0;
        }

        private static int GetColumn(PropertyInfo p)
        {
            return int.TryParse((Attribute.GetCustomAttribute(p, typeof(MudFormAttribute)) as MudFormAttribute)?.Column, out var result) ? result : 0;
        }

        public static string Labelize(string propName)
        {
            var blocks = BreakUppercase(propName);
            blocks = blocks.SelectMany(s => BreakNumbers(s));
            if (isPascalCase(blocks))
            {
                return string.Join(" ", blocks);
            }
            if (isUnderscore(propName))
            {
                return propName.Replace('_', ' ');
            }
            return propName;
        }

        private static bool isPascalCase(IEnumerable<string> blocks)
        {
            return blocks.Any(s => s.Length > 1);
        }

        private static IEnumerable<string> BreakUppercase(string str)
        {
            return Regex.Split(str, @"(?<!^)(?=[A-Z])");
        }

        private static IEnumerable<string> BreakNumbers(string str)
        {
            return Regex.Split(str, @"(?<!^)(?=[0-9])");
        }

        private static bool isUnderscore(string name)
        {
            if (name.Contains("_"))
                return true;
            //might need more rules
            return false;
        }

        public static PropertyInfo GetPropertyInfo(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return ((MemberExpression)expression).Member as PropertyInfo ?? throw new InvalidOperationException($"Cannot extract property path from ${expression}");

                case ExpressionType.Convert:
                    return GetPropertyInfo(((UnaryExpression)expression).Operand);

                default:
                    throw new NotSupportedException(expression.NodeType.ToString());
            }
        }

        public static PropertyInfo WithPropertyExpression<T>(Expression<Func<T, object>> expression)
            => GetPropertyInfo(expression.Body);

        public static PropertyInfo WithPropertyExpression(LambdaExpression expression)
            => GetPropertyInfo(expression.Body);

        private static readonly HashSet<Type> NumericTypes = new HashSet<Type>
        {
            typeof(int),
            typeof(uint),
            typeof(double),
            typeof(decimal),
            typeof(byte),
            typeof(sbyte),
            typeof(long),
            typeof(double),
            typeof(ulong),
            typeof(decimal),
            typeof(float)
        };

        private static readonly HashSet<Type> DateTimeTypes = new HashSet<Type>
        {
            typeof(DateTime),
            typeof(DateTimeOffset)
        };

        public static bool IsNumericType(Type type)
        {
            return NumericTypes.Contains(type) ||
                   NumericTypes.Contains(Nullable.GetUnderlyingType(type));
        }

        public static bool IsDateType(Type type)
        {
            return DateTimeTypes.Contains(type) ||
                   DateTimeTypes.Contains(Nullable.GetUnderlyingType(type));
        }

        public static string GetFieldNote(IModelFormContext modelFormContext, PropertyInfo propertyInfo)
        {
            if (modelFormContext != null && !string.IsNullOrEmpty(modelFormContext.GetFieldNote(propertyInfo)))
            {
                return modelFormContext.GetFieldNote(propertyInfo);
            }
            return string.Empty;
        }
    }
}