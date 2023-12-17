using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoingOutApp.Models
{
    public class AddressInfo
    {
        public string City { get; }
        public string Street { get; }
        public string HouseNumber { get; }
        public string PostalCode { get; }

        public AddressInfo(string city, string street, string houseNumber, string postalCode)
        {
            City = city;
            Street = street;
            HouseNumber = houseNumber;
            PostalCode = postalCode;
        }
    }

}
