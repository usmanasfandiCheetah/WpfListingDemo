using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfListingDemo
{
    public class TaskOperation
    {
        public List<TaskModel> UpdatedTasks { get; set; }
        public List<TaskModel> DeletedTasks { get; set; }
        public List<TaskModel> NewTasks { get; set; }

        public TaskOperation()
        {
            UpdatedTasks = new List<TaskModel>();
            DeletedTasks = new List<TaskModel>();
            NewTasks = new List<TaskModel>();
        }
    }
}
