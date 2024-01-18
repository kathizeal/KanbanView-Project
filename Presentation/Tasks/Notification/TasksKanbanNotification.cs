using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZTasksLibrary;

namespace KanbanView_Project.Presentation.Tasks.Notification
{
    public static class TasksKanbanNotification
    {
        public static event Action<string, TaskGroup> TaskCreated;
        public static void NotifyTaskCreated(string title, TaskGroup group)
        {
            TaskCreated?.Invoke(title, group);
        }


        public static event Action<double> TaskKanbanBoardWidthChanged;
        public static void NotifyTaskKanbanBoardWidthChanged(double width) 
        {
            TaskKanbanBoardWidthChanged?.Invoke(width);
        }
    }
}
