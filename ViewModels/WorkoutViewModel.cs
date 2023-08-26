using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Xml.Linq;
using Workout_Programs.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Workout_Programs.ViewModels
{
    public class WorkoutViewModel : INotifyPropertyChanged
    {
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

       // Gets and Sets workout detail when adding a workout to the database. 
        private WorkoutDetail _workoutDetail = new WorkoutDetail();
        public WorkoutDetail WorkoutDetail
        {
            get { return _workoutDetail; }
            set 
            {
                _workoutDetail = value;
                OnPropertyChanged(nameof(WorkoutDetail));
            }
        }

        // Request workout list from the database to then be loaded to view.
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

        // Gets and Sets selected workout details, to be edited.
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
            SaveCommand = new Command((s) => true, SaveWorkout);
        }



        // Add workout to the database. 
        private void AddWorkout(object obj)
        {
            WorkoutDetail.ID = "WO" + workoutProgramDBEntities.WorkoutDetails.Count();
            if(WorkoutDetail != null)
            {
                workoutProgramDBEntities.WorkoutDetails.Add(WorkoutDetail);    
                workoutProgramDBEntities.SaveChanges();
                LstWorkoutDetail.Add(WorkoutDetail);
                WorkoutDetail = new WorkoutDetail();
                
            }
            else
            {
                MessageBox.Show("Missing workout information");
            }
        }

        // Save changes made after updating the workout to the View to the database. 
        private void UpdateWorkout(object obj)
        {
            if(SelectedWorkoutDetail != null)
            {
                workoutProgramDBEntities.SaveChanges();
                SelectedWorkoutDetail = new WorkoutDetail();
            }
            else
            {
                MessageBox.Show("No workout selected");
            }
           
        }

        // Selects workout to update, populating the text box witht that workout details.
        // Clears the textboxes after updated.
        private void Select_Update(object obj)
        {
            SelectedWorkoutDetail=obj as WorkoutDetail;
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
        public ICommand SaveCommand { get; set; }




        private void SaveWorkout(object obj)
        {
            // Check if a workout is selected
            if (SelectedWorkoutDetail != null)
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;

                    try
                    {
                        // Create or overwrite the text file
                        using (StreamWriter writer = new StreamWriter(filePath))
                        {
                            // Write workout details to the file
                            writer.WriteLine($"\t\t\tTodays Workout | {DateTime.Now}\n\n");
                            writer.WriteLine($"Workout 1 : \n\t{SelectedWorkoutDetail.Workout1}\n");
                            writer.WriteLine($"\tSets / Reps : {SelectedWorkoutDetail.Sets_Reps1}\n");
                            writer.WriteLine($"\tWeight (lbs) : {SelectedWorkoutDetail.Weight1} \n");
                            writer.WriteLine($"\t\t\t**** 45 seconds rest after each set *****\n\n");
                            writer.WriteLine($"Workout 2 : \n\t{SelectedWorkoutDetail.Workout2}\n");
                            writer.WriteLine($"\tSets / Reps : {SelectedWorkoutDetail.Sets_Reps2}\n");
                            writer.WriteLine($"\tWeight (lbs) : {SelectedWorkoutDetail.Weight2} \n");
                            writer.WriteLine($"\t\t\t**** 45 seconds rest after each set *****\n\n");
                            writer.WriteLine($"Workout 3 : \n\t{SelectedWorkoutDetail.Workout3}\n");
                            writer.WriteLine($"\tSets / Reps : {SelectedWorkoutDetail.Sets_Reps3}\n");
                            writer.WriteLine($"\tWeight (lbs) : {SelectedWorkoutDetail.Weight3} \n\n\n\n");
                            writer.WriteLine($"\t\t\t\t***** Great work! *****");
                        }
                        
                        SelectedWorkoutDetail = new WorkoutDetail();

                        MessageBox.Show("Workout saved successfully as a text file.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("No workout selected");
            }

        }
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


