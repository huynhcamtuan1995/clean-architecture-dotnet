namespace N8T.Infrastructure.Auth
{
    public interface IAuthenticateModel
    {
        string UserName { get; set; }
        string Password { get; set; }
    }
}
