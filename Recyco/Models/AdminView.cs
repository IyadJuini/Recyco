using Microsoft.Extensions.Hosting;

namespace ProjectRecycle.Models
{
    public class AdminView
    {
        public List<Company> AllCompanies { get; set; } = new();
        public List<Offer> AllOffers { get; set; } = new();
        
    }
}
