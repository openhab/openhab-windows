using System.Collections.Generic;
using System.Linq;
using openHAB.Core.Client.Models;
using openHAB.Windows.ViewModel;

namespace openHAB.Windows.Services
{
    /// <summary>
    /// Service that keeps track of navigation between linked pages.
    /// </summary>
    public static class WidgetNavigationService
    {
        private static readonly Stack<WidgetViewModel> WidgetBackStack = new Stack<WidgetViewModel>();
        private static WidgetViewModel _currentWidget;

        /// <summary>
        /// Gets a value indicating whether there is a previous widget on the backstack.
        /// </summary>
        public static bool CanGoBack => _currentWidget != null;

        /// <summary>
        /// Navigates the backstack to the passed in target.
        /// </summary>
        /// <param name="target">The openHAB widget to navigate to.</param>
        public static void Navigate(WidgetViewModel target)
        {
            if (target == _currentWidget)
            {
                return;
            }

            WidgetBackStack.Push(target);
            _currentWidget = target;
        }

        /// <summary>
        /// Go back to the previous openHAB widget.
        /// </summary>
        /// <returns>The previous visited widget.</returns>
        public static WidgetViewModel GoBack()
        {
            if (WidgetBackStack.Count == 0)
            {
                return null;
            }

            WidgetBackStack.Pop();
            _currentWidget = WidgetBackStack.Count == 0 ? null : WidgetBackStack.Peek();

            return _currentWidget;
        }

        /// <summary>
        /// Go back to the previous openHAB widget.
        /// </summary>
        /// <returns>The previous visited widget.</returns>
        public static WidgetViewModel GoBackToRoot()
        {
            WidgetViewModel widget = GoBack();
            while (widget != null && widget.Parent != null)
            {
                widget = GoBack();
            }

            _currentWidget = WidgetBackStack.Count == 0 ? null : WidgetBackStack.Peek();

            return _currentWidget;
        }

        /// <summary>
        /// Clears and resets the widget navigation.
        /// </summary>
        public static void ClearWidgetNavigation()
        {
            _currentWidget = null;
            WidgetBackStack.Clear();
        }

        /// <summary>Gets the navigated widgets.</summary>
        /// <value>The widgets.</value>
        public static List<WidgetViewModel> Widgets => WidgetBackStack.Reverse().ToList();
    }
}
