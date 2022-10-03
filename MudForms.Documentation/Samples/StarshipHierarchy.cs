using MudBlazor.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudForms.Documentation.Samples
{
    public class StarshipHierarchy
    {
        [AeFormIgnore]
        private int ID { get; set; } = 1;        

        [AeFormCategory("Hierarchy")]
        [Required]
        [MudForm("Starship Sector", IsDropDown =true)]
        public StarshipLevel StarshipSector { get; set; }


        [AeFormCategory("Hierarchy")]
        [Required]
        [MudForm("Starship Branch", IsDropDown = true)]
        public StarshipLevel StarshipBranch { get; set; }

        [AeFormCategory("Hierarchy")]
        [MudForm("Starship Branch", IsDropDown = true)]
        public StarshipLevel StarshipDivision { get; set; }




    }

    public class StarshipLevel
    { 
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string LevelName { get; set; }        
    }
}
