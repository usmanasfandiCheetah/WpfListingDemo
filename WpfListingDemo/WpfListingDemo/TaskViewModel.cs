using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WpfListingDemo
{
    public class TaskViewModel : ObservableObject
    {
		private ObservableCollection<TaskModel> _taskList;
        private TaskModel _currentTask;
        private bool _isRunning;

        public bool IsRunning { get => _isRunning; set => SetProperty(ref _isRunning, value); }
        public TaskModel CurrentTask { get => _currentTask; set => SetProperty(ref _currentTask,value); }
        public ObservableCollection<TaskModel> TaskList { get => _taskList; set => SetProperty(ref _taskList,value); }
        BackgroundWorker worker;
        string DATA_FILE = Environment.CurrentDirectory + "\\data\\gridtask.dat";

        #region Commands

        public ICommand StartCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand CopyCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        #endregion

        public TaskViewModel()
		{
			TaskList = new ObservableCollection<TaskModel>();
            CurrentTask = new TaskModel();

            StartCommand = new RelayCommand<object>(new Action<object?>(ExecStartCommand));
            StopCommand = new RelayCommand<object>(new Action<object?>(ExecStopCommand));
            EditCommand = new RelayCommand<object>(new Action<object?>(ExecEditCommand));
            CopyCommand = new RelayCommand<object>(new Action<object?>(ExecCopyCommand));
            DeleteCommand = new RelayCommand<object>(new Action<object?>(ExecDeleteCommand));

            worker = new BackgroundWorker();
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.DoWork += Worker_DoWork;

            StartReading();
        }

        private void Worker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            // update ui
            if (e.Result != null)
            {
                var updatedList = (List<TaskModel>)e.Result;
                updatedList.ForEach(item =>
                {
                    var obj = TaskList.Where(x => x.ID == item.ID).FirstOrDefault();
                    if (obj != null)
                    {
                        obj.Website = item.Website;
                        obj.Size = item.Size;
                        obj.Keywords = item.Keywords;
                        obj.Proxy = item.Proxy;
                        obj.BillingProfile = item.BillingProfile;
                    }
                    else
                    {
                        TaskList.Add(item);
                    }
                });
            }

            if (IsRunning)
                worker.RunWorkerAsync(DATA_FILE);
        }

        private void Worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            e.Result = ReadFile(e.Argument.ToString());
        }

        private List<TaskModel> ReadFile(string path)
        {
            //string json = File.ReadAllText(path);
            string json = "";
            using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        json = reader.ReadToEnd();
                    }
                }
            }

            if (string.IsNullOrEmpty(json))
                return null;

            List<TaskModel>? list = JsonSerializer.Deserialize<List<TaskModel>>(json);
            List<TaskModel> tasks = new List<TaskModel>();

            if (list != null)
            {
                list.ForEach(item =>
                {
                    if (!TaskList.Contains(item))
                        tasks.Add(item);
                    else
                    {
                        var obj = TaskList.Where(x => x.ID == item.ID).FirstOrDefault();
                        if (obj != null)
                        {
                            // update existing
                            if (obj.Website.ToLower() != item.Website.ToLower()) { tasks.Add(item); }
                            if (obj.Size.ToLower() != item.Size.ToLower()) { if (!tasks.Contains(item)) { tasks.Add(item); } }
                            if (obj.Keywords.ToLower() != item.Keywords.ToLower()) { if (!tasks.Contains(item)) { tasks.Add(item); } }
                            if (obj.Proxy.ToLower() != item.Proxy.ToLower()) { if (!tasks.Contains(item)) { tasks.Add(item); } }
                            if (obj.BillingProfile.ToLower() != item.BillingProfile.ToLower()) { if (!tasks.Contains(item)) { tasks.Add(item); } }

                        }
                        else
                        {
                            tasks.Add(item);
                        }
                    }
                });
            }

            return tasks;
        }

        private void ExecStartCommand(object? obj)
        {
            if (obj != null)
            {
                CurrentTask = (TaskModel)obj;
                CurrentTask.Started = true;
            }
        }

        private void ExecStopCommand(object? obj)
        {
            if (obj != null)
            {
                CurrentTask = (TaskModel)obj;
                CurrentTask.Started = false;
            }
        }

        private void ExecEditCommand(object? obj)
        {
            if (obj != null)
            {
                CurrentTask = (TaskModel)obj;
                MessageBox.Show("Selected Task ID : " + CurrentTask.ID);
            }
        }

        private void ExecCopyCommand(object? obj)
        {
            if (obj != null)
            {
                CurrentTask = (TaskModel)obj;
                MessageBox.Show("Selected Task ID : " + CurrentTask.ID);
            }
        }

        private void ExecDeleteCommand(object? obj)
        {
            if (obj != null)
            {
                CurrentTask = (TaskModel)obj;
                MessageBox.Show("Selected Task ID : " + CurrentTask.ID);
            }
        }

       

        private void StartReading()
        {
            IsRunning = true;
            worker.RunWorkerAsync(DATA_FILE);            
        }

        private void StopReading()
        {
            IsRunning = false;
        }

    }
}
