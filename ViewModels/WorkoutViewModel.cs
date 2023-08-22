﻿using System;
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
//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Wordprocessing;
//using DocumentFormat.OpenXml;

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
           // SaveCommand = new Command((s) => true, SaveWorkout);
        }



        // Add workout to the database. 
        private void AddWorkout(object obj)
        {
            WorkoutDetail.ID = "WO" + workoutProgramDBEntities.WorkoutDetails.Count();
            if(WorkoutDetail != null)
            {
                workoutProgramDBEntities.WorkoutDetails.Add(WorkoutDetail);
                workoutProgramDBEntities.SaveChanges();
                WorkoutDetail = new WorkoutDetail();
                LstWorkoutDetail.Add(WorkoutDetail);
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
       // public ICommand SaveCommand { get; set; }




        //private void SaveWorkout(object obj)
        //{
        //    // Check if a workout is selected
        //    if (SelectedWorkoutDetail != null)
        //    {
        //        Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
        //        saveFileDialog.Filter = "Word Documents (*.docx)|*.docx|All Files (*.*)|*.*";

        //        if (saveFileDialog.ShowDialog() == true)
        //        {
        //            string filePath = saveFileDialog.FileName;

        //            // Create a Word document
        //            using (WordprocessingDocument doc = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
        //            {
        //                // Add a main document part
        //                MainDocumentPart mainPart = doc.AddMainDocumentPart();
        //                mainPart.Document = new Document();

        //                // Create a body
        //                Body body = mainPart.Document.AppendChild(new Body());

        //                // Add workout details to the document
        //                DocumentFormat.OpenXml.Drawing.Paragraph paragraph = body.AppendChild(new DocumentFormat.OpenXml.Drawing.Paragraph());
        //                DocumentFormat.OpenXml.Drawing.Run run = paragraph.AppendChild(new DocumentFormat.OpenXml.Drawing.Run());
        //                run.AppendChild(new Text($"Workout ID: {SelectedWorkoutDetail.ID}"));
        //                // Add more details as needed

        //                // Save the document
        //                mainPart.Document.Save();
        //            }
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("No workout selected");
        //    }
        //}
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


