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
    public sealed partial class KanbanTaskItemControl : UserControl
    {
        public KanbanTaskItemControl()
        {
            this.InitializeComponent();
        }
        public event Action AddButtonClickEvent;

        public KanbanTask Task
        {
            get { return (KanbanTask)GetValue(TaskProperty); }
            set { SetValue(TaskProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TaskProperty =
            DependencyProperty.Register("Task", typeof(KanbanTask), typeof(KanbanTaskItemControl), new PropertyMetadata(default));

        private void TitleTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TasksKanbanNotification.TaskKanbanBoardWidthChanged += TasksKanbanNotification_TaskKanbanBoardWidthChanged;
        }

        private void TasksKanbanNotification_TaskKanbanBoardWidthChanged(double width)
        {
            MainPanel.Width = width;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            TasksKanbanNotification.TaskKanbanBoardWidthChanged -= TasksKanbanNotification_TaskKanbanBoardWidthChanged;
        }
    }
}
