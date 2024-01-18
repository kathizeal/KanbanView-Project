using KanbanView_Project.Presentation.Tasks.Notification;
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
using ZTasksLibrary;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace KanbanView_Project.Presentation.Tasks.Control
{
    public sealed partial class TaskKanbanGroupHeaderControl : UserControl
    {
        public TaskKanbanGroupHeaderControl()
        {
            this.InitializeComponent();
        }
        public event Action AddButtonClickEvent;

        public TaskGroup TaskGroup
        {
            get { return (TaskGroup)GetValue(TaskGroupProperty); }
            set { SetValue(TaskGroupProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TaskGroupProperty =
            DependencyProperty.Register("TaskGroup", typeof(TaskGroup), typeof(TaskKanbanGroupHeaderControl), new PropertyMetadata(default));

        private void CreateButtton_Click(object sender, RoutedEventArgs e)
        {
            CreateTask();
        }

        private void MoreButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddTaskEditor_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                CreateTask();
            }
        }

        public void CreateTask()
        {
            if (!string.IsNullOrWhiteSpace(AddTaskEditor.Text))
            {
                TasksKanbanNotification.NotifyTaskCreated(AddTaskEditor.Text, TaskGroup);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TasksKanbanNotification.TaskKanbanBoardWidthChanged += TasksKanbanNotification_TaskKanbanBoardWidthChanged;
        }

        private void TasksKanbanNotification_TaskKanbanBoardWidthChanged(double width)
        {
            Maingrid.Width = width;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            TasksKanbanNotification.TaskKanbanBoardWidthChanged -= TasksKanbanNotification_TaskKanbanBoardWidthChanged;
        }
    }
}
