using KanbanView_Project.Kanban.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace KanbanView_Project.Kanban.Control
{
    public sealed partial class KanbanCardControl : UserControl
    {
        public KanbanCardControl()
        {
            this.InitializeComponent();
        }
        public KanbanCard KanbanCardItem
        {
            get { return (KanbanCard)GetValue(KanbanCardItemProperty); }
            set { SetValue(KanbanCardItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KanbanCardItemProperty =
            DependencyProperty.Register("KanbanCardItem", typeof(KanbanCard), typeof(KanbanCardControl), new PropertyMetadata(default, KanbanCardItemChanged));

        public static void KanbanCardItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var KanbanCardItem = e.NewValue as KanbanCard;
            var ctrl = (KanbanCardControl)d;
            if (KanbanCardItem != null)
            {
                if (KanbanCardItem.KanbanCardData is double val)
                {
                    ctrl.TransparentGrid.Visibility = Visibility.Visible;
                    ctrl.TransparentGrid.Height = val;
                }
                else
                {
                    ctrl.TransparentGrid.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
