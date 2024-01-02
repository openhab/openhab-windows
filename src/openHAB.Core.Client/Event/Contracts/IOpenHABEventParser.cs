namespace openHAB.Core.Client.Event.Contracts
{
    /// <summary>openHAB event parser.</summary>
    public interface IOpenHABEventParser
    {
        /// <summary>Parses the openHAB event message.</summary>
        /// <param name="message">The message.</param>
        /// <returns>
        ///   openHAB event object <see cref="OpenHABEvent"/>.
        /// </returns>
        OpenHABEvent Parse(string message);
    }
}