using System.Collections.Generic;
using System;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;

using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ToolsAveryDady
{
    //.::Tweety Theme::.
    //Author:   UnReLaTeDScript
    //Credits:  Aeonhack [Themebase], Aeonhack (Menustrip Code)
    //Version:  1.0
    abstract public class Theme : ContainerControl
    {

        #region  Initialization

        protected Graphics G;
        public Theme()
        {
            SetStyle((ControlStyles)(139270), true);
        }

        private bool ParentIsForm;
        protected override void OnHandleCreated(EventArgs e)
        {
            Dock = DockStyle.Fill;
            ParentIsForm = Parent is Form;
            if (ParentIsForm)
            {
                if (!(_TransparencyKey == Color.Empty))
                {
                    ParentForm.TransparencyKey = _TransparencyKey;
                }
                ParentForm.FormBorderStyle = FormBorderStyle.None;
            }
            base.OnHandleCreated(e);
        }

        override public string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                Invalidate();
            }
        }
        #endregion

        #region  Sizing and Movement

        private bool _Resizable = true;
        public bool Resizable
        {
            get
            {
                return _Resizable;
            }
            set
            {
                _Resizable = value;
            }
        }

        private int _MoveHeight = 24;
        public int MoveHeight
        {
            get
            {
                return _MoveHeight;
            }
            set
            {
                _MoveHeight = value;
                Header = new Rectangle(7, 7, Width - 14, _MoveHeight - 7);
            }
        }

        private IntPtr Flag;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!(e.Button == MouseButtons.Left))
            {
                return;
            }
            if (ParentIsForm)
            {
                if (ParentForm.WindowState == FormWindowState.Maximized)
                {
                    return;
                }
            }

            if (Header.Contains(e.Location))
            {
                Flag = new IntPtr(2);
            }
            else if (Current.Position == 0 || !_Resizable)
            {
                return;
            }
            else
            {
                Flag = new IntPtr(Current.Position);
            }

            Capture = false;
            Message temp_m = Message.Create(Parent.Handle, 161, Flag, IntPtr.Zero);
            DefWndProc(ref temp_m);

            base.OnMouseDown(e);
        }

        public struct Pointer
        {
            readonly public Cursor Cursor;
            readonly public byte Position;
            public Pointer(Cursor c, byte p)
            {
                Cursor = c;
                Position = p;
            }
        }

        private bool F1;
        private bool F2;
        private bool F3;
        private bool F4;
        private Point PTC;
        private Pointer GetPointer()
        {
            PTC = PointToClient(MousePosition);
            F1 = PTC.X < 7;
            F2 = PTC.X > Width - 7;
            F3 = PTC.Y < 7;
            F4 = PTC.Y > Height - 7;

            if (F1 && F3)
            {
                return new Pointer(Cursors.SizeNWSE, 13);
            }
            if (F1 && F4)
            {
                return new Pointer(Cursors.SizeNESW, 16);
            }
            if (F2 && F3)
            {
                return new Pointer(Cursors.SizeNESW, 14);
            }
            if (F2 && F4)
            {
                return new Pointer(Cursors.SizeNWSE, 17);
            }
            if (F1)
            {
                return new Pointer(Cursors.SizeWE, 10);
            }
            if (F2)
            {
                return new Pointer(Cursors.SizeWE, 11);
            }
            if (F3)
            {
                return new Pointer(Cursors.SizeNS, 12);
            }
            if (F4)
            {
                return new Pointer(Cursors.SizeNS, 15);
            }
            return new Pointer(Cursors.Default, 0);
        }

        private Pointer Current;
        private Pointer Pending;
        private void SetCurrent()
        {
            Pending = GetPointer();
            if (Current.Position == Pending.Position)
            {
                return;
            }
            Current = GetPointer();
            Cursor = Current.Cursor;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_Resizable)
            {
                SetCurrent();
            }
            base.OnMouseMove(e);
        }

        protected Rectangle Header;
        protected override void OnSizeChanged(EventArgs e)
        {
            if (Width == 0 || Height == 0)
            {
                return;
            }
            Header = new Rectangle(7, 7, Width - 14, _MoveHeight - 7);
            Invalidate();
            base.OnSizeChanged(e);
        }

        #endregion

        #region  Convienence

        abstract public void PaintHook();
        protected sealed override void OnPaint(PaintEventArgs e)
        {
            if (Width == 0 || Height == 0)
            {
                return;
            }
            G = e.Graphics;
            PaintHook();
        }

        private Color _TransparencyKey;
        public Color TransparencyKey
        {
            get
            {
                return _TransparencyKey;
            }
            set
            {
                _TransparencyKey = value;
                Invalidate();
            }
        }

        private Image _Image;
        public Image Image
        {
            get
            {
                return _Image;
            }
            set
            {
                _Image = value;
                Invalidate();
            }
        }
        public int ImageWidth
        {
            get
            {
                if (ReferenceEquals(_Image, null))
                {
                    return 0;
                }
                return _Image.Width;
            }
        }

        private Size _Size;
        private Rectangle _Rectangle;
        private LinearGradientBrush _Gradient;
        private SolidBrush _Brush;

        protected void DrawCorners(Color c, Rectangle rect)
        {
            _Brush = new SolidBrush(c);
            G.FillRectangle(_Brush, rect.X, rect.Y, 1, 1);
            G.FillRectangle(_Brush, rect.X + (rect.Width - 1), rect.Y, 1, 1);
            G.FillRectangle(_Brush, rect.X, rect.Y + (rect.Height - 1), 1, 1);
            G.FillRectangle(_Brush, rect.X + (rect.Width - 1), rect.Y + (rect.Height - 1), 1, 1);
        }

        protected void DrawBorders(Pen p1, Pen p2, Rectangle rect)
        {
            G.DrawRectangle(p1, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            G.DrawRectangle(p2, rect.X + 1, rect.Y + 1, rect.Width - 3, rect.Height - 3);
        }

        protected void DrawText(HorizontalAlignment a, Color c, int x)
        {
            DrawText(a, c, x, 0);
        }
        protected void DrawText(HorizontalAlignment a, Color c, int x, int y)
        {
            if (string.IsNullOrEmpty(Text))
            {
                return;
            }
            _Size = G.MeasureString(Text, Font).ToSize();
            _Brush = new SolidBrush(c);

            switch (a)
            {
                case HorizontalAlignment.Left:
                    G.DrawString(Text, Font, _Brush, x, _MoveHeight / 2 - _Size.Height / 2 + y);
                    break;
                case HorizontalAlignment.Right:
                    G.DrawString(Text, Font, _Brush, Width - _Size.Width - x, _MoveHeight / 2 - _Size.Height / 2 + y);
                    break;
                case HorizontalAlignment.Center:
                    G.DrawString(Text, Font, _Brush, Width / 2 - _Size.Width / 2 + x, _MoveHeight / 2 - _Size.Height / 2 + y);
                    break;
            }
        }

        protected void DrawIcon(HorizontalAlignment a, int x)
        {
            DrawIcon(a, x, 0);
        }
        protected void DrawIcon(HorizontalAlignment a, int x, int y)
        {
            if (ReferenceEquals(_Image, null))
            {
                return;
            }
            switch (a)
            {
                case HorizontalAlignment.Left:
                    G.DrawImage(_Image, x, _MoveHeight / 2 - _Image.Height / 2 + y);
                    break;
                case HorizontalAlignment.Right:
                    G.DrawImage(_Image, Width - _Image.Width - x, _MoveHeight / 2 - _Image.Height / 2 + y);
                    break;
                case HorizontalAlignment.Center:
                    G.DrawImage(_Image, Width / 2 - _Image.Width / 2, _MoveHeight / 2 - _Image.Height / 2);
                    break;
            }
        }

        protected void DrawGradient(Color c1, Color c2, int x, int y, int width, int height, float angle)
        {
            _Rectangle = new Rectangle(x, y, width, height);
            _Gradient = new LinearGradientBrush(_Rectangle, c1, c2, angle);
            G.FillRectangle(_Gradient, _Rectangle);
        }

        #endregion

    }
    abstract public class ThemeControl : Control
    {

        #region  Initialization

        protected Graphics G;
        protected Bitmap B;
        public ThemeControl()
        {
            SetStyle((ControlStyles)(139270), true);
            B = new Bitmap(1, 1);
            G = Graphics.FromImage(B);
        }

        public void AllowTransparent()
        {
            SetStyle(ControlStyles.Opaque, false);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        override public string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                Invalidate();
            }
        }
        #endregion

        #region  Mouse Handling

        protected enum State : byte
        {
            MouseNone = 0,
            MouseOver = 1,
            MouseDown = 2
        }

        protected State MouseState;
        protected override void OnMouseLeave(EventArgs e)
        {
            ChangeMouseState(State.MouseNone);
            base.OnMouseLeave(e);
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            ChangeMouseState(State.MouseOver);
            base.OnMouseEnter(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            ChangeMouseState(State.MouseOver);
            base.OnMouseUp(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ChangeMouseState(State.MouseDown);
            }
            base.OnMouseDown(e);
        }

        private void ChangeMouseState(State e)
        {
            MouseState = e;
            Invalidate();
        }

        #endregion

        #region  Convienence

        abstract public void PaintHook();
        protected sealed override void OnPaint(PaintEventArgs e)
        {
            if (Width == 0 || Height == 0)
            {
                return;
            }
            PaintHook();
            e.Graphics.DrawImage(B, 0, 0);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (!(Width == 0) && !(Height == 0))
            {
                B = new Bitmap(Width, Height);
                G = Graphics.FromImage(B);
                Invalidate();
            }
            base.OnSizeChanged(e);
        }

        private bool _NoRounding;
        public bool NoRounding
        {
            get
            {
                return _NoRounding;
            }
            set
            {
                _NoRounding = value;
                Invalidate();
            }
        }

        private Image _Image;
        public Image Image
        {
            get
            {
                return _Image;
            }
            set
            {
                _Image = value;
                Invalidate();
            }
        }
        public int ImageWidth
        {
            get
            {
                if (ReferenceEquals(_Image, null))
                {
                    return 0;
                }
                return _Image.Width;
            }
        }
        public int ImageTop
        {
            get
            {
                if (ReferenceEquals(_Image, null))
                {
                    return 0;
                }
                return Height / 2 - _Image.Height / 2;
            }
        }

        private Size _Size;
        private Rectangle _Rectangle;
        private LinearGradientBrush _Gradient;
        private SolidBrush _Brush;

        protected void DrawCorners(Color c, Rectangle rect)
        {
            if (_NoRounding)
            {
                return;
            }

            B.SetPixel(rect.X, rect.Y, c);
            B.SetPixel(rect.X + (rect.Width - 1), rect.Y, c);
            B.SetPixel(rect.X, rect.Y + (rect.Height - 1), c);
            B.SetPixel(rect.X + (rect.Width - 1), rect.Y + (rect.Height - 1), c);
        }

        protected void DrawBorders(Pen p1, Pen p2, Rectangle rect)
        {
            G.DrawRectangle(p1, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            G.DrawRectangle(p2, rect.X + 1, rect.Y + 1, rect.Width - 3, rect.Height - 3);
        }

        protected void DrawText(HorizontalAlignment a, Color c, int x)
        {
            DrawText(a, c, x, 0);
        }
        protected void DrawText(HorizontalAlignment a, Color c, int x, int y)
        {
            if (string.IsNullOrEmpty(Text))
            {
                return;
            }
            _Size = G.MeasureString(Text, Font).ToSize();
            _Brush = new SolidBrush(c);

            switch (a)
            {
                case HorizontalAlignment.Left:
                    G.DrawString(Text, Font, _Brush, x, Height / 2 - _Size.Height / 2 + y);
                    break;
                case HorizontalAlignment.Right:
                    G.DrawString(Text, Font, _Brush, Width - _Size.Width - x, Height / 2 - _Size.Height / 2 + y);
                    break;
                case HorizontalAlignment.Center:
                    G.DrawString(Text, Font, _Brush, Width / 2 - _Size.Width / 2 + x, Height / 2 - _Size.Height / 2 + y);
                    break;
            }
        }

        protected void DrawIcon(HorizontalAlignment a, int x)
        {
            DrawIcon(a, x, 0);
        }
        protected void DrawIcon(HorizontalAlignment a, int x, int y)
        {
            if (ReferenceEquals(_Image, null))
            {
                return;
            }
            switch (a)
            {
                case HorizontalAlignment.Left:
                    G.DrawImage(_Image, x, Height / 2 - _Image.Height / 2 + y);
                    break;
                case HorizontalAlignment.Right:
                    G.DrawImage(_Image, Width - _Image.Width - x, Height / 2 - _Image.Height / 2 + y);
                    break;
                case HorizontalAlignment.Center:
                    G.DrawImage(_Image, Width / 2 - _Image.Width / 2, Height / 2 - _Image.Height / 2);
                    break;
            }
        }

        protected void DrawGradient(Color c1, Color c2, int x, int y, int width, int height, float angle)
        {
            _Rectangle = new Rectangle(x, y, width, height);
            _Gradient = new LinearGradientBrush(_Rectangle, c1, c2, angle);
            G.FillRectangle(_Gradient, _Rectangle);
        }
        #endregion

    }
    abstract public class ThemeContainerControl : ContainerControl
    {

        #region  Initialization

        protected Graphics G;
        protected Bitmap B;
        public ThemeContainerControl()
        {
            SetStyle((ControlStyles)(139270), true);
            B = new Bitmap(1, 1);
            G = Graphics.FromImage(B);
        }

        public void AllowTransparent()
        {
            SetStyle(ControlStyles.Opaque, false);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        #endregion
        #region  Convienence

        abstract public void PaintHook();
        protected sealed override void OnPaint(PaintEventArgs e)
        {
            if (Width == 0 || Height == 0)
            {
                return;
            }
            PaintHook();
            e.Graphics.DrawImage(B, 0, 0);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (!(Width == 0) && !(Height == 0))
            {
                B = new Bitmap(Width, Height);
                G = Graphics.FromImage(B);
                Invalidate();
            }
            base.OnSizeChanged(e);
        }

        private bool _NoRounding;
        public bool NoRounding
        {
            get
            {
                return _NoRounding;
            }
            set
            {
                _NoRounding = value;
                Invalidate();
            }
        }

        private Rectangle _Rectangle;
        private LinearGradientBrush _Gradient;

        protected void DrawCorners(Color c, Rectangle rect)
        {
            if (_NoRounding)
            {
                return;
            }
            B.SetPixel(rect.X, rect.Y, c);
            B.SetPixel(rect.X + (rect.Width - 1), rect.Y, c);
            B.SetPixel(rect.X, rect.Y + (rect.Height - 1), c);
            B.SetPixel(rect.X + (rect.Width - 1), rect.Y + (rect.Height - 1), c);
        }

        protected void DrawBorders(Pen p1, Pen p2, Rectangle rect)
        {
            G.DrawRectangle(p1, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            G.DrawRectangle(p2, rect.X + 1, rect.Y + 1, rect.Width - 3, rect.Height - 3);
        }

        protected void DrawGradient(Color c1, Color c2, int x, int y, int width, int height, float angle)
        {
            _Rectangle = new Rectangle(x, y, width, height);
            _Gradient = new LinearGradientBrush(_Rectangle, c1, c2, angle);
            G.FillRectangle(_Gradient, _Rectangle);
        }
        #endregion

    }



    public class ReconForm : Theme
    {
        private bool _ShowIcon;
        private TextAlign TA;
        public enum TextAlign : int
        {
            Left = 0,
            Center = 1,
            Right = 2
        }
        public TextAlign TextAlignment
        {
            get
            {
                return TA;
            }
            set
            {
                TA = value;
                Invalidate();
            }
        }
        public bool ShowIcon
        {
            get
            {
                return _ShowIcon;
            }
            set
            {
                _ShowIcon = value;
                Invalidate();
            }
        }
        public ReconForm()
        {
            Color.FromArgb(45, 45, 45);
            MoveHeight = 30;
            this.ForeColor = Color.Olive;
            TransparencyKey = Color.Fuchsia;
            this.BackColor = Color.FromArgb(41, 41, 41);
        }
        override public void PaintHook()
        {
            G.Clear(Color.FromArgb(41, 41, 41));
            DrawGradient(Color.FromArgb(11, 11, 11), Color.FromArgb(26, 26, 26), 1, 1, ClientRectangle.Width, ClientRectangle.Height, 270);
            DrawBorders(Pens.Black, new Pen(Color.FromArgb(52, 52, 52)), ClientRectangle);
            DrawGradient(Color.FromArgb(42, 42, 42), Color.FromArgb(40, 40, 40), 5, 30, Width - 10, Height - 35, 90);
            G.DrawRectangle(new Pen(Color.FromArgb(18, 18, 18)), 5, 30, Width - 10, Height - 35);

            //Icon
            if (_ShowIcon == false)
            {

                switch (TA)
                {
                    case (TextAlign)0:
                        DrawText(HorizontalAlignment.Left, this.ForeColor, 6);
                        break;
                    case (TextAlign)1:
                        DrawText(HorizontalAlignment.Center, this.ForeColor, 0);
                        break;
                    case (TextAlign)2:
                        MessageBox.Show("Invalid Alignment, will not show text.");
                        break;
                }
            }
            else
            {

                switch (TA)
                {
                    case (TextAlign)0:
                        DrawText(HorizontalAlignment.Left, this.ForeColor, 35);
                        break;
                    case (TextAlign)1:
                        DrawText(HorizontalAlignment.Center, this.ForeColor, 0);
                        break;
                    case (TextAlign)2:
                        MessageBox.Show("Invalid Alignment, will not show text.");
                        break;
                }
                G.DrawIcon(this.ParentForm.Icon, new Rectangle(new Point(6, 2), new Size(29, 29)));
            }

            DrawCorners(Color.Fuchsia, ClientRectangle);
        }
    }
    public class ReconButton : ThemeControl
    {
        public ReconButton()
        {
            this.ForeColor = Color.DarkOliveGreen;
        }
        override public void PaintHook()
        {
            switch (MouseState)
            {
                case State.MouseNone:
                    G.Clear(Color.FromArgb(49, 49, 49));
                    DrawGradient(Color.FromArgb(22, 22, 22), Color.FromArgb(34, 34, 34), 1, 1, ClientRectangle.Width, ClientRectangle.Height, 270);
                    DrawBorders(Pens.Black, new Pen(Color.FromArgb(52, 52, 52)), ClientRectangle);
                    DrawText(HorizontalAlignment.Center, this.ForeColor, 0);
                    DrawCorners(this.BackColor, ClientRectangle);
                    break;
                case State.MouseDown:
                    G.Clear(Color.FromArgb(49, 49, 49));
                    DrawGradient(Color.FromArgb(28, 28, 28), Color.FromArgb(38, 38, 38), 1, 1, ClientRectangle.Width, ClientRectangle.Height, 270);
                    DrawGradient(Color.FromArgb(100, 0, 0, 0), Color.Transparent, 1, 1, ClientRectangle.Width, System.Convert.ToInt32((double)ClientRectangle.Height / 2), 90);
                    DrawBorders(Pens.Black, new Pen(Color.FromArgb(52, 52, 52)), ClientRectangle);
                    DrawText(HorizontalAlignment.Center, this.ForeColor, 1);
                    DrawCorners(this.BackColor, ClientRectangle);
                    break;
                case State.MouseOver:
                    G.Clear(Color.FromArgb(49, 49, 49));
                    DrawGradient(Color.FromArgb(28, 28, 28), Color.FromArgb(38, 38, 38), 1, 1, ClientRectangle.Width, ClientRectangle.Height, 270);
                    DrawGradient(Color.FromArgb(100, 50, 50, 50), Color.Transparent, 1, 1, ClientRectangle.Width, System.Convert.ToInt32((double)ClientRectangle.Height / 2), 90);
                    DrawBorders(Pens.Black, new Pen(Color.FromArgb(52, 52, 52)), ClientRectangle);
                    DrawText(HorizontalAlignment.Center, this.ForeColor, -1);
                    DrawCorners(this.BackColor, ClientRectangle);
                    break;
            }
            this.Cursor = Cursors.Hand;

        }
    }

    public class TxtBox : TextBox
    {

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 15:
                    Invalidate();
                    base.WndProc(ref m);
                    this.CustomPaint();
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        public TxtBox()
        {
            Font = new Font("Microsoft Sans Serif", 8);
            ForeColor = Color.Black;
            BackColor = Color.FromArgb(28, 28, 28);
            BorderStyle = BorderStyle.FixedSingle;
        }

        private void CustomPaint()
        {
            Pen p = new Pen(Color.Black);
            Pen o = new Pen(Color.FromArgb(45, 45, 45));
            CreateGraphics().DrawLine(p, 0, 0, Width, 0);
            CreateGraphics().DrawLine(p, 0, Height - 1, Width, Height - 1);
            CreateGraphics().DrawLine(p, 0, 0, 0, Height - 1);
            CreateGraphics().DrawLine(p, Width - 1, 0, Width - 1, Height - 1);

            CreateGraphics().DrawLine(o, 1, 1, Width - 2, 1);
            CreateGraphics().DrawLine(o, 1, Height - 2, Width - 2, Height - 2);
            CreateGraphics().DrawLine(o, 1, 1, 1, Height - 2);
            CreateGraphics().DrawLine(o, Width - 2, 1, Width - 2, Height - 2);
        }
    }

    public class ReconBar : ThemeControl
    {
        private int _Maximum;
        public int Maximum
        {
            get
            {
                return _Maximum;
            }
            set
            {
                if (value < _Value)
                {
                    _Value = value;
                }
                _Maximum = value;
                Invalidate();
            }
        }
        private int _Value;
        public int Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (value > _Maximum)
                {
                    value = _Maximum;
                }
                _Value = value;
                Invalidate();
            }
        }
        override public void PaintHook()
        {

            G.Clear(Color.FromArgb(49, 49, 49));
            DrawGradient(Color.FromArgb(18, 18, 18), Color.FromArgb(28, 28, 28), 1, 1, ClientRectangle.Width, ClientRectangle.Height, 90);

            //Fill
            if (_Value > 6)
            {
                int x = System.Convert.ToInt32((double)_Value / _Maximum * Width);
                int z = ClientRectangle.Height - 7;

                DrawGradient(Color.FromArgb(28, 28, 28), Color.FromArgb(38, 38, 38), 1, 1, System.Convert.ToInt32((double)_Value / _Maximum * Width), ClientRectangle.Height, 270);
                DrawGradient(Color.FromArgb(100, 50, 50, 50), Color.Transparent, 1, 1, System.Convert.ToInt32((double)_Value / _Maximum * Width), System.Convert.ToInt32((double)ClientRectangle.Height / 2), 90);

                DrawGradient(Color.FromArgb(5, this.ForeColor), Color.Transparent, 1, 1, System.Convert.ToInt32((double)_Value / _Maximum * Width), System.Convert.ToInt32((double)ClientRectangle.Height / 4), 90);
                DrawGradient(Color.FromArgb(9, this.ForeColor), Color.Transparent, 1, System.Convert.ToInt32((double)(System.Convert.ToInt32((double)_Value / _Maximum * Width)) / 2), ClientRectangle.Width, System.Convert.ToInt32((double)ClientRectangle.Height / 2), 270);


                G.DrawRectangle(new Pen(Color.FromArgb(50, 50, 50)), 1, 1, x, z + 4);

                G.DrawRectangle(Pens.Black, 2, 2, x, z + 2);
            }
            else if (_Value > 1)
            {
                DrawGradient(Color.FromArgb(109, 183, 255), Color.FromArgb(40, 154, 255), 3, 3, System.Convert.ToInt32((double)_Value / _Maximum * Width), System.Convert.ToInt32((double)Height / 2 - 3), 90);
                DrawGradient(Color.FromArgb(30, 130, 245), Color.FromArgb(15, 100, 170), 3, System.Convert.ToInt32((double)Height / 2 - 1), System.Convert.ToInt32((double)_Value / _Maximum * Width), System.Convert.ToInt32((double)Height / 2 - 2), 90);
            }

            //Borders
            DrawBorders(Pens.Black, new Pen(Color.FromArgb(52, 52, 52)), ClientRectangle);

        }
        public void Increment(int Amount)
        {
            if (this.Value + Amount > Maximum)
            {
                this.Value = Maximum;
            }
            else
            {
                this.Value += Amount;
            }
        }
    }

    public class ReconGroupBox : ThemeContainerControl
    {
        public ReconGroupBox()
        {
            AllowTransparent();
        }
        override public void PaintHook()
        {
            G.Clear(Color.FromArgb(25, 25, 25));
            this.BackColor = Color.FromArgb(25, 25, 25);
            DrawGradient(Color.FromArgb(11, 11, 11), Color.FromArgb(26, 26, 26), 1, 1, ClientRectangle.Width, ClientRectangle.Height, 270);
            DrawBorders(Pens.Black, new Pen(Color.FromArgb(52, 52, 52)), ClientRectangle);

            DrawGradient(Color.FromArgb(150, 32, 32, 32), Color.FromArgb(150, 31, 31, 31), 5, 23, Width - 10, Height - 28, 90);
            G.DrawRectangle(new Pen(Color.FromArgb(130, 13, 13, 13)), 5, 23, Width - 10, Height - 28);

            G.DrawString(Text, Font, new SolidBrush(this.ForeColor), 4, 6);

            DrawCorners(Color.Transparent, ClientRectangle);
        }
    }
    public class ReconCheck : ThemeControl
    {

        #region  Properties
        private bool _CheckedState;
        public bool CheckedState
        {
            get
            {
                return _CheckedState;
            }
            set
            {
                _CheckedState = value;
                Invalidate();
            }
        }
        #endregion

        public ReconCheck()
        {
            Size = new Size(90, 15);
            MinimumSize = new Size(16, 16);
            MaximumSize = new Size(600, 16);
            CheckedState = false;
        }

        public override void PaintHook()
        {
            G.Clear(BackColor);
            DrawGradient(Color.FromArgb(18, 18, 18), Color.FromArgb(28, 28, 28), 0, 0, 15, 15, 90);

            if (CheckedState == true)
            {

                DrawGradient(Color.FromArgb(18, 18, 18), Color.FromArgb(28, 28, 28), 0, 0, 15, 15, 270);
                DrawGradient(Color.FromArgb(100, 40, 40, 40), Color.Transparent, 0, 0, 15, 15, 90);

                DrawGradient(Color.FromArgb(5, this.ForeColor), Color.Transparent, 0, 0, 15, 15, 90);
                DrawGradient(Color.FromArgb(9, this.ForeColor), Color.Transparent, 0, 0, 15, 15, 270);

                G.DrawRectangle(new Pen(Color.FromArgb(10, 10, 10)), 3, 3, 11, 11);
                DrawGradient(Color.FromArgb(50, 50, 50), Color.FromArgb(30, 30, 30), 0, 0, 15, 15, 90);
            }

            DrawBorders(Pens.Black, new Pen(Color.FromArgb(52, 52, 52)), new Rectangle(0, 0, 15, 15));
            DrawText(HorizontalAlignment.Left, this.ForeColor, 17, 0);
        }

        public void changeCheck(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (CheckedState == true)
            {
                CheckedState = false;
            }
            else if (CheckedState == false)
            {
                CheckedState = true;
            }
        }
    }
    public class ReconCheckBox : ThemeControl
    {

        #region  Properties
        private bool _CheckedState;
        public bool CheckedState
        {
            get
            {
                return _CheckedState;
            }
            set
            {
                _CheckedState = value;
                Invalidate();
            }
        }
        #endregion

        public ReconCheckBox()
        {
            Size = new Size(90, 15);
            MinimumSize = new Size(16, 16);
            MaximumSize = new Size(600, 16);
            CheckedState = false;
        }

        public override void PaintHook()
        {
            G.Clear(this.Parent.BackColor);
            DrawGradient(Color.FromArgb(22, 22, 22), Color.FromArgb(32, 32, 32), 0, 0, 15, 15, 90);
            if (CheckedState == true)
            {
                DrawBorders(Pens.Black, new Pen(Color.FromArgb(52, 52, 52)), new Rectangle(0, 0, 15, 15));
                G.DrawString("a", new Font("Marlett", 12), Brushes.Black, new Point(-3, -1));
            }
            else if (CheckedState == false)
            {
                DrawBorders(Pens.Black, new Pen(Color.FromArgb(52, 52, 52)), new Rectangle(0, 0, 15, 15));
            }

            DrawText(HorizontalAlignment.Left, this.ForeColor, 17, 0);
        }

        public void changeCheck(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (CheckedState == true)
            {
                CheckedState = false;
            }
            else if (CheckedState == false)
            {
                CheckedState = true;
            }
        }
    }

    public class ReconSeperator : Control
    {

        private Orientation _Orientation;
        public Orientation Orientation
        {
            get
            {
                return _Orientation;
            }
            set
            {
                _Orientation = value;
                UpdateOffset();
                Invalidate();
            }
        }

        Graphics G;
        Bitmap B;
        int I;
        Color C1;
        Pen P1;
        Pen P2;
        public ReconSeperator()
        {
            SetStyle((System.Windows.Forms.ControlStyles)(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint), true);
            C1 = this.BackColor;
            P1 = new Pen(Color.FromArgb(22, 22, 22));
            P2 = new Pen(Color.FromArgb(49, 49, 49));
            MinimumSize = new Size(5, 2);
            MaximumSize = new Size(10000, 2);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            UpdateOffset();
            base.OnSizeChanged(e);
        }

        public void UpdateOffset()
        {
            I = System.Convert.ToInt32(Convert.ToInt32((int)_Orientation == 0 ? (double)Height / 2 - 1 : (double)Width / 2 - 1));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            B = new Bitmap(Width, Height);
            G = Graphics.FromImage(B);

            G.Clear(C1);

            if ((int)_Orientation == 0)
            {
                G.DrawLine(P1, 0, I, Width, I);
                G.DrawLine(P2, 0, I + 1, Width, I + 1);
            }
            else
            {
                G.DrawLine(P2, I, 0, I, Height);
                G.DrawLine(P1, I + 1, 0, I + 1, Height);
            }

            e.Graphics.DrawImage(B, 0, 0);
            G.Dispose();
            B.Dispose();
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }

    }
}