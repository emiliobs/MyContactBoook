using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyContactBoook.Models
{
    public class ContactValidation
    {
        //[Key]
        //public int ValidationId { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Please provide {0}", AllowEmptyStrings = false)]
        public string ContactFirstName { get; set; }

        [Display(Name = "Last Name")]
        public string ContactLastName { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Email not Valid.")]
        public string Email { get; set; }

        public string Phone { get; set; }

        [Display(Name = "Country")]
        public int CountryId { get; set; }

        [Display(Name = "State")]
        public int StateId { get; set; }
    }

    [MetadataType(typeof(ContactValidation))] // Apply validation
    public partial class Contact
    {

    }
}