using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Windows.Forms;

namespace FatumStyles
{
    [DefaultEvent("OnSelectedIndexChanged")]
    public class FatumComboBox : UserControl
    {
        private Color backColor = Color.WhiteSmoke;
        private Color iconColor = Color.MediumSlateBlue;
        private Color listBackColor = Color.FromArgb(230, 228, 245);
        private Color listTextColor = Color.DimGray;
        private Color borderColor = Color.MediumSlateBlue;
        private int borderSize = 1;
        private int borderRadius = 10;

        private ComboBox cmbList;
        private Label lblText;
        private Button btnIcon;

        public event EventHandler OnSelectedIndexChanged;

        public FatumComboBox()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

            cmbList = new ComboBox();
            lblText = new Label();
            btnIcon = new Button();
            SuspendLayout();

            cmbList.BackColor = listBackColor;
            cmbList.Font = new Font(Font.Name, 10F);
            cmbList.ForeColor = listTextColor;
            cmbList.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            cmbList.TextChanged += ComboBox_TextChanged;

            btnIcon.Dock = DockStyle.Right;
            btnIcon.FlatStyle = FlatStyle.Flat;
            btnIcon.FlatAppearance.BorderSize = 0;
            btnIcon.BackColor = backColor;
            btnIcon.Size = new Size(30, 30);
            btnIcon.Cursor = Cursors.Hand;
            btnIcon.Click += Icon_Click;
            btnIcon.Paint += Icon_Paint;

            lblText.Dock = DockStyle.Fill;
            lblText.AutoSize = false;
            lblText.BackColor = backColor;
            lblText.TextAlign = ContentAlignment.MiddleLeft;
            lblText.Padding = new Padding(8, 0, 0, 0);
            lblText.Font = new Font(Font.Name, 10F);
            lblText.Click += Surface_Click;
            lblText.MouseEnter += Surface_MouseEnter;
            lblText.MouseLeave += Surface_MouseLeave;

            Controls.Add(lblText);
            Controls.Add(btnIcon);
            Controls.Add(cmbList);
            MinimumSize = new Size(200, 30);
            Size = new Size(200, 30);
            ForeColor = Color.DimGray;
            Padding = new Padding(borderSize);
            Font = new Font(Font.Name, 10F);
            base.BackColor = borderColor;
            Load += FatumComboBox_Load;
            ResumeLayout();
            AdjustComboBoxDimensions();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("FatumStyles Appearance")]
        public new Color BackColor
        {
            get { return backColor; }
            set { backColor = value; lblText.BackColor = value; btnIcon.BackColor = value; Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("FatumStyles Appearance")]
        public Color IconColor
        {
            get { return iconColor; }
            set { iconColor = value; btnIcon.Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("FatumStyles Appearance")]
        public Color ListBackColor
        {
            get { return listBackColor; }
            set { listBackColor = value; cmbList.BackColor = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("FatumStyles Appearance")]
        public Color ListTextColor
        {
            get { return listTextColor; }
            set { listTextColor = value; cmbList.ForeColor = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("FatumStyles Appearance")]
        public Color BorderColor
        {
            get { return borderColor; }
            set { borderColor = value; base.BackColor = value; Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("FatumStyles Appearance")]
        public int BorderSize
        {
            get { return borderSize; }
            set { borderSize = value; Padding = new Padding(value); AdjustComboBoxDimensions(); Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("FatumStyles Appearance")]
        public int BorderRadius
        {
            get { return borderRadius; }
            set { borderRadius = value; Invalidate(); }
        }

        [Category("FatumStyles Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design", typeof(UITypeEditor))]
        public ComboBox.ObjectCollection Items => cmbList.Items;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("FatumStyles Data")]
        [AttributeProvider(typeof(IListSource))]
        public object DataSource
        {
            get { return cmbList.DataSource; }
            set { cmbList.DataSource = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("FatumStyles Data")]
        public AutoCompleteStringCollection AutoCompleteCustomSource
        {
            get { return cmbList.AutoCompleteCustomSource; }
            set { cmbList.AutoCompleteCustomSource = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("FatumStyles Data")]
        public AutoCompleteSource AutoCompleteSource
        {
            get { return cmbList.AutoCompleteSource; }
            set { cmbList.AutoCompleteSource = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("FatumStyles Data")]
        public AutoCompleteMode AutoCompleteMode
        {
            get { return cmbList.AutoCompleteMode; }
            set { cmbList.AutoCompleteMode = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("FatumStyles Data")]
        public object SelectedItem
        {
            get { return cmbList.SelectedItem; }
            set { cmbList.SelectedItem = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("FatumStyles Data")]
        public int SelectedIndex
        {
            get { return cmbList.SelectedIndex; }
            set { cmbList.SelectedIndex = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("FatumStyles Data")]
        public string DisplayMember
        {
            get { return cmbList.DisplayMember; }
            set { cmbList.DisplayMember = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("FatumStyles Data")]
        public string ValueMember
        {
            get { return cmbList.ValueMember; }
            set { cmbList.ValueMember = value; }
        }

        private void AdjustComboBoxDimensions()
        {
            cmbList.Width = lblText.Width;
            cmbList.Location = new Point(Width - Padding.Right - cmbList.Width, lblText.Bottom - cmbList.Height);
            if (cmbList.Height >= Height)
                Height = cmbList.Height + (borderSize * 2);
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectedIndexChanged?.Invoke(sender, e);
            lblText.Text = cmbList.Text;
        }

        private void ComboBox_TextChanged(object sender, EventArgs e)
        {
            lblText.Text = cmbList.Text;
        }

        private void Icon_Click(object sender, EventArgs e)
        {
            cmbList.Select();
            cmbList.DroppedDown = true;
        }

        private void Surface_Click(object sender, EventArgs e)
        {
            OnClick(e);
            cmbList.Select();
            if (cmbList.DropDownStyle == ComboBoxStyle.DropDownList)
                cmbList.DroppedDown = true;
        }

        private void Surface_MouseEnter(object sender, EventArgs e)
        {
            OnMouseEnter(e);
        }

        private void Surface_MouseLeave(object sender, EventArgs e)
        {
            OnMouseLeave(e);
        }

        private void Icon_Paint(object sender, PaintEventArgs e)
        {
            int w = 14, h = 6;
            Rectangle rect = new Rectangle((btnIcon.Width - w) / 2, (btnIcon.Height - h) / 2, w, h);
            using (GraphicsPath p = new GraphicsPath())
            using (Pen pen = new Pen(iconColor, 2))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                p.AddLine(rect.X, rect.Y, rect.X + w / 2, rect.Bottom);
                p.AddLine(rect.X + w / 2, rect.Bottom, rect.Right, rect.Y);
                e.Graphics.DrawPath(pen, p);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rectSurface = ClientRectangle;
            Rectangle rectBorder = Rectangle.Inflate(rectSurface, -borderSize, -borderSize);

            using (GraphicsPath pathSurface = GetFigurePath(rectSurface, borderRadius))
            using (GraphicsPath pathBorder = GetFigurePath(rectBorder, Math.Max(0, borderRadius - borderSize)))
            using (Pen penSurface = new Pen(Parent?.BackColor ?? BackColor, borderSize))
            using (Pen penBorder = new Pen(borderColor, borderSize))
            {
                Region = new Region(pathSurface);
                e.Graphics.DrawPath(penSurface, pathSurface);
                if (borderSize > 0)
                    e.Graphics.DrawPath(penBorder, pathBorder);
            }
        }

        private GraphicsPath GetFigurePath(Rectangle rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            float curveSize = radius * 2f;
            if (curveSize <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            path.StartFigure();
            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
            path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
            path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (borderRadius > Height)
                borderRadius = Height;
            AdjustComboBoxDimensions();
            Invalidate();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (Parent != null)
                Parent.BackColorChanged += Container_BackColorChanged;
        }

        private void Container_BackColorChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void FatumComboBox_Load(object sender, EventArgs e)
        {
            AdjustComboBoxDimensions();
        }
    }
}
