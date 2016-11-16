using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Graphics;

namespace XenoCore.Engine.GUI
{
    public class GridContainer : GUIContainer
    {
        public class GridRow
        {
            public int Height;
            public float Weight;

            internal int _actualHeight;
            internal int _top;

            public GridRow() { }
            public GridRow(int height) { this.Height = height; }
            public GridRow(float weight) { this.Weight = weight; }
        }
        public class GridColumn
        {
            public int Width;
            public float Weight;

            internal int _actualWidth;
            internal int _left;

            public GridColumn() { }
            public GridColumn(int width) { this.Width = width; }
            public GridColumn(float weight) { this.Weight = weight; }
        }
        class GridData
        {
            public int Row, Column, ColumnSpan, RowSpan;
        }

        public const String GridDataKey = "GridData";

        public List<GridRow> Rows { get; private set; } = new List<GridRow>();
        public List<GridColumn> Columns { get; private set; } = new List<GridColumn>();


        public void SetColumn(GUIControl control, int columnIndex = 0, int columnSpan = 1)
        {
            Object data = null;
            GridData gridData = null;
            if (control.ControlsData.TryGetValue(GridDataKey, out data))
            {
                gridData = data as GridData;
            }
            else
            {
                gridData = new GridData();
                control.ControlsData.Add(GridDataKey, gridData);
            }

            gridData.Column = columnIndex;
            gridData.ColumnSpan = columnSpan;
        }
        public void SetRow(GUIControl control, int rowIndex = 0, int rowSpan = 1)
        {
            Object data = null;
            GridData gridData = null;
            if (control.ControlsData.TryGetValue(GridDataKey, out data))
            {
                gridData = data as GridData;
            }
            else
            {
                gridData = new GridData();
                control.ControlsData.Add(GridDataKey, gridData);
            }

            gridData.Row = rowIndex;
            gridData.RowSpan = rowSpan;
        }

        private void SetColumnsAndRows(ref Rectangle space)
        {
            Vector2 totalWeight = Vector2.Zero;
            foreach (var column in Columns)
                totalWeight.X += column.Weight;
            foreach (var row in Rows)
                totalWeight.Y += row.Weight;

            int pos = 0;

            foreach (var column in Columns)
            {
                column._left = pos;

                if (column.Weight > 0)
                {
                    column._actualWidth = (int)(space.Width * (column.Weight / totalWeight.X));
                    totalWeight.X -= column.Weight;
                }
                else
                    column._actualWidth = column.Width;

                space.Width -= column._actualWidth;

                pos += column._actualWidth;
            }

            pos = 0;

            foreach (var row in Rows)
            {
                row._top = pos;

                if (row.Weight > 0)
                {
                    row._actualHeight = (int)(space.Height * (row.Weight / totalWeight.Y));
                    totalWeight.Y -= row.Weight;
                }
                else
                    row._actualHeight = row.Height;

                space.Height -= row._actualHeight;

                pos += row._actualHeight;
            }
        }
        public override void Update(GUISystemState systemState)
        {

            SetColumnsAndRows(ref State.Bounds);

            int rowIndex, columnIndex, rowSpan, columnSpan;

            var space = State.Bounds;

            foreach (var item in Children)
            {
                if (item.Visiblity != Visiblity.Visible)
                    continue;

                var itemSpace = space;

                Object data = null;
                GridData gridData = null;
                if (item.ControlsData.TryGetValue(GridDataKey, out data))
                {
                    gridData = data as GridData;
                    rowIndex = gridData.Row;
                    columnIndex = gridData.Column;
                    rowSpan = gridData.RowSpan;
                    columnSpan = gridData.ColumnSpan;

                }
                else
                {
                    rowIndex = columnIndex = 0;
                    rowSpan = columnSpan = 0;
                }


                if (Rows.Count > 0)
                {
                    itemSpace.Height = 0;
                    itemSpace.Y = Rows[rowIndex]._top;

                    if (rowSpan == 0)
                        rowSpan = 1;
                }

                if (Columns.Count > 0)
                {
                    itemSpace.Width = 0;
                    itemSpace.X = Columns[columnIndex]._left;

                    if (columnSpan == 0)
                        columnSpan = 1;
                }


                Debug.Assert(rowIndex >= 0 && rowIndex + rowSpan <= Rows.Count, "Invalid grid row settigns!");
                Debug.Assert(columnIndex >= 0 && columnIndex + columnSpan <= Columns.Count, "Invalid grid row settigns!");

                for (int i = rowIndex; i < rowIndex + rowSpan; ++i)
                    itemSpace.Height += Rows[i]._actualHeight;

                for (int i = columnIndex; i < columnIndex + columnSpan; ++i)
                    itemSpace.Width += Columns[i]._actualWidth;


                item.UpdateState(ref itemSpace);

                systemState.AddUpdateControl(this, item);

            }
        }
    }
}
