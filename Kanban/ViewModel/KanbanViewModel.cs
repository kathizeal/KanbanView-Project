using KanbanView_Project.Kanban.View.Contract;
using KanbanView_Project.Kanban.ViewModel.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanbanView_Project.Kanban.ViewModel
{
    public class KanbanViewModel : KanbanViewModelBase
    {
        public KanbanViewModel(IKanbanViewController view) : base(view)
        {
        }
    }
}
