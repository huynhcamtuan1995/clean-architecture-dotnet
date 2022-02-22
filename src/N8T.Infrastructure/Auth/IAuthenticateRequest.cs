using N8T.Core.Domain;

namespace N8T.Infrastructure.Auth
{
    public interface IAuthenticateRequest<TRequest, TResponse> : IQuery<TResponse>
        where TRequest : notnull, IAuthenticateModel
    {
        TRequest Model { get; init; }
    }
}
