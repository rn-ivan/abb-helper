namespace AbbHelper.Helpers
{
    partial class SystemHelper
    {
        /// <summary> 
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SystemHelper));
            log = new RichTextBox();
            toolbar = new ToolStrip();
            backup = new ToolStripButton();
            restore = new ToolStripButton();
            toolbar.SuspendLayout();
            SuspendLayout();
            // 
            // log
            // 
            log.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            log.Location = new Point(3, 36);
            log.Name = "log";
            log.ReadOnly = true;
            log.Size = new Size(768, 703);
            log.TabIndex = 0;
            log.Text = "";
            // 
            // toolbar
            // 
            toolbar.GripMargin = new Padding(0);
            toolbar.GripStyle = ToolStripGripStyle.Hidden;
            toolbar.Items.AddRange(new ToolStripItem[] { backup, restore });
            toolbar.LayoutStyle = ToolStripLayoutStyle.Flow;
            toolbar.Location = new Point(0, 0);
            toolbar.Name = "toolbar";
            toolbar.Padding = new Padding(3, 0, 3, 0);
            toolbar.RenderMode = ToolStripRenderMode.System;
            toolbar.Size = new Size(774, 33);
            toolbar.TabIndex = 1;
            // 
            // backup
            // 
            backup.BackColor = SystemColors.ControlDark;
            backup.DisplayStyle = ToolStripItemDisplayStyle.Text;
            backup.Enabled = false;
            backup.Image = (Image)resources.GetObject("backup.Image");
            backup.ImageTransparentColor = Color.Magenta;
            backup.Margin = new Padding(2);
            backup.Name = "backup";
            backup.Padding = new Padding(5);
            backup.Size = new Size(60, 29);
            backup.Text = "Backup";
            // 
            // restore
            // 
            restore.BackColor = SystemColors.ControlDark;
            restore.DisplayStyle = ToolStripItemDisplayStyle.Text;
            restore.Enabled = false;
            restore.Image = (Image)resources.GetObject("restore.Image");
            restore.ImageTransparentColor = Color.Magenta;
            restore.Margin = new Padding(2);
            restore.Name = "restore";
            restore.Padding = new Padding(5);
            restore.Size = new Size(60, 29);
            restore.Text = "Restore";
            // 
            // SystemHelper
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(toolbar);
            Controls.Add(log);
            Name = "SystemHelper";
            Size = new Size(774, 742);
            Load += SystemHelper_Load;
            toolbar.ResumeLayout(false);
            toolbar.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox log;
        private ToolStrip toolbar;
        private ToolStripButton backup;
        private ToolStripButton restore;
    }
}
