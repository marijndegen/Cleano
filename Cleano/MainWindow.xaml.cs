using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Cleano.ViewModels;

namespace Cleano
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel ViewModel => (MainViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void EditGroupButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                // Find the TextBox in the same Grid as the button
                var grid = FindParent<Grid>(button);
                if (grid != null)
                {
                    var textBox = FindChild<TextBox>(grid, "GroupNameTextBox");
                    if (textBox != null)
                    {
                        EnableEditing(textBox);
                    }
                }
            }
        }

        private void EditTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                // Find the TextBox in the same Grid as the button
                var grid = FindParent<Grid>(button);
                if (grid != null)
                {
                    var textBox = FindChild<TextBox>(grid, "TaskNameTextBox");
                    if (textBox != null)
                    {
                        EnableEditing(textBox);
                    }
                }
            }
        }

        private void EnableEditing(TextBox textBox)
        {
            textBox.IsReadOnly = false;
            textBox.Focus();
            textBox.SelectAll();
            textBox.Cursor = Cursors.IBeam;

            // Use a one-time event handler
            RoutedEventHandler lostFocusHandler = null;
            lostFocusHandler = (s, args) =>
            {
                textBox.IsReadOnly = true;
                textBox.Cursor = Cursors.Arrow;
                var binding = textBox.GetBindingExpression(TextBox.TextProperty);
                binding?.UpdateSource();
                ViewModel?.SaveCommand.Execute(null);

                if (ViewModel != null)
                {
                    var property = ViewModel.GetType().GetProperty("SortedTaskGroups");
                    if (property != null)
                    {
                        var onPropertyChanged = ViewModel.GetType().GetMethod("OnPropertyChanged",
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        onPropertyChanged?.Invoke(ViewModel, new object[] { "SortedTaskGroups" });
                    }
                }

                // Remove the handler after it's fired once
                textBox.LostFocus -= lostFocusHandler;
            };

            textBox.LostFocus += lostFocusHandler;
        }

        // Helper method to find parent of a specific type
        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }

        // Helper method to find child by name
        private static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                T childType = child as T;

                if (childType == null)
                {
                    foundChild = FindChild<T>(child, childName);
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        foundChild = (T)child;
                        break;
                    }
                    else
                    {
                        foundChild = FindChild<T>(child, childName);
                        if (foundChild != null) break;
                    }
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
    }
}