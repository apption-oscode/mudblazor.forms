﻿using System;

namespace MudBlazor.Forms
{
    public class AeFormCategoryAttribute : Attribute
    {
        public AeFormCategoryAttribute(string category, int categoryOrder = 0)
        {
            Category = category;
            CategoryOrder = categoryOrder;
        }

        public string Category { get; }
        public int CategoryOrder { get; set; }
    }
}