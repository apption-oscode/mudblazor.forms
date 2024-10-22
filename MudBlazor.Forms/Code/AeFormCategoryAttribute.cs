using System;

namespace MudBlazor.Forms
{
    /// <summary>
    /// Attribute to define the category of a property in a form.
    /// </summary>
    public class AeFormCategoryAttribute : Attribute
    {
        public AeFormCategoryAttribute(string category, int categoryOrder = 0)
        {
            Category = category;
            CategoryOrder = categoryOrder;
        }

        /// <summary>
        /// The category of the property.
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// The order of the category.
        /// </summary>
        public int CategoryOrder { get; set; }
    }
}