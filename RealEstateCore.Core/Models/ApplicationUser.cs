using System;
using System.Collections.Generic;

namespace RealEstateCore.Core.Models
{
    public class ApplicationUser : BaseClass
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime DateRegistered { get; set; }
        public bool EmailConfirmed { get; set; }
        public string ClientId { get; set; }
        public string Salt { get; set; }

        public List<RealEstateProperty> Properties { get; set; }
    }
}