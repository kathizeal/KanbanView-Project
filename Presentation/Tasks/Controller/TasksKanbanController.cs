using KanbanView_Project.Kanban.Controller.Contract;
using KanbanView_Project.Kanban.DataModel;
using KanbanView_Project.Kanban.View.Contract;
using KanbanView_Project.Presentation.Tasks.Control;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using ZTasksLibrary.Common;
using ZTasksLibrary;
using CommonLibrary;
using Windows.UI.Xaml.Automation;
using KanbanView_Project.Kanban.Util.Records;
using static KanbanView_Project.Kanban.Util.Records.RecordModifiers;
using System.Collections;
using KanbanView_Project.Kanban.Util;

namespace KanbanView_Project.Presentation.Tasks.Controller
{
    public class TasksKanbanController : IKanbanServiceController
    {

        public event Action<DragDropKanbanCardEventRecord> KanbanTaskDragDroped;
        public event Action<DragDrogKanbanBoardEventRecord> KanbanBoardDragDroped;
        public event Action FetchMoreTaskGroup;
        private TasksViewTemplate Resource;
        public DataTemplate HeaderTemplater;
        public DataTemplate ListViewItemTemplate;
        public ObservableCollection<KanbanBoard> KanbanBoardItems = new ObservableCollection<KanbanBoard>();
        private TasksGroupEnum myVar;

        public TasksGroupEnum GroupedBy
        {
            get { return myVar; }
            set { myVar = value; }
        }

        public TasksKanbanController()
        {
            RegisterNotification();
        }
        IKanbanViewController View;
        public void LoadViewController(IKanbanViewController view)
        {
            View = view;
            LoadTemplates();
           // FetchData();
        }
        public void LoadTemplates()
        {
            HeaderTemplater = GetTasksKanbanResource().GetGroupHeaderTemplate();
            ListViewItemTemplate = GetTasksKanbanResource().GetListViewItemDataTemplateSelector();
        }
        public TasksViewTemplate GetTasksKanbanResource()
        {
            if (Resource == null)
            {
                Resource = new TasksViewTemplate();
            }
            return Resource;
        }
        public void LoadGroupbyList()
        {
           
        }
        public void DeRegisterNotification()
        {
        }



        public void RegisterNotification()
        {
            DeRegisterNotification();
        }

        public void FetchData()
        {
            var tasks = LoadPreSetData();
            var groupedtask = tasks.GroupBy(t => t.Priority);
            var GroupedTaskList = new ObservableCollection<dynamic>();
            foreach (var group in groupedtask)
            {
                KanbanBoard kanbanBoardItem = new KanbanBoard()
                {
                    HeaderTemplate = HeaderTemplater,
                    ListViewItemTemplate = ListViewItemTemplate,

                };
                var groupheader = new TaskGroup();
                groupheader.TaskGroupHeader = new TaskGroupHeader()
                {
                    GroupHeaderName = group.Key,
                    GroupedBy = TasksGroupEnum.Priority
                };
                kanbanBoardItem.KanbanBoardData = groupheader;
                kanbanBoardItem.KanbanBoardHeader = groupheader.TaskGroupHeader.GroupHeaderName;
                foreach (var item in group)
                {
                    KanbanCard kanbanCardItem = new KanbanCard { CardItemTemplate = ListViewItemTemplate, KanbanCardData = item };
                    groupheader.Add(item);
                    kanbanBoardItem.KanbanCardItems.Add(kanbanCardItem);

                }
                GroupedTaskList.Add(groupheader);
                KanbanBoardItems.Add(kanbanBoardItem);

            }
            View.PopulateKanbanBoardList(KanbanBoardItems);
        }

        public static ObservableCollection<KanbanTask> LoadPreSetData()
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
            var lists = new ObservableCollection<KanbanTask>();
            foreach (var grp in tasksList)
            {
                for (int index = 0; index < grp.Value; index++)
                {
                    KanbanTask task = new KanbanTask();
                    task.Title = grp.Key + "_" + index + "_";
                    task.DueDateInMilliseconds = DateTimeOffset.Now.AddDays(index);
                    task.Description = task.Title + task.DueDate;
                    task.Priority = grp.Key;
                    lists.Add(task);
                }
            }

            return lists;
        }

        public void AddBoard(TaskGroup group)
        {
            KanbanBoard kanbanBoardItem = new KanbanBoard()
            {
                HeaderTemplate = HeaderTemplater,
                ListViewItemTemplate = ListViewItemTemplate,
                KanbanBoardData = group,
                KanbanBoardHeader = group.TaskGroupHeader.GroupHeaderName
            };
            kanbanBoardItem.AddKanbanCardRange(group.GetList());
            KanbanBoardItems.Add(kanbanBoardItem);
        }

        public void PopulateBoard(IList<TaskGroup> groups)
        {
            foreach(var group in groups)
            {
                AddBoard(group);
            }
            View.PopulateKanbanBoardList(KanbanBoardItems);
        }


        public void AddTask(KanbanTask task, string comparer)
        {
            foreach(var board in  KanbanBoardItems)
            {
                if(board.KanbanBoardHeader is string header && header == comparer)
                {
                    board.AddKanbanCard(task);
                    break;
                }
            }
        }

        public void InsertTask(int index, string groupHeader, KanbanCard card)
        {
            foreach (var board in KanbanBoardItems)
            {
                if (board.KanbanBoardHeader is string header && header == groupHeader)
                {
                    board.InsertKanbanCard(index, card);
                    break;
                }
            }
        }

        public void RemoveTask(string groupHeader, string taskId)
        {
            foreach (var board in KanbanBoardItems)
            {
                if (board.KanbanBoardHeader is string header && header == groupHeader)
                {
                    board.RemoveKanbanCard(taskId,PredicateForRemoveTask);
                    break;
                }
            }
        }

        public bool PredicateForRemoveTask(dynamic taskId, dynamic data)
        {
            if(taskId is KanbanTask task){
                return task.TaskId == data;
            }
            return false;
        }

        public void CardItemDragAndDroped(DragDropKanbanCardEventRecord data)
        {
            if (data.DropedIndex != -1)
            {
                var task = data.DropedCard?.KanbanCardData as KanbanTask;
                if(task != null)
                {
                    KanbanTaskDragDroped?.Invoke(data);
                }
            }
           
        }

        public void BoardItemDragAndDrop(DragDrogKanbanBoardEventRecord data)
        {
            KanbanBoardDragDroped?.Invoke(data);
        }

        public void MoveKanbanBoard(KanbanBoard board, int index)
        {
            KanbanBoardItems.Insert(index, board);
        }
        public void InsertBoard(int index,KanbanBoard board)
        {
            KanbanBoardItems.Insert(index,board);
        }

        public void FetchMoreBoard()
        {
            FetchMoreTaskGroup?.Invoke();
        }
    }
}
