using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers;
using System.Reflection;

namespace AbbHelper
{
    public partial class MainForm : Form
    {
        private NetworkWatcher? networkwatcher = null;
        private Dictionary<string, ControllerInfo> controllers = [];
        private Dictionary<string, Control> helpers = [];

        public MainForm()
        {
            InitializeComponent();
        }

        public ControllerInfo? SelectedController => ControllerList.Text == "" ? null : controllers[ControllerList.Text];
        public ControllerInfo[] CheckedControllers => ControllerList.CheckedItems.Cast<object>().Select(item => controllers[item.ToString()!]).ToArray();

        public event EventHandler<ControllerInfo?>? SelectedControllerChanged;
        public event EventHandler<ControllerInfo[]>? CheckedControllersChanged;

        private void MainForm_Load(object sender, EventArgs e)
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            HelperList.Items.Clear();
            foreach (var type in types)
                if (type.Name.ToLower().EndsWith("helper"))
                    HelperList.Items.Add(type.Name.Substring(0, type.Name.Length - "Helper".Length));

            Task.Run(() =>
            {
                var scanner = new NetworkScanner();
                scanner.Scan();
                var found = scanner.Controllers;
                Invoke(() =>
                {
                    foreach (ControllerInfo info in found)
                        AddController(this, info);
                    networkwatcher = new(found);
                    networkwatcher.Found += new EventHandler<NetworkWatcherEventArgs>(OnFound);
                    networkwatcher.Lost += new EventHandler<NetworkWatcherEventArgs>(OnLost);
                    networkwatcher.EnableRaisingEvents = true;
                    ControllerList.Enabled = true;
                    HelperList.Enabled = true;
                });
            });
        }

        private void OnFound(object? sender, NetworkWatcherEventArgs e)
        {
            Invoke(new EventHandler<ControllerInfo>(AddController), [this, e.Controller]);
        }

        private void OnLost(object? sender, NetworkWatcherEventArgs e)
        {
            Invoke(new EventHandler<ControllerInfo>(RemoveController), [this, e.Controller]);
        }

        private void AddController(object? sender, ControllerInfo info)
        {
            var name = $"{info.Name} ({info.IPAddress})";
            controllers[name] = info;
            ControllerList.Items.Add(name);
        }

        private void RemoveController(object? sender, ControllerInfo info)
        {
            var name = $"{info.Name} ({info.IPAddress})";
            var index = ControllerList.FindStringExact(name);
            if (index >= 0) ControllerList.Items.RemoveAt(index);
            controllers.Remove(name);
        }

        private void HelperList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var name = HelperList.Text;
            foreach (var kvp in helpers) kvp.Value.Visible = kvp.Key == name;
            if (helpers.ContainsKey(name)) return;

            try
            {
                var types = Assembly.GetExecutingAssembly().GetTypes();
                var helper = Activator.CreateInstance(types.First(t => t.Name == name + "Helper")) as Control;
                VSplit.Panel2.Controls.Add(helper);
                helper!.Dock = DockStyle.Fill;
                helpers[name] = helper;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ControllerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedControllerChanged?.Invoke(this, SelectedController);
        }

        private void ControllerList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            List<ControllerInfo> checkedControllers = [];
            for (int i = 0; i < ControllerList.Items.Count; i++)
                if ((e.Index == i && e.NewValue == CheckState.Checked) || (e.Index != i && ControllerList.GetItemChecked(i)))
                    checkedControllers.Add(controllers[ControllerList.Items[i].ToString()!]);

            CheckedControllersChanged?.Invoke(this, checkedControllers.ToArray());
        }
    }
}
