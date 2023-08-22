using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Workout_Programs.ViewModels;

namespace Workout_Programs.Views
{
    /// <summary>
    /// Interaction logic for WorkoutDetails.xaml
    /// </summary>
    public partial class WorkoutDetails : Window
    {
        public WorkoutDetails()
        {
            InitializeComponent();
            DataContext = new WorkoutViewModel();
        }

        // Move window without from anywhere.
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
