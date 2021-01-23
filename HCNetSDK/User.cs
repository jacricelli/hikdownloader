namespace HikDownloader.HCNetSDK
{
    /// <summary>
    /// Usuario.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Identificador.
        /// </summary>
        public int Identifier { get; private set; } = -1;

        /// <summary>
        /// Usuario.
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="identifier">Identificador.</param>
        /// <param name="userName">Nombre de usuario.</param>
        public User(int identifier, string userName)
        {
            Identifier = identifier;
            UserName = userName;
        }
    }
}
