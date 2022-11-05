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
        
        public TaskModel CurrentTask { get => _currentTask; set => SetProperty(ref _currentTask,value); }
        public ObservableCollection<TaskModel> TaskList { get => _taskList; set => SetProperty(ref _taskList,value); }
        DataService _service;
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
            _service = new DataService(DATA_FILE);
            _service.DataAdded += _service_DataAdded;
            _service.DataUpdated += _service_DataUpdated;
            _service.DataRemoved += _service_DataRemoved;

            StartCommand = new RelayCommand<object>(new Action<object?>(ExecStartCommand));
            StopCommand = new RelayCommand<object>(new Action<object?>(ExecStopCommand));
            EditCommand = new RelayCommand<object>(new Action<object?>(ExecEditCommand));
            CopyCommand = new RelayCommand<object>(new Action<object?>(ExecCopyCommand));
            DeleteCommand = new RelayCommand<object>(new Action<object?>(ExecDeleteCommand));

            _service.Start();
        }

        private void _service_DataRemoved(object? sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (sender == null)
                    return;

                TaskModel item = (TaskModel)sender;
                TaskList.Remove(item);
            });
        }

        private void _service_DataUpdated(object? sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (sender == null)
                    return;

                TaskModel item = (TaskModel)sender;
                var obj = TaskList.Where(x => x.ID == item.ID).FirstOrDefault();
                if (obj != null)
                {
                    obj.Website = item.Website;
                    obj.Size = item.Size;
                    obj.Keywords = item.Keywords;
                    obj.Proxy = item.Proxy;
                    obj.BillingProfile = item.BillingProfile;
                }
            });
        }

        private void _service_DataAdded(object? sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (sender == null)
                    return;

                TaskModel item = (TaskModel)sender;
                TaskList.Add(item);
            });
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
    }
}
