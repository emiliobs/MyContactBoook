using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyContactBoook.Models
{
    public class ContactModel
    {
        public int ContactId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public string Country { get; set; }
        public string State{ get; set; }

        
    }
}