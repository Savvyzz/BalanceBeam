namespace BalanceBeam.Identity.Common.Contracts
{
    public record UserRegisteredEvent(string UserName, string Email, string Token)
    {
    }
}
