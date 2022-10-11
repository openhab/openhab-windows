using openHAB.Core.Model.Event;

namespace openHAB.Core.Notification.Contracts
{
    /// <summary>openHAB event parser.</summary>
    public interface IOpenHABEventParser
    {
        /// <summary>Parses the openHAB evnet message.</summary>
        /// <param name="message">The message.</param>
        /// <returns>
        ///   openHAB event object <see cref="OpenHABEvent"/>.
        /// </returns>
        OpenHABEvent Parse(string message);
    }
}