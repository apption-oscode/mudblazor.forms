﻿using MudBlazor.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudForms.Documentation.Samples
{
    public class StarshipWithOwner
    {
        [AeFormIgnore]
        private int ID { get; set; } = 1;

        [Required]
        [StringLength(16, ErrorMessage = "Identifier too long (16 character limit).")]
        [MudForm(Placeholder = "Starship identifier...")]
        public string Identifier { get; set; }

        [Required]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Invalid Email Address")]
        [StringLength(20, ErrorMessage = "Identifier too long (16 character limit).")]
        [MudForm(Placeholder = "Enter Email...")]
        public string CaptainsEmail { get; set; }

        [Required(ErrorMessage = "The Captains Password field is required")]
        [StringLength(20, ErrorMessage = "Identifier too long (20 character limit).")]
        [MudForm(Placeholder = "Enter your credit card number...", IsPasswordField = true)]
        public string CaptainsPassword { get; set; }

        [MudForm(StringLength = 50, Placeholder = "Describe your starship including crew size")]
        public string Description { get; set; }

        [Required]
        [MudForm(ValidValues = new[] { "Class1Fighter", "Class1Discovery" })]
        public string Classification { get; set; }

        [Range(1, 100000, ErrorMessage = "Accommodation invalid (1-100000).")]
        [MudForm(Label = "Maximum Accomodation")]
        public int MaximumAccommodation { get; set; }

        [Required]
        [Range(typeof(bool), "true", "true", ErrorMessage = "This form disallows unapproved ships.")]
        [MudForm("Validated Design")]
        public bool IsValidatedDesign { get; set; }

        [Required]
        [MudForm("Production Date")]
        public DateTime ProductionDate { get; set; }

        [Required]
        [MudForm("Owner", IsDropDown =true)]
        public StarshipOwner Owner { get; set; }

        [Required]
        public StarshipPort Port { get; set; }

        public DateTime? FirstFlightDate { get; set; }

        public int? SubLightEngines { get; set; }


    }

    public record StarshipOwner
    {
        public int OwnerID { get; set; }
        public string OwnerName { get; set; }
        public string OwnerLicense { get; set; }
    }

    public record StarshipPort
    {
        public int PortID { get; set; }
        public string PortName { get; set; }
    }
}
