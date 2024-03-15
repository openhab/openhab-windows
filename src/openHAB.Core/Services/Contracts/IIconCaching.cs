using System.Threading.Tasks;

namespace openHAB.Core.Services.Contracts
{
    /// <summary>
    /// Caching mechanism for widget icons.
    /// </summary>
    public interface IIconCaching
    {
        /// <summary>
        /// Clears the icon cache directory.
        /// </summary>
        void ClearIconCache();

        /// <summary>Resolves the path to local cached icon file.<br />
        /// If the icon is not present in cache the file will be downloaded from OpenHAB server.</summary>
        /// <param name="iconUrl">The icon URL.</param>
        /// <param name="state"> Item state. </param>
        /// <param name="iconFormat">The icon format.</param>
        /// <returns>Path to icon file in local cache.</returns>
        Task<string> ResolveIconPath(string iconUrl, string state, string iconFormat);
    }
}