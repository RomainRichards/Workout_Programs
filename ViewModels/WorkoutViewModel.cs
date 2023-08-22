using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Workout_Programs.Models;

namespace Workout_Programs.ViewModels
{
    public class WorkoutViewModel : INotifyPropertyChanged
    {
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

       
        private WorkoutDetail _workoutDetail;
        public WorkoutDetail WorkoutDetail
        {
            get { return _workoutDetail; }
            set 
            {
                _workoutDetail = value;
                OnPropertyChanged(nameof(WorkoutDetail));
            }
        }

        private ObservableCollection<WorkoutDetail> lstWorkoutDetail;
        public ObservableCollection<WorkoutDetail> LstWorkoutDetail
        {
            get { return lstWorkoutDetail; }
            set
            {
                lstWorkoutDetail = value;
                OnPropertyChanged(nameof(LstWorkoutDetail));
            }
        }
        private WorkoutDetail _selectedWorkoutDetail;

        public WorkoutDetail SelectedWorkoutDetail
        {
            get { return _selectedWorkoutDetail; }
            set 
            {
                _selectedWorkoutDetail = value;
                OnPropertyChanged(nameof(SelectedWorkoutDetail));
            }
        }

        private WorkoutDetail workoutDetail;

        //public WorkoutDetail WorkoutDetail
        //{
        //    get { return workoutDetail; }
        //    set
        //    {
        //        workoutDetail = value;
        //        OnPropertyChanged(nameof(WorkoutDetail));
        //    }
        //}


        // Initialize database.
        WorkoutProgramDBEntities workoutProgramDBEntities;

        // Constructor
        // Commands to be binded to the buttons on view. 
        public WorkoutViewModel()
        {
            workoutProgramDBEntities = new WorkoutProgramDBEntities();
            LoadWorkout();
            DeleteCommand = new Command((s) => true, Delete);
            UpdateCommand = new Command((s) => true, Select_Update);
            AddWorkoutCommand = new Command((s) => true, AddWorkout);
            UpdateWorkoutCommand = new Command((s) => true, UpdateWorkout);
        }

        // Add workout to the database. 
        private void AddWorkout(object obj)
        {
            if(WorkoutDetail != null)
            {
                workoutProgramDBEntities.WorkoutDetails.Add(WorkoutDetail);
                workoutProgramDBEntities.SaveChanges();
                WorkoutDetail = new WorkoutDetail();
            }
            else
            {
                MessageBox.Show("Missing Entry");
            }
        }

        // Save changes made after updating the workout to the View to the database. 
        private void UpdateWorkout(object obj)
        {
            workoutProgramDBEntities.SaveChanges();
        }

        // Selects workout to update, populating the text box witht that workout details.
        // Clears the textboxes after updated.
        private void Select_Update(object obj)
        {
            SelectedWorkoutDetail=obj as WorkoutDetail;
            SelectedWorkoutDetail = new WorkoutDetail();
        }

        // Delete workout.
        private void Delete(object obj)
        {
            var workout = (WorkoutDetail)obj; // Casting ---> equivalent to "obj as WorkoutDetail".
            workoutProgramDBEntities.WorkoutDetails.Remove(workout);
            workoutProgramDBEntities.SaveChanges();
            LstWorkoutDetail.Remove(workout);
        }

        // Read details.
        private void LoadWorkout() 
        {
            LstWorkoutDetail = new ObservableCollection<WorkoutDetail>(workoutProgramDBEntities.WorkoutDetails);
        }

        public ICommand DeleteCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand UpdateWorkoutCommand { get; set; }
        public ICommand AddWorkoutCommand { get; set; }
    }

    // Implement ICommand interface to call different commands to the view.
    class Command : ICommand
    {
        public Command(Func<object, bool> methodCanExecute, Action<object> methodExecute)
        {
            MethodCanExecute = methodCanExecute;
            MethodExecute = methodExecute;
        }

        Action<object> MethodExecute;
        Func<object, bool> MethodCanExecute;
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return MethodExecute != null && MethodCanExecute.Invoke(parameter);
        }

        public void Execute(object parameter)
        {
            MethodExecute(parameter);
        }
    }
}


