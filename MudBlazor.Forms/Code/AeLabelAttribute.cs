using System;

namespace MudBlazor.Forms
{
    public class MudFormLabelAttribute : Attribute
    {
        public string? Label { get; init; }
        public virtual string? Placeholder { get; init; }
        public virtual int Order { get; init; }
        public virtual int StringLength { get; init; }
        public virtual int LineCount { get; init; }
        public virtual string[]? ValidValues { get; init; }
        public virtual bool IsDropDown { get; init; }
        public virtual string? Row { get; init; }
        public virtual string? Column { get; init; }
        public virtual bool IsPasswordField { get; init; }
        
        public MudFormLabelAttribute(string label)
        {
            Label = label;
        }
        
        public MudFormLabelAttribute()
        {
        }
    }
}