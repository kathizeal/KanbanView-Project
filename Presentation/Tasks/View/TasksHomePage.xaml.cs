using KanbanView_Project.Presentation.Tasks.Controller;
using KanbanView_Project.Presentation.Tasks.Notification;
using KanbanView_Project.Presentation.Tasks.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace KanbanView_Project.Presentation.Tasks.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TasksHomePage : Page, ITaskMainView
    {
        private TasksViewModel _VM;
        public TasksHomePage()
        {
            this.InitializeComponent();
            Initialize();
        }
        public void Initialize()
        {
            if(_VM == null)
            {
                _VM = new TasksViewModel(this);
                _VM.RegisterNotification();
                LoadKanbanView();
                LoadTasks();
            }
        }
        public void LoadKanbanView()
        {
            _VM.TasksKanbanController = new TasksKanbanController();
            TasksKanbanView.ServiceController = _VM.TasksKanbanController;
            _VM.TasksKanbanController.GroupedBy = _VM.GroupedBy;
            RegisterKanbanControllerNotification();
        }

        public void RegisterKanbanControllerNotification()
        {
            DeRegisterKanbanControllerNotification();
            _VM.TasksKanbanController.KanbanTaskDragDroped += TasksKanbanController_KanbanTaskDragDroped;
            _VM.TasksKanbanController.KanbanBoardDragDroped += TasksKanbanController_KanbanBoardDragDroped;
            _VM.TasksKanbanController.FetchMoreTaskGroup += TasksKanbanController_FetchMoreTaskGroup;
        }

        

        public void DeRegisterKanbanControllerNotification()
        {
            _VM.TasksKanbanController.KanbanTaskDragDroped -= TasksKanbanController_KanbanTaskDragDroped;
            _VM.TasksKanbanController.KanbanBoardDragDroped -= TasksKanbanController_KanbanBoardDragDroped;
            _VM.TasksKanbanController.FetchMoreTaskGroup -= TasksKanbanController_FetchMoreTaskGroup;
        }
        private void TasksKanbanController_FetchMoreTaskGroup()
        {
            _VM.FetchTaskGroup();
        }
        private void TasksKanbanController_KanbanTaskDragDroped(Kanban.Util.Records.RecordModifiers.DragDropKanbanCardEventRecord data)
        {
            _VM.KanbanTaskDragDrop(data);
        }
        private void TasksKanbanController_KanbanBoardDragDroped(Kanban.Util.Records.RecordModifiers.DragDrogKanbanBoardEventRecord obj)
        {
            _VM.TaskGroupDragDrop(obj);
        }

        public void LoadTasks()
        {
            _VM.FetchTasks();
        }
        public void Dispose()
        {
        }

        private void StatusOptionClicked(object sender, RoutedEventArgs e)
        {
            GroupByDropDown.Content = "Status";
            _VM.GroupedBy = ZTasksLibrary.Common.TasksGroupEnum.Status;

        }

        private void PriorityOptionClicked(object sender, RoutedEventArgs e)
        {
            _VM.GroupedBy = ZTasksLibrary.Common.TasksGroupEnum.Priority;
            GroupByDropDown.Content = "Priority";
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if(_VM != null)
            {
                _VM.DeRegisterNotification();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddTaskButton_CLick(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(TitleTB.Text) || string.IsNullOrWhiteSpace(GroupHeaderTB.Text))
            {
                return;
            }
            _VM.CreateTasksFromGlobal(TitleTB.Text, GroupHeaderTB.Text);
        }

        private void a_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {

        }

        private void WidthSizer_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            TasksKanbanNotification.NotifyTaskKanbanBoardWidthChanged(e.NewValue);
        }
    }
}
