using CommonLibrary;
using KanbanView_Project.Presentation.Tasks.Controller;
using KanbanView_Project.Presentation.Tasks.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZTasksLibrary;
using ZTasksLibrary.Common;
using static KanbanView_Project.Kanban.Util.Records.RecordModifiers;

namespace KanbanView_Project.Presentation.Tasks.ViewModel.Contract
{
    public abstract class TasksViewModelBase : ObservableObject
    {
        public int startIndex = 0;
        public const int GrowthIndexConst = 10;
        public ITaskMainView View { get; set; }
        public TasksGroupEnum GroupedBy { get; set; } = TasksGroupEnum.Status;
        public TasksViewModelBase(ITaskMainView view)
        {
            View = view;
            TasksListGroup = new ObservableCollection<TaskGroup>();
            Tasks = new ObservableCollection<KanbanTask>();
        }
        public TasksKanbanController TasksKanbanController { get; set; }
        public ObservableCollection<TaskGroup> TasksListGroup { get; set; }
        public ObservableCollection<KanbanTask> Tasks { get; set;}

        public abstract void FetchTasks();
        public abstract void AddTask(KanbanTask task);
        public abstract void RegisterNotification();
        public abstract void DeRegisterNotification();

        public abstract void CreateTasksFromGlobal(string title, string groupHeader);

        public abstract void KanbanTaskDragDrop(DragDropKanbanCardEventRecord data);
        public abstract void TaskGroupDragDrop(DragDrogKanbanBoardEventRecord data);
        public abstract void FetchTaskGroup();
    }
}
