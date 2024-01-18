using KanbanView_Project.Kanban.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanbanView_Project.Kanban.Util
{
    public static class KanbanViewExtension
    {
        public static void AddKanbanCard(this KanbanBoard board, object data)
        {
            KanbanCard kanbanCard = new KanbanCard
            {
                KanbanCardData = data,
                CardItemTemplate = board.ListViewItemTemplate,

            };
            board.KanbanCardItems.Add(kanbanCard);
        }
    }
}
