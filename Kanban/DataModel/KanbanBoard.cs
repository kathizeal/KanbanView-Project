using CommonLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace KanbanView_Project.Kanban.DataModel
{
    public class KanbanBoard : ObservableObject
    {
        public DataTemplate HeaderTemplate { get; set; }
        public DataTemplate ListViewItemTemplate { get; set; }
        public dynamic KanbanBoardHeader { get; set; }
        public dynamic KanbanBoardData { get; set; }
        public ObservableCollection<KanbanCard> KanbanCardItems { get; set; }
        public KanbanBoard()
        {
            KanbanCardItems = new ObservableCollection<KanbanCard>();
        }

        public void AddKanbanCard(dynamic data)
        {
            KanbanCard kanbanCard = new KanbanCard
            {
                KanbanCardData = data,
                CardItemTemplate = ListViewItemTemplate,

            };
            KanbanCardItems.Add(kanbanCard);
        }

        public void AddKanbanCardRange(IEnumerable<dynamic> dataList)
        {
            foreach (var cur in dataList)
            {
                AddKanbanCard(cur);
            }
        }

        public void InsertKanbanCard(int index, KanbanCard data)
        {
            data.CardItemTemplate = ListViewItemTemplate;
            KanbanCardItems.Insert(index, data);
        }

        public KanbanCard FindKanbanCard(dynamic finderValue, Func<dynamic, dynamic, bool> predicate)
        {
            return KanbanCardItems.FirstOrDefault(t => predicate(t.KanbanCardData, finderValue));
            
        }

        public void RemoveKanbanCard(object finderValue, Func<dynamic, dynamic, bool> predicate)
        {
            var card = KanbanCardItems.FirstOrDefault(t => predicate(t.KanbanCardData, finderValue));
            if(card != null)
            {
                KanbanCardItems.Remove(card);
            }
        }
    }
}
