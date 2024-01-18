using KanbanView_Project.Kanban.DataModel;
using KanbanView_Project.Kanban.View.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer.DragDrop;
using ZTasksLibrary;
using static KanbanView_Project.Kanban.Util.Records.RecordModifiers;

namespace KanbanView_Project.Kanban.Controller.Contract
{
    public interface IKanbanServiceController
    {
        void LoadViewController(IKanbanViewController view);
        void LoadTemplates();

        void RegisterNotification();
        void DeRegisterNotification();

        void CardItemDragAndDroped(DragDropKanbanCardEventRecord data);
        void BoardItemDragAndDrop(DragDrogKanbanBoardEventRecord data);
        void FetchMoreBoard();
       

    }
}
 