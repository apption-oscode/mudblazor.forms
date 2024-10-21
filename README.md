# MudBlazor.Forms

**MudBlazor.Forms** is a library built on top of **MudBlazor** for creating responsive, dynamic, and highly customizable forms in Blazor applications. It simplifies form handling, validation, and binding, offering a clean and efficient way to work with structured data inputs in Blazor.

## Features

- **Seamless Blazor Integration:** Easily integrate with MudBlazor components for a consistent and beautiful UI.
- **Dynamic Form Creation:** Automatically generate forms based on data models with minimal configuration.
- **Form Validation:** Built-in validation support, compatible with Data Annotations and custom validation logic.
- **Custom Field Types:** Support for custom field types and layouts, making forms flexible and extensible.
- **Responsive Design:** Fully responsive form components out-of-the-box, adapting to various screen sizes.
- **Styled by MudBlazor:** Use MudBlazor's theming and design system for professional, modern UI styling.

## Installation

You can install MudBlazor.Forms via the NuGet package manager:

```powershell
Install-Package MudBlazor.Forms
```

### Usage

- See [Examples](https://github.com/apption-oscode/mudblazor.forms/tree/main/MudForms.Documentation/Samples/Examples)


- Import the required namespaces into your Blazor project.
```csharp
@using MudBlazor
@using MudBlazor.Forms
```

- Annotate your classes or use fluent API to define validation rules.
```csharp
    public class InteractiveStarship
    {

        [AeFormIgnore]
        private int ID { get; set; } = 1;

        [Required]
        [StringLength(16, ErrorMessage = "Identifier too long (16 character limit).")]
        [MudForm(Placeholder ="Starship identifier...")]
        public string Identifier { get; set; }

        [Required]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Invalid Email Address")]
        [StringLength(20, ErrorMessage = "Identifier too long (16 character limit).")]
        [MudForm(Placeholder = "Enter Email...")]
        public string CaptainsEmail { get; set; }

        [Editable(false)]
        [MudForm(Placeholder ="Describe your starship including crew size")]
        public string Description { get; set; }

        [Required]
        [MudForm(IsDropDown =true)]
        public string Classification { get; set; }

        [Required]
        [MudForm(IsDropDown = true)]
        public string SubClassification { get; set; }

        [Range(1, 100000, ErrorMessage = "Accommodation invalid (1-100000).")]
        [MudForm(Label = "Maximum Accomodation")]
        [DisplayFormat(DataFormatString = "C2")]
        public double MaximumAccommodation { get; set; }

        [MudForm(Label = "Maintenance Cost")]
        [Editable(false)]
        [DisplayFormat(DataFormatString = "C2")]
        public double MaintenanceCost => MaximumAccommodation * 100;

        [Required]
        [Range(typeof(bool), "true", "true", ErrorMessage = "This form disallows unapproved ships.")]
        [MudForm("Validated Design")]
        public bool IsValidatedDesign { get; set; }

        [Required]
        [MudForm("Production Date")]
        [DisplayFormat(DataFormatString = "yyyy/MM/dd")]
        public DateTime ProductionDate { get; set; }

        [Editable(false)]
        [DisplayFormat(DataFormatString = "yyyy/MM/dd")]
        public DateTime? FirstFlightDate { get; set; }

        public int? SubLightEngines { get; set; }
    }
```

- Use MudForm components in your Blazor pages to generate dynamic forms.
```html
<MudModelForm T="InteractiveStarship"
             Model="@_starship"
             SubmitLabel="Save changes"
             OnValidSubmit="HandleValidSubmit"
             IsReadOnly="false"
             CancelLabel="Cancel"    
             IsSubmitEnabled="true"
             OnConfigure="OnConfigure"
             OnCancel="CreateStarship" />
```

- Customize the form using MudBlazor's extensive styling and component system.


### Contributing
Contributions are welcome! Please feel free to open issues or submit pull requests to help improve this project.

### License
MudBlazor.Forms is licensed under the MIT License. See LICENSE for more details.
