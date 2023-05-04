using Microsoft.UI.Xaml;
using openHAB.Core.Common;

namespace openHAB.Windows.Controls
{
    /// <summary>
    /// Trigger to differentiate between device families.
    /// </summary>
    public class DeviceFamilyStateTrigger : StateTriggerBase
    {
        /// <summary>
        /// The target device family property.
        /// </summary>
        public static readonly DependencyProperty TargetDeviceFamilyProperty = DependencyProperty.Register(
            "TargetDeviceFamily", typeof(DeviceFamily), typeof(DeviceFamilyStateTrigger), new PropertyMetadata(default(DeviceFamily), OnDeviceTypePropertyChanged));

        /// <summary>
        /// Gets or sets the target device family.
        /// </summary>
        /// <value>The target device family.</value>
        public DeviceFamily TargetDeviceFamily
        {
            get
            {
                return (DeviceFamily)GetValue(TargetDeviceFamilyProperty);
            }

            set
            {
                SetValue(TargetDeviceFamilyProperty, value);
            }
        }

        private static void OnDeviceTypePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            var trigger = (DeviceFamilyStateTrigger)dependencyObject;
            var newTargetDeviceFamily = (DeviceFamily)eventArgs.NewValue;
            trigger.SetActive(newTargetDeviceFamily == DeviceTypeHelper.DeviceFamily);
        }
    }
}
