using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Services
{
    public class DadataService : IDadataService
    {
        public DadataService()
        {

        }

        public async Task<PartyResponse?> GetSuggestionAsync(string inn)
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://suggestions.dadata.ru");
            var content = JsonContent.Create(new PartyRequest()
            {
                Count = 10,
                Query = inn
            });
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                Content = content,
                RequestUri = new Uri("https://suggestions.dadata.ru/suggestions/api/4_1/rs/findById/party")
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", "62e19320bac8f5e05f1056364b99e0a38c2b37c6");
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var resultString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<PartyResponse>(resultString);
                return result;
            }
            return null;
        }

    }

    public class PartyTest
    {
        public int Id { get; set; }
        public string Query { get; set; }
        public int Count { get; set; }
    }

    public class PartyRequest
    {
        public string? Query { get; set; }
        public int Count { get; set; }
    }

    public class PartyResponse
    {
        public List<Suggestion<Party>> Suggestions { get; set; }
    }

    public class Suggestion<T>
    {
        public string value { get; set; }
        public string unrestricted_value { get; set; }
        public T data { get; set; }
    }

    public class Party
    {
        public int branch_count { get; set; }
        public string branch_type { get; set; }
        public string inn { get; set; }
        public string kpp { get; set; }
        public string ogrn { get; set; }    
        public string hid { get; set; }
        public PartyManagement management { get; set; }
        public PartyName name { get; set; }
        public Fullname fio { get; set; }
        public string okato { get; set; }
        public string oktmo { get; set; }
        public string okpo { get; set; }
        public string okogu { get; set; }
        public string okfs { get; set; }
        public string okved { get; set; }
        public List<PartyOkved> okveds { get; set; }
        public string okved_type { get; set; }
        public PartyOpf opf { get; set; }
        public PartyState state { get; set; }
        public string type { get; set; }
        public int? employee_count { get; set; }
        public PartyFinance finance { get; set; }
        public PartyCapital capital { get; set; }
        public PartyAuthorities authorities { get; set; }
        public PartyCitizenship citizenship { get; set; }
        public PartyDocuments documents { get; set; }
        public List<PartyLicense> licenses { get; set; }

        public List<PartyFounder> founders { get; set; }
        public List<PartyManager> managers { get; set; }

        public List<PartyReference> predecessors { get; set; }
        public List<PartyReference> successors { get; set; }
    }

    public class Fullname
    {
        public string surname { get; set; }
        public string name { get; set; }
        public string patronymic { get; set; }
    }

    public class PartyReference
    {
        public string ogrn { get; set; }
        public string inn { get; set; }
        public string name { get; set; }
    }

    public class PartyAuthorities
    {
        public PartyAuthority fts_registration { get; set; }
        public PartyAuthority fts_report { get; set; }
        public PartyAuthority pf { get; set; }
        public PartyAuthority sif { get; set; }
    }

    public class PartyAuthority
    {
        public string type { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string address { get; set; }
    }

    public class PartyCapital
    {
        public string type { get; set; }
        public decimal? value { get; set; }
    }

    public class PartyCitizenship
    {
        public PartyCodeUnit code { get; set; }
        public PartyNameUnit name { get; set; }
    }

    public class PartyCodeUnit
    {
        public string numeric { get; set; }
        public string alpha_3 { get; set; }
    }

    public class PartyDocument
    {
        public string type { get; set; }
        public string series { get; set; }
        public string number { get; set; }
        public string issue_authority { get; set; }
    }

    public class PartyDocuments
    {
        public PartyDocument fts_registration { get; set; }
        public PartyDocument fts_report { get; set; }
        public PartyDocument pf_registration { get; set; }
        public PartyDocument sif_registration { get; set; }
        public PartySmb smb { get; set; }
    }

    public class PartyNameUnit
    {
        public string full { get; set; }
        public string @short { get; set; }
    }

    public class PartyFinance
    {
        public string? tax_system { get; set; }
        public decimal? income { get; set; }
        public decimal? expense { get; set; }
        public decimal? debt { get; set; }
        public decimal? penalty { get; set; }
        public int? year { get; set; }
    }

    public class PartyFounder
    {
        public string ogrn { get; set; }
        public string inn { get; set; }
        public string name { get; set; }
        public Fullname fio { get; set; }
        public string hid { get; set; }
        public string type { get; set; }
        public PartyFounderShare share { get; set; }
    }

    public class PartyFounderShare
    {
        public string type { get; set; }
        public decimal value { get; set; }
        public long numerator { get; set; }
        public long denominator { get; set; }
    }

    public class PartyLicense
    {
        public string series { get; set; }
        public string number { get; set; }
        public string issue_authority { get; set; }
        public string suspend_authority { get; set; }
        public List<string> activities { get; set; }
        public List<string> addresses { get; set; }
    }

    public class PartyManagement
    {
        public string name { get; set; }
        public string post { get; set; }
        public string disqualified { get; set; }
    }

    public class PartyManager
    {
        public string ogrn { get; set; }
        public string inn { get; set; }
        public string name { get; set; }
        public Fullname fio { get; set; }
        public string post { get; set; }
        public string hid { get; set; }
        public string type { get; set; }
    }

    public class PartyName
    {
        public string full_with_opf { get; set; }
        public string short_with_opf { get; set; }
        public string latin { get; set; }
        public string full { get; set; }
        public string @short { get; set; }
    }

    public class PartyOkved
    {
        public bool main { get; set; }
        public string type { get; set; }
        public string code { get; set; }
        public string name { get; set; }
    }

    public class PartyOpf
    {
        public string code { get; set; }
        public string full { get; set; }
        public string @short { get; set; }
    }

    public class PartySmb
    {
        public string type { get; set; }
        public string category { get; set; }
    }

    public class PartyState
    {
        public string status { get; set; }
        public string code { get; set; }
    }
}