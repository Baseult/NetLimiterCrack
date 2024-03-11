using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Timer = System.Windows.Forms.Timer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.IO;

namespace NetLimiter_Crack
{
    public partial class Form1 : Form
    {
        private Point lastPoint;

        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                lastPoint = new Point(e.X, e.Y);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint.X;
                this.Top += e.Y - lastPoint.Y;
            }
        }

        private void Panel_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y); // Store the current mouse position
        }

        private void Panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint.X;
                this.Top += e.Y - lastPoint.Y;
            }
        }

        private string FindNetLimiterDll()
        {
            try
            {
                string netLimiterFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Locktime Software", "NetLimiter");
                string dllPath = Path.Combine(netLimiterFolder, "NetLimiter.dll");

                if (File.Exists(dllPath))
                    return dllPath;
                else
                    return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while finding NetLimiter.dll: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private void ReplaceDll(string originalDllPath, ModuleDefMD modifiedModule)
        {
            try
            {
                string originalBackupPath = originalDllPath + ".bak";
                string modifiedDllPath = Path.ChangeExtension(originalDllPath, ".patched.dll");

                // Backup the original DLL
                // File.Replace(originalDllPath, originalBackupPath, null);

                // Save the modified assembly
                modifiedModule.Write(modifiedDllPath);

                // Replace the original DLL with the modified one
                File.Replace(modifiedDllPath, originalDllPath, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while replacing DLL: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button1.Text = "PATCHING...";

            try
            {
                await DisplayMessage("Stopping NLClientApp.exe...", 5);

                // Stop NLClientApp.exe
                await Task.Run(async () =>
                {
                    try
                    {
                        var process = Process.Start(new ProcessStartInfo
                        {
                            FileName = "taskkill",
                            Arguments = "/f /im NLClientApp.exe",
                            UseShellExecute = false,
                            CreateNoWindow = true
                        });

                        if (process != null)
                        {
                            process.WaitForExit();
                            await DisplayMessage("NLClientApp.exe stopped successfully", 10);
                        }
                        else
                        {
                            throw new Exception("Failed to start taskkill process");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error stopping NLClientApp.exe", ex);
                    }
                });

                await DisplayMessage("Stopping nlsvc...", 15);
                // Stop nlsvc
                await Task.Run(async () =>
                {
                    try
                    {
                        var process = Process.Start(new ProcessStartInfo
                        {
                            FileName = "net",
                            Arguments = "stop nlsvc",
                            UseShellExecute = false,
                            CreateNoWindow = true
                        });

                        if (process != null)
                        {
                            process.WaitForExit();
                            await DisplayMessage("Nlsvc stopped successfully", 20);
                        }
                        else
                        {
                            throw new Exception("Failed to start 'net' process to stop nlsvc");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error stopping nlsvc", ex);
                    }
                });


                await DisplayMessage("Searching NetLimiter.dll...", 25);

                string dllPath = FindNetLimiterDll();
                if (dllPath == null)
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Title = "Select NetLimiter.dll";
                    openFileDialog.Filter = "NetLimiter DLL|NetLimiter.dll";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        dllPath = openFileDialog.FileName;
                    }
                    else
                    {
                        MessageBox.Show("NetLimiter.dll not found. Program will exit.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(1);
                        return;
                    }
                }

                await DisplayMessage("NetLimiter.dll found", 30);
                await DisplayMessage("Opening NetLimiter.dll...", 40);

                // Load the assembly
                ModuleDefMD module = ModuleDefMD.Load(dllPath);

                await DisplayMessage("NetLimiter.dll loaded successfully", 50);

                // Navigate to the NLLicense constructor
                await DisplayMessage("Navigating to NLLicense constructor...", 60);

                TypeDef nLLicenseType = module.Find("NetLimiter.Service.NLLicense", true);
                MethodDef nLLicenseConstructor = nLLicenseType.FindMethod(".ctor");

                // Modify the IL to change this.IsRegistered = false; to true;
                await DisplayMessage("Modifying NLLicense constructor...", 70);

                foreach (Instruction instruction in nLLicenseConstructor.Body.Instructions)
                {
                    if (instruction.OpCode == OpCodes.Ldc_I4_0) // Find the instruction loading false
                    {
                        instruction.OpCode = OpCodes.Ldc_I4_1; // Replace it with loading true
                        break;
                    }
                }

                await DisplayMessage("Modified NLLicense succesfully", 76);

                // Navigate to the InitLicense method
                await DisplayMessage("Navigating to InitLicense method...", 80);

                TypeDef nLServiceTempType = module.Find("NetLimiter.Service.NLServiceTemp", true);
                MethodDef initLicenseMethod = nLServiceTempType.FindMethod("InitLicense");

                // Modify the IL to change the expiration time
                await DisplayMessage("Modifying InitLicense succesfully", 90);

                foreach (Instruction instruction in initLicenseMethod.Body.Instructions)
                {
                    if (instruction.OpCode == OpCodes.Ldc_R8 && (double)instruction.Operand == 28.0) // Find the instruction loading 28.0
                    {
                        instruction.Operand = 99999.0; // Change it to 99999.0
                        break;
                    }
                }

                await DisplayMessage("Modified InitLicense method", 93);

                // Save the modified assembly
                await DisplayMessage("Saving the modified assembly...", 94);

                module.Write(Path.ChangeExtension(dllPath, ".patched.dll"));

                await DisplayMessage("Modified assembly saved successfully", 95);

                await DisplayMessage("Replacing patched dll with the original...", 96);

                // Replace the original DLL with the modified one
                await Task.Run(() => ReplaceDll(dllPath, module));

                await DisplayMessage("Replaced patched dll succesfully", 97);

                await DisplayMessage("Starting nlsvc service...", 98);

                // Start nlsvc
                await Task.Run(async () =>
                {
                    try
                    {
                        var process = Process.Start(new ProcessStartInfo
                        {
                            FileName = "net",
                            Arguments = "start nlsvc",
                            UseShellExecute = false,
                            CreateNoWindow = true
                        });

                        if (process != null)
                        {
                            process.WaitForExit();
                            await DisplayMessage("Nlsvc started successfully", 99);
                        }
                        else
                        {
                            throw new Exception("Failed to start 'net' process to start nlsvc");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error starting nlsvc", ex);
                    }
                });

                // Show final message
                await DisplayMessage("Net Limiter Succesfully Cracked!", 100);
                label1.Text = "You can close this program now!";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Display messages with delay and update progress bar
        private async Task DisplayMessage(string message, int progress)
        {
            if (label1.InvokeRequired)
            {
                label1.BeginInvoke((Action)(() => label1.Text = ""));
            }
            else
            {
                label1.Text = "";
            }

            Progressbar(progress);

            foreach (char c in message)
            {
                if (label1.InvokeRequired)
                {
                    label1.BeginInvoke((Action)(() => label1.Text += c));
                }
                else
                {
                    label1.Text += c;
                }
                await Task.Delay(25); // Adjust delay for typing animation
            }

            await Task.Delay(500); // Delay before clearing message

            if (label1.InvokeRequired)
            {
                label1.BeginInvoke((Action)(() => label1.Text = ""));
            }
            else
            {
                label1.Text = "";
            }
        }

        int previousprogress = 0;
        private async Task Progressbar(int progress)
        {

            customProgressBar1.Value = progress;

            previousprogress = progress;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            customProgressBar1.Value = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    public class CustomProgressBar : Control
    {
        private bool started = false;
        private int _value = 0;
        private int _targetValue = 0;
        private int _animationIncrement = 1;
        private Timer loadingAnimationTimer;
        private List<Particle> particles = new List<Particle>();
        private Random random = new Random();
        private int highlightedBlockIndex = 0; // Index of the currently highlighted block
        private Timer waveAnimationTimer;

        public int Value
        {
            get { return _value; }
            set
            {
                if (value == 0 && !started)
                {
                    started = true;
                    GenerateParticles(50);
                    loadingAnimationTimer.Start();
                    waveAnimationTimer.Start(); // Start the wave animation when the progress bar starts
                }

                if (value < 0)
                    _targetValue = 0;
                else if (value > 100)
                    _targetValue = 100;
                else
                    _targetValue = value;

                if (_targetValue != _value)
                {
                    if (_animationIncrement == 0)
                        _animationIncrement = Math.Sign(_targetValue - _value);
                    loadingAnimationTimer.Start();
                }
            }
        }

        public CustomProgressBar()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            InitializeLoadingAnimationTimer();
            InitializeWaveAnimationTimer();
        }

        private void InitializeLoadingAnimationTimer()
        {
            loadingAnimationTimer = new Timer();
            loadingAnimationTimer.Interval = 2; // Adjust speed of animation here
            loadingAnimationTimer.Tick += LoadingAnimationTimer_Tick;
        }

        private void LoadingAnimationTimer_Tick(object sender, EventArgs e)
        {
            GenerateParticles(1);

            if (_value != _targetValue)
            {
                _value += _animationIncrement;
                if ((_animationIncrement > 0 && _value > _targetValue) || (_animationIncrement < 0 && _value < _targetValue))
                    _value = _targetValue;

                UpdateParticles();

                Invalidate();
            }
            else
            {
                UpdateParticles(); // Update particles even when idle
                Invalidate();
            }
        }

        private void InitializeWaveAnimationTimer()
        {
            waveAnimationTimer = new Timer();
            waveAnimationTimer.Interval = 100; // Adjust speed of wave animation here
            waveAnimationTimer.Tick += WaveAnimationTimer_Tick;
        }

        private void WaveAnimationTimer_Tick(object sender, EventArgs e)
        {
            highlightedBlockIndex = (highlightedBlockIndex + 1) % 20; // Update the highlighted block index
            Invalidate();
        }

        private void GenerateParticles(int number)
        {
            // Generate particles at the edge where the current value of the progress bar is
            float progressWidth = ((float)_value / 100) * ClientRectangle.Width;
            if (progressWidth < ClientRectangle.Width)
            {
                int numParticles = random.Next(number, number); // Adjust number of particles
                for (int i = 0; i < numParticles; i++)
                {
                    float x = progressWidth - 1 + random.Next(3); // Randomize X coordinate near the edge
                    float y = random.Next(ClientRectangle.Height); // Randomize Y coordinate
                    float speedX = (float)random.NextDouble() * 2 - 1; // Adjust speed
                    float speedY = (float)random.NextDouble() * 2 - 1; // Adjust speed
                    Color color = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256)); // Random color
                    particles.Add(new Particle(x, y, speedX, speedY, color));
                }
            }
        }

        private void UpdateParticles()
        {
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                Particle particle = particles[i];
                particle.Update();
                if (!ClientRectangle.Contains(new Point((int)particle.X, (int)particle.Y)))
                {
                    particles.RemoveAt(i);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw background
            e.Graphics.Clear(Color.Black);

            // Draw border
            using (Pen borderPen = new Pen(Color.FromArgb(45, 45, 45), 2))
            {
                e.Graphics.DrawRectangle(borderPen, ClientRectangle);
            }

            // Draw blocks
            int blockCount = 20; // Number of blocks
            float blockWidth = (ClientRectangle.Width - 4) / (float)blockCount;
            for (int i = 0; i < blockCount; i++)
            {
                float blockX = 2 + i * (blockWidth + 1); // Adjusted to include a gap between blocks
                float blockProgress = ((float)_value / 100) * ClientRectangle.Width;
                if (blockX < blockProgress)
                {
                    float blockWidthAdjusted = Math.Min(blockWidth, blockProgress - blockX);
                    RectangleF blockRect = new RectangleF(blockX, 2, blockWidthAdjusted, ClientRectangle.Height - 4);

                    // Calculate color interpolation based on progress
                    float progress = i / (float)blockCount;
                    int red = (int)(255 * (1 - progress));
                    int green = (int)(255 * progress);
                    Color blockColor = Color.FromArgb(red, green, 0);

                    if (i == highlightedBlockIndex && _value != 100) // Highlight the current block with a lighter color
                    {
                        blockColor = Color.FromArgb(Math.Min(red + 50, 255), Math.Min(green + 50, 255), 0);
                        // Increase the size of the highlighted block
                        blockRect.Inflate(1, 1);
                    }

                    using (SolidBrush brush = new SolidBrush(blockColor))
                    {
                        e.Graphics.FillRectangle(brush, blockRect);
                    }
                }
            }

            // Draw particles
            foreach (Particle particle in particles)
            {
                e.Graphics.FillEllipse(new SolidBrush(particle.Color), particle.X, particle.Y, 2, 2); // Adjust size of particles
            }

            // Draw text
            using (Font font = new Font("Consolas", 10))
            using (SolidBrush brush = new SolidBrush(Color.Cyan))
            {
                string text = $"{_value}%";
                SizeF textSize = e.Graphics.MeasureString(text, font);
                PointF textLocation = new PointF((ClientRectangle.Width - textSize.Width) / 2, (ClientRectangle.Height - textSize.Height) / 2);
                e.Graphics.DrawString(text, font, brush, textLocation);
            }
        }

        private class Particle
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float SpeedX { get; set; }
            public float SpeedY { get; set; }
            public Color Color { get; set; }

            public Particle(float x, float y, float speedX, float speedY, Color color)
            {
                X = x;
                Y = y;
                SpeedX = speedX;
                SpeedY = speedY;
                Color = color;
            }

            public void Update()
            {
                X += SpeedX;
                Y += SpeedY;

                // Apply force towards the right
                SpeedX += 0.05f;
            }
        }
    }
}
