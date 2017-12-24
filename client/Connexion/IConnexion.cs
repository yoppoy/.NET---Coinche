using client.Game;

namespace client.Connexion
{
    public interface IConnexion
    {
        bool Connect(string ip, int port, IGame game);
    }
}
