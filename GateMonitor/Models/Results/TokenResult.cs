namespace GateMonitor.Models.Results
{
    public class TokenResult
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
    }
}
