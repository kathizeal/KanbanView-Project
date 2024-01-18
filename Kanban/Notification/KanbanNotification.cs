using KanbanView_Project.Kanban.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanbanView_Project.Kanban.Notification
{
    public static class KanbanNotification
    {
        public static event Action<KanbanCard, int> KanbanCardDraged;
        public static void NotifyKanbanCardDraged(KanbanCard card, int index)
        {
            KanbanCardDraged?.Invoke(card,index);
        }

        public static event Action KanbanCardDroped;
        public static void NotifyKanbanCardDroped()
        {
            KanbanCardDroped?.Invoke();
        }
    }
}
