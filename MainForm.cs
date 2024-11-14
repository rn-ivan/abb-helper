using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers;
using System.Reflection;
using System.Threading.Channels;
using System.Collections;

namespace AbbHelper
{
    public class Controller : IReadOnlyCollection<ABB.Robotics.Controllers.Controller>, IDisposable
    {
        private List<ABB.Robotics.Controllers.Controller> controllers = [];
        private Action? done;

        public Controller(ABB.Robotics.Controllers.Controller[] controllers, Action? done = null)
        {
            this.controllers.AddRange(controllers); 
            this.done = done;
        }

        public int Count => controllers.Count;
        public IEnumerator<ABB.Robotics.Controllers.Controller> GetEnumerator() => (IEnumerator<ABB.Robotics.Controllers.Controller>)controllers.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose()
        {
            done?.Invoke();
        }
    }

    public partial class MainForm : Form
    {
        public struct ControllerEvent
        {
            public string action;
            public ControllerInfo? info;
        }

        public event EventHandler<string>? SelectedControllerChanged;
        public event EventHandler<string[]>? CheckedControllersChanged;

        private NetworkWatcher? networkwatcher = null;
        private Dictionary<string, ABB.Robotics.Controllers.Controller> controllers = [];
        private Dictionary<string, Control> helpers = [];
        private SemaphoreSlim semaphore = new(1);
        private Channel<ControllerEvent> channel = Channel.CreateUnbounded<ControllerEvent>();
        private CancellationTokenSource source = new();

        public MainForm()
        {
            InitializeComponent();
        }

        public string SelectedController => ControllerList.Text;
        public string[] CheckedControllers => ControllerList.CheckedItems.Cast<string>().ToArray();

        public Controller GetControllers(params string[] names)
        {
            ControllerList.Enabled = false;
            HelperList.Enabled = false;
            semaphore.Wait();
            return new Controller(
                controllers.Where(x => names.Contains(x.Key)).Select(x => x.Value).ToArray(),
                () => { 
                    Invoke(() =>
                    {
                        ControllerList.Enabled = true;
                        HelperList.Enabled = true;
                        semaphore.Release();
                    });
                }
            );
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
                    var writer = channel.Writer;
                    foreach (ControllerInfo info in found)
                        writer.TryWrite(new ControllerEvent { action = "add", info = info });
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
            ABB.Robotics.Controllers.Controller? controller = null;
            var dispose = false;
            try
            {
                controller = ABB.Robotics.Controllers.Controller.Connect(info, ConnectionType.Standalone);
                controller.Logon(UserInfo.DefaultUser);
            }
            catch
            {
                dispose = true;
            }
            if (controller == null) return;
            if (!dispose)
            {
                var name = $"{controller.Name} {controller.SystemName} {controller.IPAddress} RW{controller.RobotWareVersion} ";
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
            if (controllers.TryGetValue(name, out ABB.Robotics.Controllers.Controller? controller))
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
            SelectedControllerChanged?.Invoke(this, ControllerList.Text);
        }

        private void ControllerList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var name = ControllerList.Items[e.Index].ToString();
            var chk = ControllerList.CheckedItems;
            CheckedControllersChanged?.Invoke(this, ControllerList.Items.Cast<string>().Where(x => x == name ? e.NewValue==CheckState.Checked : chk.Contains(x)).ToArray());
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            source.Cancel();
        }
    }
}
