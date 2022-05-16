using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DSRProject.Data;
using DSRProject.Servicies;
using DSRProject.Data.DTO;

namespace DSRProject.Pages;

public class IndexModel : PageModel
{
    private readonly CurrencyDbContext _context;
    private HttpClient _client;
    private readonly ICurrenciesRepository _currenciesRepository;
    private readonly IExternalService _externalService;
    
    public IEnumerable<CurrencyDTO> Currencies { get; set; } = Enumerable.Empty<CurrencyDTO>();
    [BindProperty]
    public DateTime FirstDate { get; set; }
    [BindProperty]
    public DateTime SecondDate { get; set; }

    public IndexModel(CurrencyDbContext context, HttpClient client, ICurrenciesRepository currenciesRepository, IExternalService externalService) {
        _context = context;
        _client = client;
        _currenciesRepository = currenciesRepository;
        _externalService = externalService;
    }

    public void OnGet() {
        Currencies = _currenciesRepository.GetCurrencies();
        SecondDate = DateTime.UtcNow.Date;
        FirstDate = SecondDate.AddDays(-30);
    }
}