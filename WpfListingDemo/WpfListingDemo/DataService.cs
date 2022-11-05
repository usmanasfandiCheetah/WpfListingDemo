using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WpfListingDemo
{
    public class DataService
    {
        BackgroundWorker worker;
        string DATA_FILE;
        public event EventHandler DataAdded;
        public event EventHandler DataRemoved;
        public event EventHandler DataUpdated;
        public List<TaskModel> CurrentList { get; set; }
        public bool IsRunning { get; set; }

        public DataService(string filepath)
        {
            this.DATA_FILE = filepath;
            CurrentList = new List<TaskModel>();
            worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        public void Start()
        {
            worker.RunWorkerAsync();
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        private void Worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            e.Result = ReadFile();   
        }

        private void Worker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (IsRunning)
                worker.RunWorkerAsync();
        }

        private bool ReadFile()
        {
            //string json = File.ReadAllText(path);
            string json = "";
            using (FileStream stream = File.Open(DATA_FILE, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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
                return false;

            List<TaskModel>? list = JsonSerializer.Deserialize<List<TaskModel>>(json);

            if (list != null)
            {
                list.ForEach(item =>
                {
                    bool changed = false;
                    if (CurrentList.Count(x => x.ID == item.ID) <= 0)
                    {
                        //transaction.NewTasks.Add(item);
                        CurrentList.Add(item);
                        DataAdded?.Invoke(item, null);
                    }
                    else
                    {
                        var obj = CurrentList.Where(x => x.ID == item.ID).FirstOrDefault();
                        if (obj != null)
                        {
                            // update existing
                            if (obj.Website.ToLower() != item.Website.ToLower()) { 
                                obj.Website = item.Website;
                                changed = true;
                            }
                            if (obj.Size.ToLower() != item.Size.ToLower()) {
                                obj.Size = item.Size;
                                changed = true;
                            }
                            if (obj.Keywords.ToLower() != item.Keywords.ToLower()) {
                                obj.Keywords = item.Keywords;
                                changed = true;
                            }
                            if (obj.Proxy.ToLower() != item.Proxy.ToLower()) {
                                obj.Proxy = item.Proxy;
                                changed = true;
                            }
                            if (obj.BillingProfile.ToLower() != item.BillingProfile.ToLower()) {
                                obj.BillingProfile = item.BillingProfile;
                                changed = true;
                            }

                            if (changed)
                                DataUpdated?.Invoke(item, null);
                        }
                        else
                        {
                            CurrentList.Add(item);
                            DataAdded?.Invoke(item, null);
                        }
                    }
                });

                // identifiy deleted items
                var deletedItems = new List<TaskModel>();
                foreach (var item in CurrentList)
                {
                    if (list.Count(x => x.ID == item.ID) <= 0)
                    {
                        deletedItems.Add(item);
                        DataRemoved?.Invoke(item, null);
                    }
                }

                foreach (var item in deletedItems)
                {
                    CurrentList.Remove(item);
                }
            }

            return true;
        }

    }
}