using KanbanView_Project.Kanban.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanbanView_Project.Kanban.View.Contract
{
    public interface IKanbanViewController : IDisposable
    {
        void AddKanbanBoardItem(KanbanBoard item);
        void PopulateKanbanBoardList(ObservableCollection<KanbanBoard> list);
    }
}
