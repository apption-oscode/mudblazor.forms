﻿using FluentValidation;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace MudBlazor.Forms
{
    public class ModelFormArgsInternal(PropertyInfo propertyInfo)
    {
        public PropertyInfo PropertyInfo { get; set; } = propertyInfo;
        public EditContext EditContext { get; set; } = new EditContext(propertyInfo);

        public bool HasPropertyChanged<T>(Expression<Func<T, object>> expression)
        {
            return MudModelFormTools.WithPropertyExpression<T>(expression) == PropertyInfo;
        }
    }

    public class ModelFormChangeArgs(ModelFormArgsInternal internalArgs, IModelFormContext modelFormContext)
    {
        public PropertyInfo PropertyInfo { get; set; } = internalArgs.PropertyInfo;
        public EditContext EditContext { get; set; } = internalArgs.EditContext;

        public IModelFormContext Context { get; set; } = modelFormContext;

        public bool HasPropertyChanged<T>(Expression<Func<T, object>> expression)
        {
            return MudModelFormTools.WithPropertyExpression(expression) == PropertyInfo;
        }
    }

    public class ModelFormContext<T> : IModelFormContext
    {
        private Dictionary<PropertyInfo, (Delegate Label, Delegate Choices, Delegate? onChange)> optionProperties = [];
        private Dictionary<PropertyInfo, (MudFormsDropdownPropertyInput<T> component, Action updateOptions)> optionPropertyComponent = [];
        private readonly Dictionary<PropertyInfo, string> fieldNotes = [];
        private readonly Dictionary<string, string> categoryNotes = [];
        internal ILogger? Logger { get; set; }
        private readonly List<string> categoryLocks = [];
        public Func<Task>? RefreshModel { get; set; }
        public List<PropertyInfo> Properties { get; private set; } = [];

        public List<(string category, List<List<PropertyInfo>> properties)> GetCategories() =>
            typeof(T).GetMudModelFormCategories().Select(elem => (string.IsNullOrWhiteSpace(elem.category) ? string.Empty : elem.category,
                    visibleProperties: elem.properties.Select(l => l.Where(l => IsVisible(l)).ToList()).ToList()))
                    .Where(p => p.visibleProperties.Any(l => l.Count > 0)).ToList();

        public List<string> LockedCategories => categoryLocks;

        public bool IsCategoryLocked(string category)
        {
            return categoryLocks.Contains(category);
        }

        public void RegisterCategoryLock(string category, bool isLocked)
        {
            if (isLocked && !categoryLocks.Contains(category))
            {
                categoryLocks.Add(category);
            }

            if (!isLocked && categoryLocks.Contains(category))
            {
                categoryLocks.Remove(category);
            }
        }

        public string GetFieldNote(PropertyInfo propertyInfo)
        {
            if (fieldNotes.ContainsKey(propertyInfo))
            {
                return fieldNotes[propertyInfo];
            }
            return string.Empty;
        }

        public string GetCategoryNote(string category)
        {
            if (categoryNotes.ContainsKey(category))
            {
                return categoryNotes[category];
            }
            return string.Empty;
        }

        public void RegisterCategoryNotes(string category, string notes)
        {
            if (categoryNotes.ContainsKey(category))
            {
                categoryNotes.Remove(category);
            }
            categoryNotes.Add(category, notes);
        }

        public void RegisterFieldNotes<P>(Expression<Func<P, object>> propertyPath, string notes)
        {
            var propertyInfo = GetPropertyInfo(propertyPath);
            if (fieldNotes.ContainsKey(propertyInfo))
            {
                fieldNotes.Remove(propertyInfo);
            }
            fieldNotes.Add(propertyInfo, notes);
        }

        public void RegisterOptionValueProperty(PropertyInfo property, Func<IEnumerable<string>> choices, Action<string>? onChange = null)
        {
            RegisterOptionValueProperty(property, e => e, choices, onChange);
        }

        public void RegisterOptionValueProperty(Expression<Func<T, string>> propertyPath, Func<IEnumerable<string>> choices, Action<string>? onChange = null)
        {
            RegisterOptionValueProperty<string>(propertyPath, e => e, choices, onChange);
        }

        private static PropertyInfo GetPropertyInfo(LambdaExpression expression)
        {
            return MudModelFormTools.WithPropertyExpression(expression);
        }

        public void RegisterOptionValueProperty<P>(Expression<Func<P, string>> propertyPath, Func<IEnumerable<string>> choices, Action<string>? onChange = null)
        {
            RegisterOptionValueProperty(GetPropertyInfo(propertyPath), e => e, choices, onChange);
        }

        public void RegisterOptionValueProperty<P1, P2>(Expression<Func<P1, P2>> propertyPath, Func<P2, string> label, Func<IEnumerable<P2>> choices, Action<P2>? onChange = null)
        {
            RegisterOptionValueProperty(GetPropertyInfo(propertyPath), label, choices, onChange);
        }

        public void RegisterOptionValueProperty<S>(Expression<Func<T, S>> propertyPath, Func<S, string> label, Func<IEnumerable<S>> choices, Action<S>? onChange = null)
        {
            RegisterOptionValueProperty(GetPropertyInfo(propertyPath), label, choices, onChange);
        }

        public void RegisterOptionValueProperty<S>(PropertyInfo property, Func<S, string> label, Func<IEnumerable<S>> choices, Action<S>? onChange = null)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (label is null)
            {
                throw new ArgumentNullException(nameof(label));
            }

            if (choices is null)
            {
                throw new ArgumentNullException(nameof(choices));
            }

            if (optionProperties.ContainsKey(property))
            {
                optionProperties.Remove(property);
            }

            optionProperties.Add(property, (label, choices, onChange));
        }

        public void OnOptionPropertyChange(PropertyInfo property, object newValue)
        {
            if (optionProperties.TryGetValue(property, out var delegates))
            {
                delegates.onChange?.DynamicInvoke(newValue);
            }
        }

        public bool IsDropDown(PropertyInfo propertyInfo)
        {
            var attributeIsDefined = Attribute.IsDefined(propertyInfo, typeof(MudFormAttribute));
            var customAttribute = Attribute.GetCustomAttribute(propertyInfo, typeof(MudFormAttribute)) as MudFormAttribute;
            
            var hasValidValues = attributeIsDefined
                ? customAttribute?.ValidValues?.Length > 0
                : optionProperties.ContainsKey(propertyInfo);
            var hasDropDown = attributeIsDefined
                ? customAttribute?.IsDropDown ?? false
                : false;
            return hasValidValues || hasDropDown;
        }

        public static Type[] GetDelegateParameterTypes(MethodInfo invoke)
        {
            ParameterInfo[] parameters = invoke.GetParameters();
            Type[] typeParameters = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                typeParameters[i] = parameters[i].ParameterType;
            }
            return typeParameters;
        }

        public static Type GetDelegateReturnType(MethodInfo invoke)
        {
            return invoke.ReturnType;
        }

        public string? GetDisplayValue(PropertyInfo propertyInfo, object item)
        {
            var currentValue = propertyInfo.GetValue(item);
            if (optionProperties.TryGetValue(propertyInfo, out var accessors))
            {
                return accessors.Label.DynamicInvoke(currentValue) as string;
            }
            return currentValue?.ToString();
        }

        public void RegisterOptionComponent(PropertyInfo propertyInfo, MudFormsDropdownPropertyInput<T> aeDropdown, Action updateOptions)
        {
            if (!optionPropertyComponent.TryAdd(propertyInfo, (aeDropdown, updateOptions)))
            {
                optionPropertyComponent[propertyInfo] = (aeDropdown, updateOptions);
            }
        }

        public void RefreshOptions(PropertyInfo propertyInfo)
        {
            if (optionPropertyComponent.TryGetValue(propertyInfo, out var component))
            {
                component.updateOptions?.Invoke();
            }
        }

        public void RefreshOptions<T1>(Expression<Func<T1, object>> propertyPath)
        {
            RefreshOptions(GetPropertyInfo(propertyPath));
        }

        public void RefreshOptions<S>(Expression<Func<T, S>> propertyPath)
            => RefreshOptions(GetPropertyInfo(propertyPath));

        public (List<object> values, List<string> labels) GetOptionValuesForProperty(PropertyInfo propertyInfo)
        {
            if (optionProperties.TryGetValue(propertyInfo, out var accessors))
            {
                var validValues = accessors.Choices.DynamicInvoke() as IEnumerable;
                if (validValues == null) return ([], []);
                var displayValues = new List<string>();
                var values = new List<object>();
                foreach (var item in validValues)
                {
                    values.Add(item);
                    displayValues.Add(accessors.Label.DynamicInvoke(item) as string);
                }
                return (values, displayValues);
            }
            var dropdownValues = propertyInfo.DropdownValues()?.ToList();
            if (dropdownValues is null)
                Logger?.LogWarning($"Property {propertyInfo.Name} doesn't have any values for dropdown");
            return (dropdownValues?.Cast<object>()?.ToList() ?? new List<object>(), dropdownValues ?? new List<string>());
        }

        public void Clear()
        {
            RefreshModel = null;
            Properties = [];
            optionProperties = [];
            optionPropertyComponent = [];
            propertyVisibility = [];
        }

        public PropertyInfo GetProperty(string name)
        {
            return Properties.Single(p => string.Equals(p.Name, name, StringComparison.InvariantCultureIgnoreCase));
        }

        public void SetProperties(List<PropertyInfo> properties)
        {
            this.Properties = properties;
        }

        public void SetVisible<T1>(Expression<Func<T1, object>> propertyPath, bool isVisible)
            => SetVisible(GetPropertyInfo(propertyPath), isVisible);

        private Dictionary<PropertyInfo, bool> propertyVisibility = new Dictionary<PropertyInfo, bool>();

        public bool IsVisible(PropertyInfo property)
        {
            if (propertyVisibility.TryGetValue(property, out bool isVisible))
            {
                return isVisible;
            }
            return true;
        }

        public void SetVisible(PropertyInfo property, bool isVisible)
        {
            if (!propertyVisibility.TryAdd(property, isVisible))
                propertyVisibility[property] = isVisible;
        }

        private Func<T, Dictionary<string, List<string>>>? _validator;

        public Dictionary<string, List<string>> Validate(T model)
        {
            return _validator is null
                ? new Dictionary<string, List<string>>()
                : _validator(model);
        }

        public void SetValidator<T1>(Func<T1, Dictionary<string, List<string>>> validator)
        {
            _validator = (Func<T, Dictionary<string, List<string>>>)Convert.ChangeType(validator,
                typeof(Func<T1, Dictionary<string, List<string>>>));
        }

        public bool CustomValidation => _validator != null;
    }
}