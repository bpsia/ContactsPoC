using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contact.Common
{
    public class Customer
    {
        public Customer()
        {

        }

        [Key]
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ListOfEmails { get; set; }

        public string ListOfPhoneNumbers { get; set; }

        public DateTime DateOfBirth { get; set; }
    }
}
