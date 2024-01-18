using KanbanView_Project.Kanban.DataModel;
using KanbanView_Project.Kanban.Notification;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using ZTasksLibrary;
using static KanbanView_Project.Kanban.Util.Records.RecordModifiers;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace KanbanView_Project.Kanban.Control
{
    public sealed partial class KanbanBoardControl : UserControl
    {
        public event Action DragStarted;
        public event Action DragCompleted;
        public event Action<DragDropKanbanCardEventRecord> ItemDropedEvent;
        public KanbanBoardControl()
        {
            this.InitializeComponent();
            dummyCard = new KanbanCard();
            {
                dummyCard.KanbanCardData = (double)40;
               
            }
        }
        private void KanbanCardItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
        }
        bool isdraging = false;
        int DropIndex = -1;
        KanbanCard dummyCard;
        KanbanCard DragedCard;
        int IndexOfDragedCard = -1;
      
        public KanbanBoard KanbanBoardItem
        {
            get { return (KanbanBoard)GetValue(KanbanBoardItemProperty); }
            set { SetValue(KanbanBoardItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KanbanBoardItemProperty =
            DependencyProperty.Register("KanbanBoardItem", typeof(KanbanBoard), typeof(KanbanBoardControl), new PropertyMetadata(default, KanbanBoardItemChanged));
        public static void KanbanBoardItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var val = e.NewValue as KanbanBoard;
            var ctrl = (KanbanBoardControl)d;
            if (val != null)
            {
                if (val.KanbanBoardData is string vale)
                {
                    ctrl.TransparentGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    ctrl.TransparentGrid.Visibility = Visibility.Collapsed;
                }
            }
        }
        private void BoardListView_DragEnter(object sender, DragEventArgs e)
        {
            //    BoardListView.Visibility = Visibility.Collapsed;
            e.DragUIOverride.IsContentVisible = true;
            e.DragUIOverride.IsCaptionVisible = true;
            e.DragUIOverride.IsGlyphVisible = true;
            e.DragUIOverride.Caption = "Drop here to move to " + KanbanBoardItem.KanbanBoardHeader as string;
            isdraging = true;
        }

        private void BoardListView_DragLeave(object sender, DragEventArgs e)
        {
            DashStroke.Visibility = Visibility.Collapsed;
            DragGrid.Visibility = Visibility.Collapsed;
            BoardListView.Opacity = 1;
            isdraging = false;
        }

        private void BoardListView_DragOver(object sender, DragEventArgs e)
        {
            DashStroke.Visibility = Visibility.Visible;
            DragGrid.Visibility = Visibility.Visible;
            BoardListView.Opacity = 1;
            var draggedItem = e.OriginalSource as ListViewItem;
            if (draggedItem != null)
            {
                draggedItem.Content = "Dragging...";
                draggedItem.FontSize = 18;
                draggedItem.FontWeight = Windows.UI.Text.FontWeights.Bold;
            }
            //  Debug.WriteLine("drag over" + KanbanBoardItem.KanbanCardItems.Count());
       //     Debug.WriteLine(" ___----------" + KanbanBoardItem.KanbanCardItems.Count());
          //  Debug.WriteLine("@@@@@@@@@@@@@@@" + KanbanBoardItem.KanbanCardItems.Count());
            e.AcceptedOperation = DataPackageOperation.Move;
            Point dropPosition = e.GetPosition(BoardListView.ItemsPanelRoot);
            DragOver_Override(dropPosition.X,dropPosition.Y);
        }

        public void DragOver_Override(double pointer_X,double pointer_Y)
        {
            var x = pointer_X;
            var y = pointer_Y;
            try
            {
                if (BoardListView.Items.Count == 0)
                {
                    KanbanBoardItem.KanbanCardItems.Insert(0, dummyCard);
                    DropIndex = 0;
                }
                else
                {
                    ListViewItem firstVisibleItem = BoardListView.ContainerFromIndex(0) as ListViewItem;
                    int index = BoardListView.Items.IndexOf(firstVisibleItem);
                    double itemHeight = firstVisibleItem.ActualHeight;
                    while (pointer_Y > itemHeight)
                    {
                        index++;
                        pointer_Y -= itemHeight;
                    }
                    var item = FindListViewItem(new Point(x,y));
                    if(item != null)
                    {
                        index = BoardListView.Items.IndexOf(item.Content);
                    }
                    if (index > -1)
                    {
                        if(index == 0)
                        {

                        }
                        Debug.WriteLine(DropIndex + "Drop Index");
                        KanbanBoardItem.KanbanCardItems.Remove(dummyCard);
                        KanbanBoardItem.KanbanCardItems.Insert(index, dummyCard);
                        DropIndex = index;

                    }
                   
                    //else
                    //{
                    //    KanbanBoardItem.KanbanCardItems.Remove(dummyCard);
                    //    KanbanBoardItem.KanbanCardItems.Add(dummyCard);
                    //    DropIndex = KanbanBoardItem.KanbanCardItems.Count() - 1;
                    //}
                }



            }
            catch (Exception ex)
            {

                //KanbanBoardItem.KanbanCardItems.Add(dummyCard);
                //DropIndex = KanbanBoardItem.KanbanCardItems.Count() - 1;
            }

        }
        private ListViewItem FindListViewItem(Point pointerPosition)
        {
            // Find the ListViewItem at the pointer position
            ListViewItem listViewItem = null;

            // Iterate through the ListView items
            for (int i = 0; i < BoardListView.Items.Count; i++)
            {
                // Get the ListViewItem for the current index
                ListViewItem item = BoardListView.ContainerFromIndex(i) as ListViewItem;

                if (item != null)
                {
                    // Get the position of the ListViewItem relative to the ListView
                    Rect itemBounds = item.TransformToVisual(BoardListView.ItemsPanelRoot).TransformBounds(new Rect(0, 0, item.ActualWidth, item.ActualHeight));

                    // Check if the pointer position is within the bounds of the ListViewItem
                    if (itemBounds.Contains(pointerPosition))
                    {
                        listViewItem = item;
                        break;
                    }
                }
            }
            return listViewItem;
        }
        private void BoardListView_Drop(object sender, DragEventArgs e)
        {
            DashStroke.Visibility = Visibility.Collapsed;
            DragGrid.Visibility = Visibility.Collapsed;
            BoardListView.Opacity = 1;          //  e.AcceptedOperation = DataPackageOperation.Move;
            if (KanbanBoardItem.KanbanCardItems.Contains(DragedCard))
            {
                DragDropKanbanCardEventRecord dragDropEventRecor = new DragDropKanbanCardEventRecord(DropIndex, DragedCard, true, KanbanBoardItem);
                ItemDropedEvent?.Invoke(dragDropEventRecor);
            }
            else
            {
                DragDropKanbanCardEventRecord dragDropEventRecor = new DragDropKanbanCardEventRecord(DropIndex, DragedCard, false, KanbanBoardItem);
                ItemDropedEvent?.Invoke(dragDropEventRecor);
            }
            KanbanBoardItem.KanbanCardItems.Remove(dummyCard);

            DropIndex = -1;
        }

        private void BoardListView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            var s = args.Items[0];
            BoardListView.PointerMoved -= BoardListView_PointerMoved;
            s = default;
            DashStroke.Visibility = Visibility.Collapsed;
            DragGrid.Visibility = Visibility.Collapsed;
            BoardListView.Opacity = 1; DragCompleted?.Invoke();
            isdraging = false;
            DropIndex = -1;
            KanbanBoardItem.KanbanCardItems.Remove(dummyCard);

            if (args.DropResult == DataPackageOperation.None)
            {
                KanbanBoardItem.KanbanCardItems.Insert(IndexOfDragedCard,DragedCard);
            }
            KanbanNotification.NotifyKanbanCardDroped();


        }

        private void BoardListView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            DragStarted?.Invoke();
            KanbanBoardItem.KanbanCardItems.Remove(dummyCard);

            isdraging = true;
            BoardListView.PointerMoved -= BoardListView_PointerMoved;
            BoardListView.PointerMoved += BoardListView_PointerMoved;
            var dragedKanbanCard = e.Items[0] as KanbanCard;
            int indexOfDragedKanbanCard = KanbanBoardItem.KanbanCardItems.IndexOf(dragedKanbanCard);
            KanbanNotification.NotifyKanbanCardDraged(e.Items[0] as KanbanCard, indexOfDragedKanbanCard);
            //   KanbanBoardItem.KanbanCardItems.Remove(DragedCard);
            ListViewItem listViewItem = (ListViewItem)BoardListView.ContainerFromItem(dragedKanbanCard);
            if (listViewItem != null)
            {
                var cardControl = (KanbanCardControl)listViewItem.ContentTemplateRoot;
               // Debug.WriteLine($" Height  --   {cardControl.ActualHeight} ");
              
            }
        }

        private void BoardListView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void OnWidthResized(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            double wt = MainGrid.Width + e.Delta.Translation.X;
            MainGrid.Width = wt;

        }
        int pointermoved = 0;
        private void BoardListView_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (isdraging)
            {
               // Debug.WriteLine("pointer moved capture " + ++pointermoved);
                var dropPosition = e.GetCurrentPoint(BoardListView.ItemsPanelRoot);
                // Debug.WriteLine(dropPosition);

                DragOver_Override(dropPosition.RawPosition.X,dropPosition.RawPosition.Y);
            }


        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            RegisterNotification();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            DeRegisterNotification();
        }

        public void RegisterNotification()
        {
            DeRegisterNotification();
            KanbanNotification.KanbanCardDraged += KanbanNotification_KanbanCardDraged;
            KanbanNotification.KanbanCardDroped += KanbanNotification_KanbanCardDroped;
        }

     

        public void DeRegisterNotification()
        {
            KanbanNotification.KanbanCardDraged -= KanbanNotification_KanbanCardDraged;
            KanbanNotification.KanbanCardDroped -= KanbanNotification_KanbanCardDroped;
        }

        private void KanbanNotification_KanbanCardDraged(KanbanCard card, int index)
        {
            DragedCard = card;
            IndexOfDragedCard = index;
        }
        private void KanbanNotification_KanbanCardDroped()
        {
            DragedCard = default;
            IndexOfDragedCard = -1;
        }

        private void MainGrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
           

        }

        private void MainGrid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
         //   DashStroke.Visibility = Visibility.Collapsed;
            //BoardListView.Visibility = Visibility.Visible;
        }
    }
}
