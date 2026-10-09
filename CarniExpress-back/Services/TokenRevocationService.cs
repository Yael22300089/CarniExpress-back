
using System.Collections.Concurrent;

namespace CarniExpress_back.Services
{
    public class TokenRevocationService
    {
        private readonly ConcurrentDictionary<string, DateTimeOffset>
            _tokensRevocados = new();

        public void Revocar(string tokenId, DateTimeOffset expiracion)
        {
            _tokensRevocados[tokenId] = expiracion;
        }

        public bool EstaRevocado(string tokenId)
        {
            if (_tokensRevocados.TryGetValue(
                tokenId, out var expiracion))
            {
                if (expiracion > DateTimeOffset.UtcNow)
                    return true;

                _tokensRevocados.TryRemove(tokenId, out _);
            }

            return false;
        }
    }
}
