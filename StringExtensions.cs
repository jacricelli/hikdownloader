namespace HikDownloader
{
    using System.Text;

    /// <summary>
    /// Extensión de <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Une uno o más valores a una cadena.
        /// </summary>
        /// <param name="value">Cadena.</param>
        /// <param name="others">Valores.</param>
        /// <returns>La cadena unida a los valores especificados.</returns>
        public static string JoinTo(this string value, params string[] others)
        {
            var builder = new StringBuilder(value);
            foreach (var v in others)
            {
                builder.Append(v);
            }

            return builder.ToString();
        }
    }
}