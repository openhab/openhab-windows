using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace System.Collections.ObjectModel
{
    /// <summary>
    /// Collection of extensions for the ObservableCollection class.
    /// </summary>
    public static class ObservableCollectionExtensions
    {
        /// <summary>
        /// Adds a range of items to the observable collection.
        /// </summary>
        /// <param name="target">The ObservableCollection.</param>
        /// <param name="range">List of items to add.</param>
        /// <typeparam name="T">Type of items.</typeparam>
        /// <returns>The ObservableCollection, including the new range of items.</returns>
        public static ObservableCollection<T> AddRange<T>(this ObservableCollection<T> target, IEnumerable<T> range)
        {
            if (range == null)
            {
                return target;
            }

            foreach (var item in range)
            {
                target.Add(item);
            }

            return target;
        }
    }
}
