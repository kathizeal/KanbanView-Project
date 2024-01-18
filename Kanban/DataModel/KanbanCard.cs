using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace KanbanView_Project.Kanban.DataModel
{
    public class KanbanCard : ObservableObject
    {
        public object KanbanCardData { get; set; }
        public DataTemplate CardItemTemplate { get; set; }
    }
}
