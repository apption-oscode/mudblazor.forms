using System;
using System.ComponentModel.DataAnnotations;
using MudBlazor.Forms;

namespace MudForms.Documentation.Data
{
    public class Starship2
    {
        [AeFormIgnore]
        private int ID { get; set; } = 1;

        [Required]
        [MudForm(Placeholder = "Starship identifier...", Row = "1", Column = "1")]
        public string ShipName { get; set; }

        [Required]
        [StringLength(16, ErrorMessage = "Identifier too long (16 character limit).")]
        [MudForm(Placeholder = "Starship identifier...", Row = "1", Column ="2")]
        public string Identifier { get; set; }

        [Required]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Invalid Email Address")]
        [StringLength(20, ErrorMessage = "Identifier too long (16 character limit).")]
        [MudForm(Placeholder = "Enter Email...", Row = "2", Column ="1")]
        public string CaptainsEmail { get; set; }

        [MudForm(StringLength = 50, Placeholder = "Describe your starship including crew size", Row = "3", Column ="1")]
        public string Description { get; set; }

        [Required]
        [MudForm(ValidValues = new[] { "Class1Fighter", "Class1Discovery" }, Row = "2", Column ="2")]
        public string Classification { get; set; }

        [Range(1, 100000, ErrorMessage = "Accommodation invalid (1-100000).")]
        [MudForm(Label = "Maximum Accomodation", Row = "4", Column ="1")]
        public int MaximumAccommodation { get; set; }

        [Required]
        [Range(typeof(bool), "true", "true", ErrorMessage = "This form disallows unapproved ships.")]
        [MudForm("Validated Design", Row = "4", Column ="2")]
        public bool IsValidatedDesign { get; set; }

        [Required]
        [MudForm("Production Date", Row = "4", Column ="3")]
        public DateTime ProductionDate { get; set; }

        [MudForm("Production Date", Row = "5", Column ="1")]
        public DateTime? FirstFlightDate { get; set; }

        [MudForm("Production Date", Row = "5", Column ="2")]
        public int? SubLightEngines { get; set; }
    }
}
