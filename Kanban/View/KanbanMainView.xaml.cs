using CommonLibrary;
using KanbanView_Project.Kanban.Controller.Contract;
using KanbanView_Project.Kanban.DataModel;
using KanbanView_Project.Kanban.Notification;
using KanbanView_Project.Kanban.View.Contract;
using KanbanView_Project.Kanban.ViewModel;
using KanbanView_Project.Kanban.ViewModel.Contract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.Extensions;
using static KanbanView_Project.Kanban.Util.Records.RecordModifiers;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace KanbanView_Project.Kanban.View
{
    public sealed partial class KanbanMainView : UserControl, IKanbanViewController
    {
        KanbanBoard DummyBoard;
        bool isdraging = false;
        int DropIndex = -1;
        KanbanBoard DragedBoard;
        int IndexOfDragedBoard = -1;
        bool isCardDragInProgress = false;

        private KanbanViewModelBase _VM;
        public KanbanMainView()
        {
            this.InitializeComponent();
            DummyBoard = new KanbanBoard() { KanbanBoardData = "Drag" };
        }
        


        public IKanbanServiceController ServiceController
        {
            get { return (IKanbanServiceController)GetValue(ServiceControllerProperty); }
            set { SetValue(ServiceControllerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ServiceControllerProperty =
            DependencyProperty.Register("ServiceController", typeof(IKanbanServiceController), typeof(KanbanMainView), new PropertyMetadata(default, ServiceControllerPropertyChanged));

       
        private static void ServiceControllerPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var serivceController = args.NewValue as IKanbanServiceController;
            if(dependencyObject is KanbanMainView ctrl)
            {
                ctrl.Initialize();
                ctrl._VM.ServiceController = serivceController;
                ctrl._VM.ServiceController.LoadViewController(ctrl);
                ctrl.SetSource();
            }
        }



        public ObservableCollection<KanbanBoard> KanbanBoardItems
        {
            get { return (ObservableCollection<KanbanBoard>)GetValue(KanbanBoardItemsProperty); }
            set { SetValue(KanbanBoardItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for KanbanBoardItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KanbanBoardItemsProperty =
            DependencyProperty.Register("KanbanBoardItems", typeof(ObservableCollection<KanbanBoard>), typeof(KanbanMainView), new PropertyMetadata(default));


        public void Initialize()
        {
            if(_VM == null)
            {
                _VM = new KanbanViewModel(this);
            }
        }
        public void SetSource()
        {
            KanbanBoardItems = _VM.KanbanBoardItems;
        }


        public void AddKanbanBoardItem(KanbanBoard item)
        {
            _VM.KanbanBoardItems.Add(item);
        }

        public void PopulateKanbanBoardList(ObservableCollection<KanbanBoard> list)
        {
            _VM.KanbanBoardItems.Clear();
            _VM.KanbanBoardItems.AddRange(list);
        }

        public void Dispose()
        {
          
        }

        private void KanbanBoardControl_ItemDropedEvent(DragDropKanbanCardEventRecord dragDropRecord)
        {
            _VM.ServiceController.CardItemDragAndDroped(dragDropRecord);
        }

        private void KanbanListView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            if (!isCardDragInProgress)
            {
                if (DummyBoard != null)
                {
                    _VM.KanbanBoardItems.Remove(DummyBoard);
                }
                isdraging = true;
                KanbanListView.PointerMoved -= KanbanListView_PointerMoved;
                KanbanListView.PointerMoved += KanbanListView_PointerMoved;
                DragedBoard = e.Items[0] as KanbanBoard;
                IndexOfDragedBoard = _VM.KanbanBoardItems.IndexOf(DragedBoard);
                _VM.KanbanBoardItems.Remove(DragedBoard);
            }
          
        }

        private void KanbanListView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            if (!isCardDragInProgress)
            {
                isdraging = false;
                KanbanListView.PointerMoved -= KanbanListView_PointerMoved;
                if (DummyBoard != null)
                {
                    _VM.KanbanBoardItems.Remove(DummyBoard);
                }
                if (args.DropResult == DataPackageOperation.None)
                {
                    _VM.KanbanBoardItems.Insert(IndexOfDragedBoard, DragedBoard);
                }
                else if(args.DropResult == DataPackageOperation.Move && DropIndex > -1)
                {
                    DragDrogKanbanBoardEventRecord dragDrogKanbanBoardEventRecord = new DragDrogKanbanBoardEventRecord(DropIndex, DragedBoard);
                    _VM.ServiceController.BoardItemDragAndDrop(dragDrogKanbanBoardEventRecord);
                }
                DropIndex = -1;
            }

        }

        private void KanbanListView_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!isCardDragInProgress)
            {
                if (isdraging)
                {
                    e.Handled = true;
                    var dropPosition = e.GetCurrentPoint(KanbanListView.ItemsPanelRoot);
                    Debug.WriteLine(dropPosition);
                    DragOver_Override(dropPosition.Position.X);
                }
            }
           
        }

        public void DragOver_Override(double pointer_X)
        {
            if (DummyBoard != null)
            {
                _VM.KanbanBoardItems.Remove(DummyBoard);
            }

            try
            {
                var scrollViewer = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(KanbanListView, 0), 0) as ScrollViewer;
                var positionX = scrollViewer.VerticalOffset + pointer_X;
                var index = GetItemIndex(positionX, KanbanListView);
                if (index > -1)
                {
                    Debug.WriteLine(DropIndex + "Drop Index");
                    _VM.KanbanBoardItems.Insert(index, DummyBoard);
                    DropIndex = index;
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());

            }
        }
        int GetItemIndex(double positionY, ListView targetListView)
        {
            var index = 0;
            double height = 0;

            foreach (var item in targetListView.Items)
            {
                height += GetColumnWidth(item, targetListView);
                if (height > positionY)
                {
                    break;
                }
                index++;
            }

            return index;
        }

        double GetColumnWidth(object listItem, ListView targetListView)
        {
            var listItemContainer = targetListView.ContainerFromItem(listItem) as ListViewItem;
            var height = listItemContainer.ActualWidth;
            var marginTop = listItemContainer.Margin.Left;
            return marginTop + height;
        }

        private void KanbanListView_DragLeave(object sender, DragEventArgs e)
        {
            if (!isCardDragInProgress)
            {
                if (DummyBoard != null)
                {
                    _VM.KanbanBoardItems.Remove(DummyBoard);
                }
            }
           
        }

        private void KanbanListView_DragOver(object sender, DragEventArgs e)
        {

            if (!isCardDragInProgress)
            {
                if (DummyBoard != null)
                {
                    _VM.KanbanBoardItems.Remove(DummyBoard);
                }
                e.AcceptedOperation = DataPackageOperation.Move;
                Point dropPosition = e.GetPosition(KanbanListView.ItemsPanelRoot);
                DragOver_Override(dropPosition.X);
            }
           
        }

        private void KanbanListView_Drop(object sender, DragEventArgs e)
        {
            KanbanListView.PointerMoved -= KanbanListView_PointerMoved;
            if (DummyBoard != null)
            {
                _VM.KanbanBoardItems.Remove(DummyBoard);
            }
        }

        private void KanbanBoardControl_DragStarted()
        {
            isCardDragInProgress = true;
          //  KanbanListView.DragItemsStarting -= KanbanListView_DragItemsStarting;
        //    KanbanListView.PointerMoved -= KanbanListView_PointerMoved;
        }

        private void KanbanBoardControl_DragCompleted()
        {
           // KanbanListView.DragItemsStarting += KanbanListView_DragItemsStarting;
            isCardDragInProgress = false;
        }

        private void KanbanListView_Loaded(object sender, RoutedEventArgs e)
        {
            RegisterScrollViewerEvents();
        }
        ScrollViewer KanbanListViewScroller;

        private void RegisterScrollViewerEvents()
        {
            if (KanbanListViewScroller == null)
            {
                KanbanListViewScroller = KanbanListView.GetScrollViewer();
            }

            if (KanbanListViewScroller == default) { return; }
            KanbanListViewScroller.ViewChanged -= OnTaskListScrolled;
            KanbanListViewScroller.ViewChanged += OnTaskListScrolled;
        }

        public void DeRegisterScrollViewerEventsForGroupedView()
        {
            if (KanbanListViewScroller == null)
            {
                KanbanListViewScroller = KanbanListView.GetScrollViewer();
            }
            if (KanbanListViewScroller == default) { return; }
            KanbanListViewScroller.ViewChanged -= OnTaskListScrolled;
        }
        private void OnTaskListScrolled(object sender, ScrollViewerViewChangedEventArgs e)
        {
            try
            {
                var itempanel = KanbanListView.ItemsPanelRoot as ItemsStackPanel;
                int lastVisibleItemIndex = itempanel.LastVisibleIndex;

                

                if (CanLoadMore() && !e.IsIntermediate)
                {
                    _VM.ServiceController.FetchMoreBoard();
                    //_VM.Logger.Debug(LogManager.GetCallerInfo(), "Task Load more called due to scroll");
                    //_VM.LoadMoreTasks();
                }
            }
            catch (Exception ex)
            {
            }


            bool CanLoadMore() => (true && IsScrollingThresholdMet());


            bool IsScrollingThresholdMet()
            {
                double d;
                var thresholdIndex = d = _VM.KanbanBoardItems.Count * 0.25;
                thresholdIndex = _VM.KanbanBoardItems.Count - thresholdIndex;
                var val = KanbanListView.ItemsPanelRoot is ItemsStackPanel itemsPanel && itemsPanel.LastVisibleIndex >= thresholdIndex;
                if (val)
                {

                }
                return val;
            }
        }
    }
}
