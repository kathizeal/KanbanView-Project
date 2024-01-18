using CommonLibrary;
using KanbanView_Project.Kanban.Controller.Contract;
using KanbanView_Project.Kanban.DataModel;
using KanbanView_Project.Kanban.View.Contract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanbanView_Project.Kanban.ViewModel.Contract
{
    public class KanbanViewModelBase : ObservableObject
    {
        public IKanbanViewController ViewController { get; set; }
        public IKanbanServiceController ServiceController { get; set; }

        public ObservableCollection<KanbanBoard> KanbanBoardItems { get; set; } = new ObservableCollection<KanbanBoard>();


        public KanbanViewModelBase(IKanbanViewController view)
        {
            ViewController = view;
        }
    }
}
