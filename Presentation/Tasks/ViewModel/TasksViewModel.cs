using KanbanView_Project.Presentation.Tasks.View;
using KanbanView_Project.Presentation.Tasks.ViewModel.Contract;
using ZKanbanViewLibrary;
using CommonLibrary;
using ZTasksLibrary.Common;
using System.Collections.Generic;
using System.Linq;
using ZTasksLibrary;
using System.Collections.ObjectModel;
using System;
using KanbanView_Project.Presentation.Tasks.Notification;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Automation;
using System.Collections;
using System.Threading.Tasks;
using KanbanView_Project.Kanban.Util.Records;

namespace KanbanView_Project.Presentation.Tasks.ViewModel
{
    public class TasksViewModel : TasksViewModelBase
    {
        public TasksViewModel(ITaskMainView view) : base(view)
        {
        }
        public override void RegisterNotification()
        {
            DeRegisterNotification();
            TasksKanbanNotification.TaskCreated += TasksKanbanNotification_TaskCreated;
        }


        public override void DeRegisterNotification()
        {
            TasksKanbanNotification.TaskCreated -= TasksKanbanNotification_TaskCreated;
        }
        private void TasksKanbanNotification_TaskCreated(string title, TaskGroup group)
        {
            KanbanTask kanbanTask = new KanbanTask()
            {
                Title = title,
                DueDateInMilliseconds = DateTimeOffset.Now,
                Description = $"Task created with Time of {DateTimeOffset.Now.ToString("mm hh dd MM")}",
            };
            switch (group.TaskGroupHeader.GroupedBy)
            {
                case TasksGroupEnum.Status:
                    {
                        kanbanTask.Status = group.TaskGroupHeader.GroupHeaderName;
                    }
                    break;
                case TasksGroupEnum.Priority:
                    {
                        kanbanTask.Priority = group.TaskGroupHeader.GroupHeaderName;
                    }
                    break;


            }
            AddTask(kanbanTask);
        }

        public override void AddTask(KanbanTask task)
        {
            TasksServiceManager.AddTasks(task);
            Tasks.Add(task);
            var comparer = string.Empty;
            switch (GroupedBy)
            {
                case TasksGroupEnum.Status:
                    comparer = task.Status;
                    break;
                case TasksGroupEnum.Priority:
                    comparer = task.Priority;
                    break;
            }
            InsertTaskInGroupedList(task, comparer);
        }

        public void InsertTaskInGroupedList(KanbanTask task, string comparer)
        {
           
            foreach(var group in TasksListGroup)
            {
                if(group.TaskGroupHeader.GroupHeaderName == comparer) 
                {
                    group.Add(task);
                    TasksKanbanController.AddTask(task, comparer);
                    return;
                }
            }
            var groupheader = new TaskGroup();
            groupheader.TaskGroupHeader = new TaskGroupHeader()
            {
                GroupHeaderName = comparer,
                GroupedBy = GroupedBy
            };
            groupheader.Add(task);
            TasksListGroup.Add(groupheader);
            TasksKanbanController.AddBoard(groupheader);
        }



        public override void FetchTasks()
        {
            startIndex = 0;
            
            var tasks = TasksServiceManager.FetchTasksFromDB(0, startIndex + GrowthIndexConst);
            startIndex = startIndex + GrowthIndexConst;
            if (tasks == null || tasks?.Count == 0)
            {
                LoadDefaultData();
            }
            else
            {
                Tasks.AddRange(tasks);
                PopulateListInGroupedList();
            }

        }
        public void LoadDefaultData()
        {
            Dictionary<string, int> tasksList = new Dictionary<string, int>();
            tasksList.Add("2", 10);
            tasksList.Add("3", 100);
            tasksList.Add("4", 100);
            tasksList.Add("5", 100);
            tasksList.Add("6", 100);
            tasksList.Add("7", 100);
            tasksList.Add("8", 100);
            tasksList.Add("9", 100);
            tasksList.Add("10", 100);
            tasksList.Add("11", 100);
            tasksList.Add("12", 100);
            tasksList.Add("13", 100);
            tasksList.Add("14", 100);
            tasksList.Add("15", 100);
            tasksList.Add("16", 100);
            tasksList.Add("17", 100);
            tasksList.Add("18", 100);
            tasksList.Add("19", 100);
            tasksList.Add("20", 100);
            foreach (var group in tasksList)
            {

                var groupheader = new TaskGroup();
                groupheader.TaskGroupHeader = new TaskGroupHeader()
                {
                    GroupHeaderName = group.Key,
                    GroupedBy = TasksGroupEnum.Status
                };
                TasksListGroup.Add(groupheader);
            }
            TasksKanbanController.PopulateBoard(TasksListGroup);
        }
        public void PopulateListInGroupedList()
        {
            IEnumerable<IGrouping<string, KanbanTask>> groupedTasks = default;
            switch (GroupedBy)
            {
                case TasksGroupEnum.Status:
                    {
                        groupedTasks = Tasks.GroupBy(t => t.Status);
                    }
                    break;
                case TasksGroupEnum.Priority:
                    {

                        groupedTasks = Tasks.GroupBy(t => t.Priority);
                    }
                    break;
            }
            if (groupedTasks != null)
            {
                foreach (var group in groupedTasks)
                {

                    var groupheader = new TaskGroup();
                    groupheader.TaskGroupHeader = new TaskGroupHeader()
                    {
                        GroupHeaderName = group.Key,
                        GroupedBy = TasksGroupEnum.Status
                    };
                    foreach (var item in group)
                    {
                        groupheader.Add(item);

                    }
                    TasksListGroup.Add(groupheader);
                }
            }
            TasksKanbanController.PopulateBoard(TasksListGroup);
        }
        public override void CreateTasksFromGlobal(string title, string groupHeader)
        {
            KanbanTask task = new KanbanTask();
            {
                task.Title = title;
            }
            switch (GroupedBy)
            {
                case TasksGroupEnum.Status:
                    {
                        task.Status = groupHeader;
                    }
                    break;
                case TasksGroupEnum.Priority:
                    {

                        task.Priority = groupHeader;
                    }
                    break;
            }
            AddTask(task);
        }

        public override void KanbanTaskDragDrop(RecordModifiers.DragDropKanbanCardEventRecord data)
        {
            var kanbanTask = data.DropedCard.KanbanCardData as KanbanTask;
            TaskGroup DropHeader = data.BoardData.KanbanBoardData as TaskGroup;
            var comparer = DropHeader.TaskGroupHeader.GroupHeaderName;
            var previousValue = string.Empty;
            switch (GroupedBy)
            {
                case TasksGroupEnum.Status:
                    previousValue = kanbanTask.Status;
                    kanbanTask.Status = comparer;
                    break;
                case TasksGroupEnum.Priority:
                    previousValue = kanbanTask.Status;
                    kanbanTask.Priority = comparer;
                    break;
            }
            foreach (var grp in TasksListGroup)
            {
                if (grp.TaskGroupHeader.GroupHeaderName == previousValue)
                {
                    grp.Remove(kanbanTask);
                    TasksKanbanController.RemoveTask(previousValue, kanbanTask.TaskId);
                    break;
                }
            }
            foreach (var grp in TasksListGroup)
            {
                if (grp.TaskGroupHeader.GroupHeaderName == comparer)
                {
                    grp.Insert(data.DropedIndex,kanbanTask);
                    TasksKanbanController.InsertTask(data.DropedIndex,comparer,data.DropedCard);
                    break;
                }
            }
        }

        public override void TaskGroupDragDrop(RecordModifiers.DragDrogKanbanBoardEventRecord data)
        {
            var taskGroup = data.BoardData.KanbanBoardData as TaskGroup;
            TasksListGroup.Move(TasksListGroup.IndexOf(taskGroup),data.DropedIndex);
            TasksKanbanController.MoveKanbanBoard(data.BoardData, data.DropedIndex);
        }

        public override void FetchTaskGroup()
        {
            var tasks = TasksServiceManager.FetchTasksFromDB(0, startIndex + GrowthIndexConst);
            startIndex = startIndex + GrowthIndexConst;
            
            {
                Tasks.AddRange(tasks);
                PopulateListInGroupedList();
            }
        }

    }
}
