namespace N8T.Infrastructure.Cache
{
    public interface ICacheService<T> where T : notnull
    {
        T Get(string key);

        bool Set(string key, T data);

        bool Remove(string key);
    }
}
