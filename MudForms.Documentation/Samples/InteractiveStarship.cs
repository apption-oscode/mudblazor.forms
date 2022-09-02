using System;
using System.ComponentModel.DataAnnotations;
using MudBlazor.Forms;
namespace MudForms.Documentation.Data
{
    public class InteractiveStarship
    {

        [AeFormIgnore]
        private int ID { get; set; } = 1;

        [Required]
        [StringLength(16, ErrorMessage = "Identifier too long (16 character limit).")]
        [MudForm(Placeholder ="Starship identifier...")]
        public string Identifier { get; set; }

        [Required]
        //[RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Invalid Email Address")]
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
        [DisplayFormat(DataFormatString = "C")]
        public double MaximumAccommodation { get; set; }

        [MudForm(Label = "Maintenance Cost")]
        [Editable(false)]
        [DisplayFormat(DataFormatString = "C2")]
        public double MaintenanceCost => MaximumAccommodation * 100;

        [Required]
        //[Range(typeof(bool), "true", "true", ErrorMessage = "This form disallows unapproved ships.")]
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
}
