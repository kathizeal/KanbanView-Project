using KanbanView_Project.Kanban.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanbanView_Project.Kanban.Util.Records
{
    public static class RecordModifiers
    {
        public record DragDropKanbanCardEventRecord
        {
            public int DropedIndex;
            public KanbanCard DropedCard;
            public bool SameList;
            public KanbanBoard BoardData;
            public DragDropKanbanCardEventRecord(int dropedIndex, KanbanCard dropedData, bool sameList, KanbanBoard boardData)
            {
                DropedIndex = dropedIndex;
                DropedCard = dropedData;
                SameList = sameList;
                BoardData = boardData;
            }
        }

        public record DragDrogKanbanBoardEventRecord 
        {
            public int DropedIndex;
            public KanbanBoard BoardData;
            public DragDrogKanbanBoardEventRecord(int dropedIndex,KanbanBoard boardData)
            {
                DropedIndex = dropedIndex;
                BoardData = boardData;
            }
        }

    }
}
