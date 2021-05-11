using System;
using System.Collections;
using System.Windows.Forms;

namespace BNS_Tools_ALL.Entity
{
    class ListViewItemComparer : IComparer
    {
        private int col;
        private SortOrder order;
        public ListViewItemComparer()
        {
            col = 0;
            order = SortOrder.Ascending;
        }
        public ListViewItemComparer(int column, SortOrder order)
        {
            col = column;
            this.order = order;
        }
        public int Compare(object x, object y)
        {
            int returnVal = -1;
            float a = 0, b = 0;
            if (float.TryParse(((ListViewItem)x).SubItems[col].Text, out a) && float.TryParse(((ListViewItem)y).SubItems[col].Text, out b))
            {
                returnVal = a >= b ? (a == b ? 0 : 1) : -1;
                if (order == SortOrder.Descending)
                {
                    returnVal *= -1;
                }
            }
            else
            {
                returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
                                        ((ListViewItem)y).SubItems[col].Text);
                // Determine whether the sort order is descending.
                if (order == SortOrder.Descending)
                {
                    // Invert the value returned by String.Compare.
                    returnVal *= -1;
                }
            }
            return returnVal;
        }
    }
}
