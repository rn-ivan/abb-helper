namespace AbbHelper
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            HSplit = new SplitContainer();
            ControllerList = new CheckedListBox();
            VSplit = new SplitContainer();
            HelperList = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)HSplit).BeginInit();
            HSplit.Panel1.SuspendLayout();
            HSplit.Panel2.SuspendLayout();
            HSplit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)VSplit).BeginInit();
            VSplit.Panel1.SuspendLayout();
            VSplit.SuspendLayout();
            SuspendLayout();
            // 
            // HSplit
            // 
            HSplit.BorderStyle = BorderStyle.Fixed3D;
            HSplit.Dock = DockStyle.Fill;
            HSplit.Location = new Point(0, 0);
            HSplit.Name = "HSplit";
            // 
            // HSplit.Panel1
            // 
            HSplit.Panel1.Controls.Add(ControllerList);
            // 
            // HSplit.Panel2
            // 
            HSplit.Panel2.Controls.Add(VSplit);
            HSplit.Size = new Size(800, 450);
            HSplit.SplitterDistance = 179;
            HSplit.TabIndex = 1;
            // 
            // ControllerList
            // 
            ControllerList.BorderStyle = BorderStyle.None;
            ControllerList.Dock = DockStyle.Fill;
            ControllerList.Enabled = false;
            ControllerList.FormattingEnabled = true;
            ControllerList.Location = new Point(0, 0);
            ControllerList.Name = "ControllerList";
            ControllerList.Size = new Size(175, 446);
            ControllerList.Sorted = true;
            ControllerList.TabIndex = 1;
            ControllerList.ItemCheck += ControllerList_ItemCheck;
            ControllerList.SelectedIndexChanged += ControllerList_SelectedIndexChanged;
            // 
            // VSplit
            // 
            VSplit.Dock = DockStyle.Fill;
            VSplit.FixedPanel = FixedPanel.Panel1;
            VSplit.IsSplitterFixed = true;
            VSplit.Location = new Point(0, 0);
            VSplit.Name = "VSplit";
            VSplit.Orientation = Orientation.Horizontal;
            // 
            // VSplit.Panel1
            // 
            VSplit.Panel1.Controls.Add(HelperList);
            VSplit.Panel1MinSize = 24;
            VSplit.Size = new Size(613, 446);
            VSplit.SplitterDistance = 25;
            VSplit.SplitterWidth = 2;
            VSplit.TabIndex = 1;
            // 
            // HelperList
            // 
            HelperList.Dock = DockStyle.Fill;
            HelperList.DropDownStyle = ComboBoxStyle.DropDownList;
            HelperList.FormattingEnabled = true;
            HelperList.Location = new Point(0, 0);
            HelperList.Name = "HelperList";
            HelperList.Size = new Size(613, 23);
            HelperList.TabIndex = 0;
            HelperList.SelectedIndexChanged += HelperList_SelectedIndexChanged;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(HSplit);
            Name = "MainForm";
            Text = "ABB Helper";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            HSplit.Panel1.ResumeLayout(false);
            HSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)HSplit).EndInit();
            HSplit.ResumeLayout(false);
            VSplit.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)VSplit).EndInit();
            VSplit.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer HSplit;
        private CheckedListBox ControllerList;
        private SplitContainer VSplit;
        private ComboBox HelperList;
    }
}
