using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers;
using System.Reflection;
using System.Threading.Channels;

namespace AbbHelper
{
    public partial class MainForm : Form
    {
        public struct ControllerEvent
        {
            public string action;
            public string? name;
            public ControllerInfo? info;
        }

        public event EventHandler? SelectedControllerChanged;
        public event EventHandler? CheckedControllersChanged;

        private NetworkWatcher? networkwatcher = null;
        private Dictionary<string, Controller> controllers = [];
        private Dictionary<string, Control> helpers = [];
        private SemaphoreSlim semaphore = new(1);
        private Channel<ControllerEvent> channel = Channel.CreateUnbounded<ControllerEvent>();
        private CancellationTokenSource source = new();

        public MainForm()
        {
            InitializeComponent();
        }

        public Controller? SelectedController
        {
            get
            {
                semaphore.Wait();
                var value = ControllerList.Text == "" ? null : controllers[ControllerList.Text];
                semaphore.Release();
                return value;
            }
        }
        public Controller[] CheckedControllers
        {
            get
            {
                semaphore.Wait();
                var value = ControllerList.CheckedItems.Cast<object>().Select(item => controllers[item.ToString()!]).ToArray();
                semaphore.Release();
                return value;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            HelperList.Items.Clear();
            foreach (var type in types)
                if (type.Name.ToLower().EndsWith("helper"))
                    HelperList.Items.Add(type.Name.Substring(0, type.Name.Length - "Helper".Length));

            var token = source.Token;
            Task.Run(() =>
            {
                var scanner = new NetworkScanner();
                scanner.Scan();
                var found = scanner.Controllers;
                Invoke(() =>
                {
                    foreach (ControllerInfo info in found)
                        channel.Writer.TryWrite(new ControllerEvent { action = "add", info = info });
                    networkwatcher = new(found);
                    networkwatcher.Found += new EventHandler<NetworkWatcherEventArgs>(OnFound);
                    networkwatcher.Lost += new EventHandler<NetworkWatcherEventArgs>(OnLost);
                    networkwatcher.EnableRaisingEvents = true;
                    ControllerList.Enabled = true;
                    HelperList.Enabled = true;
                });
                var reader = channel.Reader;
                while (!token.IsCancellationRequested)
                {
                    if (reader.TryRead(out var item))
                    {
                        switch (item.action)
                        {
                            case "add":
                                addController(item.info);
                                break;
                            case "remove":
                                removeController(item.info);
                                break;
                        }
                    }
                    else Thread.Sleep(100);
                }
                semaphore.Wait();
                foreach (var controller in controllers.Values)
                {
                    var name = $"{controller.Name} ({controller.IPAddress})";
                    Invoke(() => ControllerList.Items.Remove(name));
                    try { controller?.Dispose(); } catch { }
                }
                controllers.Clear();
                semaphore.Release();
            }, token);
        }

        private void addController(ControllerInfo? info)
        {
            if (info == null) return;
            Controller? controller = null;
            var dispose = false;
            try
            {
                controller = Controller.Connect(info, ConnectionType.Standalone);
                controller.Logon(UserInfo.DefaultUser);
            }
            catch
            {
                dispose = true;
            }
            if (controller == null) return;
            if (!dispose)
            {
                var name = $"{controller.Name} ({controller.IPAddress})";
                semaphore.Wait();
                if (controllers.ContainsKey(name))
                    dispose = true;
                else
                {
                    controllers[name] = controller;
                    Invoke(() => ControllerList.Items.Add(name));
                }
                semaphore.Release();
            }
            if (dispose)
                try { controller?.Dispose(); } catch { }
        }

        private void removeController(ControllerInfo? info)
        {
            if (info == null) return;
            var name = $"{info.Name} ({info.IPAddress})";
            semaphore.Wait();
            if (controllers.TryGetValue(name, out Controller? controller))
            {
                controllers.Remove(name);
                Invoke(() => ControllerList.Items.Remove(name));
            }
            semaphore.Release();
            try { controller?.Dispose(); } catch { }
        }

        private void OnFound(object? sender, NetworkWatcherEventArgs e)
        {
            channel.Writer.TryWrite(new ControllerEvent { action = "add", info = e.Controller });
        }

        private void OnLost(object? sender, NetworkWatcherEventArgs e)
        {
            channel.Writer.TryWrite(new ControllerEvent { action = "remove", info = e.Controller });
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
            SelectedControllerChanged?.Invoke(this, e);
        }

        private void ControllerList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            CheckedControllersChanged?.Invoke(this, e);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            source.Cancel();
        }
    }
}
