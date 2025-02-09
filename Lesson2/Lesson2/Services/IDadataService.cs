namespace Services
{
    public interface IDadataService
    {
        Task<PartyResponse?> GetSuggestionAsync(string inn);
    }
}