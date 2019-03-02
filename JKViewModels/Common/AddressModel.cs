using System.Text;

namespace JKViewModels.Common
{
    public class AddressModel : BaseEntityModel
    {
        public int AddressId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public string FullAddress
        {
            get
            {
                var addressLine = string.IsNullOrEmpty(Address2) ? "{0}" : "{0} {1}";
                return string.Format(addressLine + ", {2}, {3} {4}", Address1, Address2, City, State, ZipCode);
            }
        }

        public string FormattedFullAddress
        {
            get
            {
                var address = _removeSpecialCharacters(Address1);
                return $"{address}, {City}, {State} {ZipCode}";
            }
        }

        private static string _removeSpecialCharacters(string str)
        {
            var sb = new StringBuilder();
            foreach (var c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
