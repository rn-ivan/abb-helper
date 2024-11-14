using ABB.Robotics.Controllers;
using System.Xml.Linq;

namespace AbbHelper.Helpers
{
    public partial class SystemHelper : UserControl
    {
        public SystemHelper()
        {
            InitializeComponent();
        }

        private void SystemHelper_Load(object sender, EventArgs e)
        {
            if (ParentForm is not MainForm form) return;
            form.CheckedControllersChanged += OnCheckedControllersChanged;
        }

        private void OnCheckedControllersChanged(object? sender, string[] names)
        {
            if (ParentForm is not MainForm) return;
            backup.Enabled = names.Length > 0;
            restore.Enabled = names.Length == 1;
        }
    }
}
