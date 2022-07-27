using MudBlazor.Forms;
using System;
using System.ComponentModel.DataAnnotations;

namespace MudForms.Documentation.Data
{
    public class TransportStarship
    {

        [AeFormIgnore]
        private int ID { get; set; } = 1;

        [Required]
        [StringLength(16, ErrorMessage = "Identifier too long (16 character limit).")]
        [MudForm(Placeholder ="Starship identifier...")]
        [AeFormCategory("Identification",1)]
        public string Identifier { get; set; }

        [Required]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Invalid Email Address")]
        [StringLength(20, ErrorMessage = "Identifier too long (16 character limit).")]
        [MudForm(Placeholder = "Enter Email...")]
        public string CaptainsEmail { get; set; }

        [MudForm(StringLength =50,Placeholder ="Describe your starship including crew size")]
        [AeFormCategory("Details", CategoryOrder = 2)]
        public string Description { get; set; }

        [Required]
        [MudForm(ValidValues =new[] { "Class1Fighter", "Class1Discovery"})]
        [AeFormCategory("Details", CategoryOrder = 2)]
        public string Classification { get; set; }

        [Range(1, 100000, ErrorMessage = "Accommodation invalid (1-100000).")]
        [AeFormCategory("Details", CategoryOrder = 2)]
        [MudForm(Label = "Maximum Accomodation")]
        public int MaximumAccommodation { get; set; }

        [Required]
        [Range(typeof(bool), "true", "true", ErrorMessage = "This form disallows unapproved ships.")]
        [AeFormCategory("Details", CategoryOrder = 2)]
        [MudForm("Validated Design")]
        public bool IsValidatedDesign { get; set; }

        [Required]
        [MudForm("Production Date")]
        [AeFormCategory("Details", CategoryOrder = 2)]
        [DisplayFormat(DataFormatString ="d")]
        public DateTime ProductionDate { get; set; }
        
        public DateTime? FirstFlightDate { get; set; }

        public int? SubLightEngines { get; set; }
    }
}
