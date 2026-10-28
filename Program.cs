using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text.Json;
using NAudio.CoreAudioApi;

namespace PrismDisplay;

static class Native {
    [StructLayout(LayoutKind.Sequential)] public struct Effect { [MarshalAs(UnmanagedType.ByValArray, SizeConst=25)] public float[] matrix; }
    [DllImport("Magnification.dll", SetLastError=true)] public static extern bool MagInitialize();
    [DllImport("Magnification.dll", SetLastError=true)] public static extern bool MagUninitialize();
    [DllImport("Magnification.dll", SetLastError=true)] public static extern bool MagGetFullscreenColorEffect(ref Effect effect);
    [DllImport("Magnification.dll", SetLastError=true)] public static extern bool MagSetFullscreenColorEffect(ref Effect effect);
    [DllImport("user32.dll")] public static extern short GetAsyncKeyState(int key);
    [DllImport("user32.dll")] public static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")] public static extern uint GetWindowThreadProcessId(IntPtr h, out uint pid);
    [DllImport("user32.dll", SetLastError=true)] public static extern bool RegisterHotKey(IntPtr h,int id,uint modifiers,uint key);
    [DllImport("user32.dll")] public static extern bool UnregisterHotKey(IntPtr h,int id);
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)] public struct DisplaySettings {
        [MarshalAs(UnmanagedType.ByValTStr,SizeConst=32)] public string dmDeviceName;
        public short dmSpecVersion,dmDriverVersion,dmSize,dmDriverExtra; public int dmFields,dmPositionX,dmPositionY,dmDisplayOrientation,dmDisplayFixedOutput;
        public short dmColor,dmDuplex,dmYResolution,dmTTOption,dmCollate;
        [MarshalAs(UnmanagedType.ByValTStr,SizeConst=32)] public string dmFormName;
        public short dmLogPixels; public int dmBitsPerPel,dmPelsWidth,dmPelsHeight,dmDisplayFlags,dmDisplayFrequency,dmICMMethod,dmICMIntent,dmMediaType,dmDitherType,dmReserved1,dmReserved2,dmPanningWidth,dmPanningHeight;
    }
    [DllImport("user32.dll",CharSet=CharSet.Unicode)] public static extern bool EnumDisplaySettingsEx(string deviceName,int modeNum,ref DisplaySettings mode,int flags);
    [DllImport("user32.dll",CharSet=CharSet.Unicode)] public static extern int ChangeDisplaySettingsEx(string deviceName,ref DisplaySettings mode,IntPtr hwnd,int flags,IntPtr lParam);
    [DllImport("gdi32.dll", CharSet=CharSet.Unicode, SetLastError=true)] public static extern IntPtr CreateDC(string driver,string device,IntPtr output,IntPtr initData);
    [DllImport("gdi32.dll")] public static extern bool DeleteDC(IntPtr dc);
    [DllImport("gdi32.dll", SetLastError=true)] public static extern bool GetDeviceGammaRamp(IntPtr dc, [Out] ushort[] ramp);
    [DllImport("gdi32.dll", SetLastError=true)] public static extern bool SetDeviceGammaRamp(IntPtr dc, [In] ushort[] ramp);
    [DllImport("dwmapi.dll")] public static extern int DwmSetWindowAttribute(IntPtr hwnd,int attribute,ref int value,int size);
    [DllImport("user32.dll")] public static extern void keybd_event(byte vk,byte scan,uint flags,UIntPtr extraInfo);
    [DllImport("user32.dll")] public static extern bool DestroyIcon(IntPtr icon);
    public static bool Down(int k)=>(GetAsyncKeyState(k)&0x8000)!=0;
}
record Look(int Saturation=100,int Brightness=0,int Contrast=100,int Warmth=0,int Red=100,int Green=100,int Blue=100,int Boost=20,int Gamma=100,int Hue=0);
class Settings {
    public Look Current {get;set;}=new();
    public Dictionary<string,Look> Looks {get;set;}=new();
    public Dictionary<string,string> Games {get;set;}=new();
    public string Theme {get;set;}="Carbon";
    public string Accent {get;set;}="Ice";
    public bool SnapSaturation {get;set;}=true;
    public string VisionMode {get;set;}="Normal";
    public int EyeWarmth {get;set;}=0;
    public int EyeDim {get;set;}=0;
    public bool DarkEnabled {get;set;}=true;
    public int DarkStrength {get;set;}=55;
    public int DarkBlackLevel {get;set;}=0;
    public string DarkColour {get;set;}="Green";
    public bool DarkKeepColours {get;set;}=false;
    public string DarkMode {get;set;}="Hold";
    public bool DarkOnlyGames {get;set;}=false;
    public bool DarkFade {get;set;}=true;
    public int DarkKey {get;set;}=0x77;
    public int DarkModifiers {get;set;}=0;
    public int LoudnessThreshold {get;set;}=50;
    public bool UtilityShortcuts {get;set;}=false;
    public int QuickKey {get;set;}=0x56;
    public int QuickModifiers {get;set;}=3;
    public bool MediaShortcuts {get;set;}=false;
    public int MediaPopupKey {get;set;}=0x4D;
    public int MediaPopupModifiers {get;set;}=3;
    public int MediaPlayKey {get;set;}=0;
    public int MediaPlayModifiers {get;set;}=0;
    public int MediaNextKey {get;set;}=0;
    public int MediaNextModifiers {get;set;}=0;
    public int MediaPrevKey {get;set;}=0;
    public int MediaPrevModifiers {get;set;}=0;
    public int WindowKey {get;set;}=0x50;
    public int WindowModifiers {get;set;}=3;
    public int CrossKey {get;set;}=0x43;
    public int CrossModifiers {get;set;}=3;
    public bool CloseToTray {get;set;}=true;
    public string GameDetection {get;set;}="Focused window";
    public int CrossOffsetX {get;set;}=0;
    public int CrossOffsetY {get;set;}=0;
    public int CrossAngle {get;set;}=0;
    public bool CrossSpin {get;set;}=false;
    public int CrossSpinSpeed {get;set;}=90;
    public string CrossAnimation {get;set;}="None";
    public int CrossAnimationSpeed {get;set;}=100;
    public bool EffectEnabled {get;set;}=true;
    public bool CrosshairEnabled {get;set;}=false;
    public bool AutoGames {get;set;}=false;
    public bool Hotkeys {get;set;}=false;
    public string BoostKey {get;set;}="F8";
    public string CrossColor {get;set;}="Lime";
    public string CrossColorHex {get;set;}="";
    public string CrossShape {get;set;}="Cross";
    public int CrossSize {get;set;}=12;
    public int CrossGap {get;set;}=4;
    public int CrossThickness {get;set;}=2;
    public bool CrossOutline {get;set;}=true;
    public bool CrossDot {get;set;}=false;
    public int Monitor {get;set;}=0;
    public string[] QuickLooks {get;set;}=new[]{"Original","Vivid","Treeline","Night"};
}
static class Palette {
    public static Color Bg,Side,Card,Card2,Line,Text,Muted,Violet,Green;
    public static void Configure(string theme,string accent){
        (Bg,Side,Card,Card2,Line)=theme switch {
            "Carbon" => (Color.FromArgb(17,17,19),Color.FromArgb(20,20,22),Color.FromArgb(27,27,29),Color.FromArgb(38,38,41),Color.FromArgb(65,65,68)),
            "Midnight" => (Color.FromArgb(9,15,27),Color.FromArgb(14,23,39),Color.FromArgb(18,30,48),Color.FromArgb(29,43,64),Color.FromArgb(54,74,98)),
            "Slate" => (Color.FromArgb(22,25,30),Color.FromArgb(28,32,38),Color.FromArgb(35,40,47),Color.FromArgb(45,51,59),Color.FromArgb(68,76,88)),
            "Aubergine" => (Color.FromArgb(20,13,24),Color.FromArgb(28,18,33),Color.FromArgb(37,24,43),Color.FromArgb(48,33,54),Color.FromArgb(77,56,83)),
            _ => (Color.FromArgb(12,14,21),Color.FromArgb(18,20,29),Color.FromArgb(24,27,39),Color.FromArgb(31,34,48),Color.FromArgb(49,53,70))};
        Text=Color.FromArgb(235,237,246);Muted=Color.FromArgb(166,174,191);Green=Color.FromArgb(117,230,185);
        Violet=accent switch {"Ice"=>Color.FromArgb(235,238,244),"Cyan"=>Color.FromArgb(95,220,227),"Blue"=>Color.FromArgb(112,167,255),"Rose"=>Color.FromArgb(255,137,178),"Mint"=>Color.FromArgb(120,232,185),"Amber"=>Color.FromArgb(255,201,111),_=>Color.FromArgb(159,142,255)};
    }
    public static Font Font(float n,FontStyle style=FontStyle.Regular)=>new("Segoe UI Variable",n,style);
}
static class Program {
 [STAThread] static void Main(string[] args){ if(args.Length==2 && args[0]=="--resolution-guard"){ResolutionGuard.Run(args[1]);return;} using var mutex=new System.Threading.Mutex(true,"Local\\PrismDisplaySingleInstance",out bool first);if(!first){MessageBox.Show("Prism is already running.");return;} Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);Application.EnableVisualStyles();Application.SetCompatibleTextRenderingDefault(false);Application.Run(new MainWindow(args.Contains("--start-minimized",StringComparer.OrdinalIgnoreCase))); }
}
sealed class SmoothSlider:Control {
    public event EventHandler? Changed;
    public string Caption {get;set;}=""; public string Unit{get;set;}=""; public bool Hero{get;set;}
    public int Min{get;set;} public int Max{get;set;}=100; public int SnapStep{get;set;} public int SnapTolerance{get;set;}=3; int val;
    public int Value {get=>val;set{int v=Math.Clamp(value,Min,Max);if(v==val)return;val=v;Invalidate();Changed?.Invoke(this,EventArgs.Empty);}}
    public SmoothSlider(){Height=64;MinimumSize=new Size(180,64);DoubleBuffered=true;TabStop=true;Cursor=Cursors.Hand;BackColor=Palette.Card;SetStyle(ControlStyles.Selectable,true);}
    protected override void OnPaint(PaintEventArgs e){
        base.OnPaint(e);var g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;using var text=new SolidBrush(Palette.Text);using var accent=new SolidBrush(Palette.Violet);using var labelFont=Palette.Font(9.5f,FontStyle.Bold);using var valueFont=Palette.Font(Hero?26:10,FontStyle.Bold);
        g.DrawString(Caption,labelFont,text,2,2);string v=val+Unit;var measured=g.MeasureString(v,valueFont);if(Hero)g.DrawString(v,valueFont,text,0,23);else g.DrawString(v,valueFont,accent,Width-measured.Width-3,1);
        float x1=9,x2=Width-10,y=Hero?86:43,x=x1+(x2-x1)*(val-Min)/(float)(Max-Min);using var track=new Pen(Palette.Line,Hero?4:3){StartCap=LineCap.Round,EndCap=LineCap.Round};using var selected=new Pen(Palette.Violet,Hero?4:3){StartCap=LineCap.Round,EndCap=LineCap.Round};g.DrawLine(track,x1,y,x2,y);if(x>x1)g.DrawLine(selected,x1,y,x,y);
        if(SnapStep>0){using var marks=new Pen(Palette.Muted,1);for(int mark=Min;mark<=Max;mark+=SnapStep){float mx=x1+(x2-x1)*(mark-Min)/(float)(Max-Min);g.DrawLine(marks,mx,y+8,mx,y+12);}}
        if(Hero){using var tickFont=Palette.Font(8);using var muted=new SolidBrush(Palette.Muted);for(int mark=Min;mark<=Max;mark+=100){float mx=x1+(x2-x1)*(mark-Min)/(float)(Max-Min);string t=mark.ToString();g.DrawString(t,tickFont,muted,Math.Clamp(mx-5,0,Width-30),y+15);}}
        g.FillEllipse(accent,x-7,y-7,14,14);using var white=new SolidBrush(Color.White);g.FillEllipse(white,x-3,y-3,6,6);if(Focused){using var pen=new Pen(Palette.Violet,1);g.DrawRectangle(pen,0,0,Width-1,Height-1);}
    }
    void ChangeAt(int x){float f=Math.Clamp((x-9f)/Math.Max(1,Width-19f),0,1);int raw=Min+(int)Math.Round(f*(Max-Min));if(SnapStep>0&&(ModifierKeys&Keys.Shift)==0){int snap=Min+(int)Math.Round((raw-Min)/(double)SnapStep)*SnapStep;if(Math.Abs(raw-snap)<=SnapTolerance)raw=snap;}Value=raw;}
    protected override void OnMouseDown(MouseEventArgs e){base.OnMouseDown(e);if(e.Button==MouseButtons.Left){Focus();Capture=true;ChangeAt(e.X);}}
    protected override void OnMouseMove(MouseEventArgs e){base.OnMouseMove(e);if(Capture&&e.Button==MouseButtons.Left)ChangeAt(e.X);}
    protected override void OnMouseUp(MouseEventArgs e){base.OnMouseUp(e);Capture=false;}
    protected override bool IsInputKey(Keys k)=>k is Keys.Left or Keys.Right or Keys.Up or Keys.Down or Keys.Home or Keys.End || base.IsInputKey(k);
    protected override void OnKeyDown(KeyEventArgs e){base.OnKeyDown(e);if(e.KeyCode is Keys.Left or Keys.Down)Value--;else if(e.KeyCode is Keys.Right or Keys.Up)Value++;else if(e.KeyCode==Keys.Home)Value=Min;else if(e.KeyCode==Keys.End)Value=Max;}
}
sealed class RoundedButton:Button {
    bool hover,pressed;float glow;readonly System.Windows.Forms.Timer animation=new(){Interval=16};
    public RoundedButton(){SetStyle(ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer,true);animation.Tick+=(_,__)=>{float target=pressed ? 1f : (hover ? .55f : 0f);glow+=(target-glow)*.28f;if(Math.Abs(target-glow)<.015f){glow=target;animation.Stop();}Invalidate();};}
    protected override void OnMouseEnter(EventArgs e){base.OnMouseEnter(e);hover=true;animation.Start();}
    protected override void OnMouseLeave(EventArgs e){base.OnMouseLeave(e);hover=false;pressed=false;animation.Start();}
    protected override void OnMouseDown(MouseEventArgs e){base.OnMouseDown(e);if(e.Button==MouseButtons.Left){pressed=true;animation.Start();}}
    protected override void OnMouseUp(MouseEventArgs e){base.OnMouseUp(e);pressed=false;animation.Start();}
    protected override void OnPaint(PaintEventArgs e){
        var g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;g.Clear(Parent?.BackColor??Palette.Bg);using var path=new GraphicsPath();float r=4;var rect=new RectangleF(0,0,Width-1,Height-1);path.AddArc(rect.X,rect.Y,r*2,r*2,180,90);path.AddArc(rect.Right-r*2,rect.Y,r*2,r*2,270,90);path.AddArc(rect.Right-r*2,rect.Bottom-r*2,r*2,r*2,0,90);path.AddArc(rect.X,rect.Bottom-r*2,r*2,r*2,90,90);path.CloseFigure();
        Color baseColor=BackColor;float t=glow*.22f;Color bg=Color.FromArgb((int)(baseColor.R+(255-baseColor.R)*t),(int)(baseColor.G+(255-baseColor.G)*t),(int)(baseColor.B+(255-baseColor.B)*t));using var brush=new SolidBrush(bg);g.FillPath(brush,path);
        var flags=TextFormatFlags.VerticalCenter|TextFormatFlags.SingleLine|TextFormatFlags.EndEllipsis|(TextAlign==ContentAlignment.MiddleLeft?TextFormatFlags.Left:TextFormatFlags.HorizontalCenter);var textRect=new Rectangle(Padding.Left,0,Width-Padding.Horizontal,Height);TextRenderer.DrawText(g,Text,Font,textRect,Enabled?ForeColor:Palette.Muted,flags);if(Focused&&ShowFocusCues){using var pen=new Pen(Palette.Violet,1);g.DrawPath(pen,path);}
    }
    protected override void Dispose(bool disposing){if(disposing)animation.Dispose();base.Dispose(disposing);}
}
sealed class MainWindow:Form {
    readonly string folder=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"PrismDisplay");
    string FilePath=>Path.Combine(folder,"settings-v2.json");
    GammaController? gamma; int lastGammaRequest=int.MinValue; string? gammaError;
    Settings data=new(); Dictionary<string,Look> looks=new(StringComparer.OrdinalIgnoreCase); Dictionary<string,SmoothSlider> sliders=new();
    readonly System.Windows.Forms.Timer timer=new(){Interval=20}; Native.Effect previous=new(){matrix=new float[25]}; bool initialized,suppress,boostHeld,failed,dirty,emergencyLatch,darkToggle,lastDarkKey; float boostLevel; long nextDarkProcessPoll; bool darkGameRunning; int elapsed=0,saveElapsed=0; string? autoGame,autoLook,runningGame; Look? manualBeforeGame,autoAppliedLook; long nextProcessPoll; Label? gameStatus;
    QuickColourPopup? quickPopup; MediaPopup? mediaPopup; readonly Dictionary<int,bool> mouseEdges=new();
    AudioDucker? ducker; readonly System.Windows.Forms.Timer audioTimer=new(){Interval=50}; CheckBox? audioToggle; Label? audioInfo; bool syncingAudio; int audioInfoCounter;
    Panel content=new(){Dock=DockStyle.Fill,BackColor=Palette.Bg}; Label title=new(),subtitle=new(),status=new(),stateTag=new(); Button? power; Dictionary<string,Button> nav=new();
    ComboBox? looksBox, gameLook, shapeBox, colorBox, monitorBox; ComboBox[] quickBoxes=new ComboBox[4]; ListBox? gamesList; TextBox? processBox; CheckBox? autoBox,hotkeysBox,crossBox,outlineBox,dotBox;
    SmoothSlider? crossSize,crossGap,crossThickness; Crosshair? cross; bool registered; string currentPage="COLOUR";
    NotifyIcon? tray; ContextMenuStrip? trayMenu; Icon? trayIcon; bool exiting, notified; readonly bool startMinimized;
    readonly System.Windows.Forms.Timer animationTimer=new(){Interval=33};
    static Dictionary<string,Look> Builtins()=>new(StringComparer.OrdinalIgnoreCase){["Original"]=new(),["Vivid"]=new(200,0,105,0),["Treeline"]=new(175,5,110,3),["Night"]=new(145,12,95,0),["Snow"]=new(135,-8,105,-5),["Warm"]=new(130,2,100,18),["Muted"]=new(65,0,100,0),["Forest"]=new(210,2,108,3),["Desert"]=new(190,0,105,10),["Smoke"]=new(165,7,110,-2),["Long range"]=new(175,3,112,0)};
    public MainWindow(bool startMinimized=false){this.startMinimized=startMinimized;Text="Prism";Size=new Size(1320,900);MinimumSize=new Size(820,620);StartPosition=FormStartPosition.CenterScreen;BackColor=Palette.Bg;ForeColor=Palette.Text;Font=Palette.Font(9);AutoScaleMode=AutoScaleMode.Dpi;
        LoadSettings(); Palette.Configure(data.Theme,data.Accent); BackColor=Palette.Bg; content.BackColor=Palette.Bg; gamma=new GammaController(folder); gammaError=gamma.Recover(); initialized=Native.MagInitialize();if(initialized&&!Native.MagGetFullscreenColorEffect(ref previous)){previous.matrix=new float[25];for(int i=0;i<5;i++)previous.matrix[i*5+i]=1;}
        Build();SetLook(data.Current);UpdatePower();SetupTray();if(trayIcon!=null)Icon=trayIcon;HandleCreated+=(_,__)=>ApplyDarkCaption();animationTimer.Tick+=(_,__)=>cross?.Invalidate();audioTimer.Tick+=(_,__)=>AudioTick();if(data.CrosshairEnabled)UpdateCross();timer.Tick+=(_,__)=>OnTick();timer.Start();
        FormClosing+=OnClosing;Resize+=(_,__)=>{if(WindowState==FormWindowState.Minimized&&data.CloseToTray)HideToTray();};
        Shown+=(_,__)=>{var area=Screen.FromControl(this).WorkingArea;if(Width>area.Width||Height>area.Height){Size=new Size(Math.Min(Width,area.Width),Math.Min(Height,area.Height));Location=new Point(area.Left,area.Top);}ApplyDarkCaption();if(startMinimized)HideToTray();if(!initialized)MessageBox.Show("Windows Magnification could not start. Display colour effects are unavailable.","Prism",MessageBoxButtons.OK,MessageBoxIcon.Warning);else Apply(); if(data.Hotkeys||data.UtilityShortcuts||data.MediaShortcuts)RegisterShortcuts();};
    }
    void LoadSettings(){try{if(File.Exists(FilePath)){string json=File.ReadAllText(FilePath);data=JsonSerializer.Deserialize<Settings>(json)??new();using var doc=JsonDocument.Parse(json);if(!doc.RootElement.TryGetProperty("DarkKey",out _))data.DarkKey=data.BoostKey switch {"F6"=>0x75,"F7"=>0x76,"F9"=>0x78,"F10"=>0x79,"F11"=>0x7a,"F12"=>0x7b,_=>0x77};}else{string old=Path.Combine(folder,"looks.json");if(File.Exists(old))data.Looks=JsonSerializer.Deserialize<Dictionary<string,Look>>(File.ReadAllText(old))??new();}}catch{data=new();} data.Looks??=new();data.Games??=new();data.Games=data.Games.GroupBy(p=>p.Key,StringComparer.OrdinalIgnoreCase).ToDictionary(g=>g.Key,g=>g.Last().Value,StringComparer.OrdinalIgnoreCase);data.Current??=new();if(data.Current.Gamma==0)data.Current=data.Current with { Gamma=100 };foreach(var k in data.Looks.Keys.ToArray())if(data.Looks[k]!=null&&data.Looks[k].Gamma==0)data.Looks[k]=data.Looks[k] with { Gamma=100 };if(data.QuickLooks==null||data.QuickLooks.Length!=4)data.QuickLooks=new[]{"Original","Vivid","Treeline","Night"};foreach(var p in Builtins())looks[p.Key]=p.Value;foreach(var p in data.Looks)if(!looks.ContainsKey(p.Key)&&p.Value!=null)looks[p.Key]=p.Value;}
    void SaveSettings(){try{Directory.CreateDirectory(folder);data.Current=autoGame!=null&&manualBeforeGame!=null?manualBeforeGame:Current();data.Looks=looks.Where(p=>!Builtins().ContainsKey(p.Key)).ToDictionary(p=>p.Key,p=>p.Value);File.WriteAllText(FilePath,JsonSerializer.Serialize(data,new JsonSerializerOptions{WriteIndented=true}));dirty=false;}catch(Exception e){status.Text="Settings could not be saved: "+e.Message;dirty=false;}}
    void Mark(){if(!suppress)dirty=true;}
    void Build(string firstPage="COLOUR"){
        var shell=new TableLayoutPanel{Dock=DockStyle.Fill,ColumnCount=2,RowCount=1,Margin=Padding.Empty};shell.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,160));shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));Controls.Add(shell);
        var side=new Panel{Dock=DockStyle.Fill,BackColor=Palette.Side,Padding=new Padding(10,20,10,16)};shell.Controls.Add(side,0,0);
        var sideLayout=new TableLayoutPanel{Dock=DockStyle.Fill,RowCount=4,ColumnCount=1,BackColor=Palette.Side};sideLayout.RowStyles.Add(new RowStyle(SizeType.Absolute,44));sideLayout.RowStyles.Add(new RowStyle(SizeType.Absolute,37));sideLayout.RowStyles.Add(new RowStyle(SizeType.Percent,100));sideLayout.RowStyles.Add(new RowStyle(SizeType.Absolute,76));side.Controls.Add(sideLayout);
        var brand=new PrismLogo{Dock=DockStyle.Fill};sideLayout.Controls.Add(brand,0,0);
        var brandSub=new Label{Text="DISPLAY STUDIO",ForeColor=Palette.Muted,Font=Palette.Font(8,FontStyle.Bold),Dock=DockStyle.Fill};sideLayout.Controls.Add(brandSub,0,1);
        var stack=new FlowLayoutPanel{Dock=DockStyle.Fill,FlowDirection=FlowDirection.TopDown,WrapContents=false,AutoScroll=true,Padding=new Padding(0,15,0,0),BackColor=Palette.Side};sideLayout.Controls.Add(stack,0,2);
        stack.Controls.Add(new Label{Text="WORKSPACE",Width=118,Height=30,ForeColor=Palette.Muted,Font=Palette.Font(8,FontStyle.Bold)});
        foreach(string n in new[]{"COLOUR","LOOKS","DARK BOOST","CROSSHAIR","RESOLUTION","GAMES","VISION & CARE","MEDIA","RECORDING","APPEARANCE","SETTINGS","DIAGNOSTICS"}){string label=n switch{"COLOUR"=>"◐  Display","LOOKS"=>"▤  Library","DARK BOOST"=>"◒  Dark Boost","CROSSHAIR"=>"⊕  Crosshair","RESOLUTION"=>"▣  Resolution","GAMES"=>"◇  Games","VISION & CARE"=>"◉  Eye care","MEDIA"=>"♪  Media","RECORDING"=>"▢  Recording","APPEARANCE"=>"◈  Appearance","SETTINGS"=>"⚙  Settings","DIAGNOSTICS"=>"≡  Diagnostics",_=>n};var b=MakeButton(label,118,32);b.Font=Palette.Font(8.5f);b.TextAlign=ContentAlignment.MiddleLeft;b.Padding=new Padding(8,0,0,0);b.Margin=new Padding(0,1,0,3);b.Click+=(_,__)=>ShowPage(n);stack.Controls.Add(b);nav[n]=b;}
        var tip=new Label{Text="DESKTOP COLOUR\nPowered by Windows\nMagnification API",Dock=DockStyle.Fill,ForeColor=Palette.Muted,Font=Palette.Font(8),Padding=new Padding(0,10,0,0)};sideLayout.Controls.Add(tip,0,3);
        var main=new TableLayoutPanel{Dock=DockStyle.Fill,RowCount=3,ColumnCount=1,Padding=new Padding(28,20,28,12),BackColor=Palette.Bg};main.RowStyles.Add(new RowStyle(SizeType.Absolute,92));main.RowStyles.Add(new RowStyle(SizeType.Percent,100));main.RowStyles.Add(new RowStyle(SizeType.Absolute,40));shell.Controls.Add(main,1,0);
        var head=new Panel{Dock=DockStyle.Fill};main.Controls.Add(head,0,0);title=new Label{Text="Display",Font=Palette.Font(21,FontStyle.Bold),ForeColor=Palette.Text,AutoSize=true,Location=new Point(0,0)};subtitle=new Label{Text="Fine-tune your display in real time.",Font=Palette.Font(9.5f),ForeColor=Palette.Muted,AutoSize=true,Location=new Point(2,46)};head.Controls.Add(title);head.Controls.Add(subtitle);
        power=MakeButton("●  EFFECT ON",150,36);power.Anchor=AnchorStyles.Top|AnchorStyles.Right;power.Location=new Point(625,8);head.Controls.Add(power);head.Resize+=(_,__)=>power.Left=head.ClientSize.Width-power.Width;power.Click+=(_,__)=>{data.EffectEnabled=!data.EffectEnabled;if(!data.EffectEnabled){darkToggle=false;boostLevel=0;}UpdatePower();Apply();Mark();};
        main.Controls.Add(content,0,1);var footer=new Panel{Dock=DockStyle.Fill};main.Controls.Add(footer,0,2);status=new Label{Text="Ready",Dock=DockStyle.Fill,TextAlign=ContentAlignment.MiddleLeft,ForeColor=Palette.Muted,Font=Palette.Font(8.5f)};footer.Controls.Add(status);stateTag=new Label{Text="CTRL + ALT + F12  •  ALL OFF",Dock=DockStyle.Right,Width=230,TextAlign=ContentAlignment.MiddleRight,ForeColor=Palette.Muted,Font=Palette.Font(8,FontStyle.Bold)};footer.Controls.Add(stateTag);
        ShowPage(firstPage);
    }
    static Button MakeButton(string text,int w=180,int h=38){var b=new RoundedButton{Text=text,Width=w,Height=h,FlatStyle=FlatStyle.Flat,BackColor=Palette.Card2,ForeColor=Palette.Text,Font=Palette.Font(9,FontStyle.Bold),Cursor=Cursors.Hand,Margin=new Padding(0,5,8,5)};b.FlatAppearance.BorderSize=0;b.FlatAppearance.MouseOverBackColor=Color.FromArgb(62,59,87);return b;}
    static Label Label(string text,int height=24,float size=9,Color? color=null,FontStyle style=FontStyle.Regular)=>new(){Text=text,Height=height,Width=650,ForeColor=color??Palette.Muted,Font=Palette.Font(size,style),Margin=new Padding(0,0,0,2)};
    static ComboBox Combo(int width=220){return new ComboBox{Width=width,DropDownStyle=ComboBoxStyle.DropDownList,FlatStyle=FlatStyle.Flat,BackColor=Palette.Card2,ForeColor=Palette.Text,Font=Palette.Font(10),Margin=new Padding(0,6,8,8)};}
    static CheckBox Check(string text,bool value){return new CheckBox{Text=text,Checked=value,ForeColor=Palette.Text,AutoSize=true,Font=Palette.Font(9.5f),Margin=new Padding(0,9,0,10)};}
    static FlowLayoutPanel Row(int height=51){return new FlowLayoutPanel{Width=650,Height=height,FlowDirection=FlowDirection.LeftToRight,WrapContents=false,Margin=new Padding(0,0,0,4)};}
    FlowLayoutPanel NewPage(){var page=new FlowLayoutPanel{Dock=DockStyle.Fill,FlowDirection=FlowDirection.TopDown,WrapContents=false,AutoScroll=true,Padding=new Padding(18,16,18,20),BackColor=Palette.Card};content.Controls.Add(page);page.Resize+=(_,__)=>FitPage(page);return page;}
    void FitPage(FlowLayoutPanel page){if(page.IsDisposed||page.ClientSize.Width<100)return;int width=Math.Max(500,page.ClientSize.Width-page.Padding.Horizontal-SystemInformation.VerticalScrollBarWidth-8);foreach(Control child in page.Controls){if(child.Tag==null&&child.Width>=600)child.Tag="fluid";if(child.Tag is string tag&&tag=="fluid"&&child.Width!=width)child.Width=width;}}
    void AddSection(FlowLayoutPanel p,string eyebrow,string description){p.Controls.Add(Label(eyebrow,26,9,Palette.Violet,FontStyle.Bold));p.Controls.Add(Label(description,34,9));}
    void ShowPage(string name){if(currentPage=="COLOUR" && sliders.Count>0)data.Current=Current(); sliders.Clear(); currentPage=name;foreach(var n in nav) {n.Value.BackColor=n.Key==name?Palette.Card2:Palette.Side;n.Value.ForeColor=n.Key==name?Palette.Violet:Palette.Muted;}foreach(Control old in content.Controls.Cast<Control>().ToArray())old.Dispose();var page=NewPage();switch(name){case "COLOUR":title.Text="Display";subtitle.Text="Saturation and fine-tuning for your screen.";ColourPage(page);break;case "LOOKS":title.Text="Your looks";subtitle.Text="Save, switch and shortcut your favourite settings.";LooksPage(page);break;case "CROSSHAIR":title.Text="Crosshair";subtitle.Text="Build a minimal overlay that stays out of your way.";CrossPage(page);break;case "RESOLUTION":title.Text="Resolution & scaling";subtitle.Text="Choose a display mode with a timed safety revert.";ResolutionPage(page);break;case "GAMES":title.Text="Game profiles";subtitle.Text="Automatically apply a look when a game is focused.";GamesPage(page);break;case "MEDIA":title.Text="Audio & media";subtitle.Text="Control media and open Windows audio settings.";MediaPage(page);break;case "RECORDING":title.Text="Recording";subtitle.Text="Check what your capture actually sees.";RecordingPage(page);break;case "DARK BOOST":title.Text="Dark Boost";subtitle.Text="Amplify dark scenes while holding or toggling a shortcut.";DarkPage(page);break;case "VISION & CARE":title.Text="Vision & eye care";subtitle.Text="Simple colour assistance and comfortable viewing controls.";VisionPage(page);break;case "DIAGNOSTICS":title.Text="Diagnostics";subtitle.Text="Check your display and profile status.";DiagnosticsPage(page);break;case "SETTINGS":title.Text="Settings";subtitle.Text="Control background behaviour and startup.";SettingsPage(page);break;case "APPEARANCE":title.Text="Make it yours";subtitle.Text="Choose the look and accent of your workspace.";AppearancePage(page);break;default:title.Text="Diagnostics";DiagnosticsPage(page);break;} FitPage(page); }
    void ColourPage(FlowLayoutPanel p){
        AddSection(p,"01   /   DISPLAY COLOUR","Drag a slider or use the arrow keys. Changes apply immediately.");
        AddSlider(p,"Saturation",0,350,data.Current.Saturation,"%",v=>data.Current=Current());
        var snap=Check("Snap saturation near 50% marks (hold Shift while dragging to bypass)",data.SnapSaturation);snap.CheckedChanged+=(_,__)=>{data.SnapSaturation=snap.Checked;if(sliders.TryGetValue("Saturation",out var sl))sl.SnapStep=data.SnapSaturation?50:0;sliders["Saturation"].Invalidate();Mark();};p.Controls.Add(snap);
        AddSection(p,"FINE TUNE","Colour adjustments apply together and are saved with your looks.");
        var grid=new TableLayoutPanel{Width=620,Height=260,ColumnCount=2,RowCount=4,Margin=new Padding(0,0,0,5)};grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50));grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50));for(int i=0;i<4;i++)grid.RowStyles.Add(new RowStyle(SizeType.Percent,25));p.Controls.Add(grid);
        void GridSlider(string name,int min,int max,int value,string unit,int col,int row){var slider=new SmoothSlider{Caption=name,Min=min,Max=max,Unit=unit,Value=Math.Clamp(value,min,max),Dock=DockStyle.Fill,Margin=new Padding(0,0,14,0)};sliders[name]=slider;slider.Changed+=(_,__)=>{if(!suppress){data.Current=Current();Apply();Mark();}};grid.Controls.Add(slider,col,row);}
        GridSlider("Brightness",-50,50,data.Current.Brightness,"",0,0);GridSlider("Contrast",50,150,data.Current.Contrast,"%",1,0);GridSlider("Gamma",50,200,data.Current.Gamma,"%",0,1);GridSlider("Warmth",-50,50,data.Current.Warmth,"",1,1);GridSlider("Hue shift",-180,180,data.Current.Hue,"°",0,2);GridSlider("Red",50,150,data.Current.Red,"%",1,2);GridSlider("Green",50,150,data.Current.Green,"%",0,3);GridSlider("Blue",50,150,data.Current.Blue,"%",1,3);
        p.Controls.Add(Label("Gamma support depends on your display driver and HDR settings.",30,8.5f));
        p.Controls.Add(Label("QUICK LOOKS",24,8,Palette.Violet,FontStyle.Bold));var quick=Row();foreach(string name in new[]{"Original","Vivid","Forest","Night"}){var button=MakeButton(name,140);button.Click+=(_,__)=>{if(looks.TryGetValue(name,out var look))SetLook(look);};quick.Controls.Add(button);}p.Controls.Add(quick);
        var reset=MakeButton("↺   Reset colour",190);reset.Click+=(_,__)=>SetLook(new());p.Controls.Add(reset);p.Controls.Add(Label("100% saturation is unchanged. Extreme values can clip colours.",35,8.5f));
    }
    void AddSlider(FlowLayoutPanel p,string name,int min,int max,int value,string unit,Action<int> changed){var s=new SmoothSlider{Caption=name,Min=min,Max=max,Unit=unit,Width=620,SnapStep=name=="Saturation"&&data.SnapSaturation?50:0,Hero=name=="Saturation",Value=Math.Clamp(value,min,max),Margin=new Padding(0,0,0,2)};if(s.Hero)s.Height=116;sliders[name]=s;s.Changed+=(_,__)=>{if(!suppress){changed(s.Value);if(name=="Saturation")quickPopup?.SetSaturation(s.Value);Apply();Mark();}};p.Controls.Add(s);}
    Look Current(){int V(string key,int fallback)=>sliders.TryGetValue(key,out var s)?s.Value:fallback;var c=data.Current;return new(V("Saturation",c.Saturation),V("Brightness",c.Brightness),V("Contrast",c.Contrast),V("Warmth",c.Warmth),V("Red",c.Red),V("Green",c.Green),V("Blue",c.Blue),V("Boost",c.Boost),V("Gamma",c.Gamma),V("Hue shift",c.Hue));}
    void SetLook(Look look){data.Current=new Look(Math.Clamp(look.Saturation,0,350),Math.Clamp(look.Brightness,-50,50),Math.Clamp(look.Contrast,50,150),Math.Clamp(look.Warmth,-50,50),Math.Clamp(look.Red,50,150),Math.Clamp(look.Green,50,150),Math.Clamp(look.Blue,50,150),Math.Clamp(look.Boost,0,60),Math.Clamp(look.Gamma,50,200),Math.Clamp(look.Hue,-180,180));suppress=true;foreach(var (n,v) in new[]{("Saturation",data.Current.Saturation),("Brightness",data.Current.Brightness),("Contrast",data.Current.Contrast),("Warmth",data.Current.Warmth),("Red",data.Current.Red),("Green",data.Current.Green),("Blue",data.Current.Blue),("Boost",data.Current.Boost),("Gamma",data.Current.Gamma),("Hue shift",data.Current.Hue)})if(sliders.TryGetValue(n,out var s)&&!s.IsDisposed)s.Value=v;suppress=false;quickPopup?.SetSaturation(data.Current.Saturation);Apply();Mark();}
    void FillLooks(ComboBox box,string? select=null){box.Items.Clear();foreach(var n in looks.Keys)box.Items.Add(n);box.SelectedItem=select??"Original";if(box.SelectedIndex<0&&box.Items.Count>0)box.SelectedIndex=0;}
    void LooksPage(FlowLayoutPanel p){AddSection(p,"LOOK LIBRARY","Built-in scene looks and your own saved colour settings.");looksBox=Combo(300);FillLooks(looksBox);p.Controls.Add(looksBox);var row=Row();var apply=MakeButton("Apply look",150);apply.Click+=(_,__)=>{if(looksBox.SelectedItem is string n&&looks.TryGetValue(n,out var l))SetLook(l);};var save=MakeButton("Save current",150);save.Click+=(_,__)=>SaveLook();var del=MakeButton("Delete custom",150);del.Click+=(_,__)=>DeleteLook();row.Controls.AddRange(new Control[]{apply,save,del});p.Controls.Add(row);
        p.Controls.Add(Label("QUICK SWITCH  /  GLOBAL HOTKEYS",28,9,Palette.Violet,FontStyle.Bold));p.Controls.Add(Label("Assign looks to Ctrl + Alt + 1–4. Shortcuts must be enabled.",30));hotkeysBox=Check("Enable global look shortcuts",data.Hotkeys);hotkeysBox.CheckedChanged+=(_,__)=>{data.Hotkeys=hotkeysBox.Checked;RegisterShortcuts();Mark();};p.Controls.Add(hotkeysBox);
        for(int i=0;i<4;i++){int index=i;var r=Row(44);r.Controls.Add(new Label{Text=$"CTRL + ALT + {i+1}",Width=145,Height=35,ForeColor=Palette.Muted,Font=Palette.Font(8,FontStyle.Bold),TextAlign=ContentAlignment.MiddleLeft});var box=Combo(250);FillLooks(box,data.QuickLooks[i]);box.SelectedIndexChanged+=(_,__)=>{if(box.SelectedItem is string n){data.QuickLooks[index]=n;Mark();}};quickBoxes[i]=box;r.Controls.Add(box);p.Controls.Add(r);}
        p.Controls.Add(Label("Some shortcuts may already be in use by Windows or other apps.",40,8.5f));
    }
    void SaveLook(){using var d=new Form{Text="Save look",ClientSize=new Size(370,135),StartPosition=FormStartPosition.CenterParent,FormBorderStyle=FormBorderStyle.FixedDialog,MaximizeBox=false,MinimizeBox=false,BackColor=Palette.Card,ForeColor=Palette.Text};d.Controls.Add(new Label{Text="Name your look",Left=16,Top=12,Width=320,Height=25});var input=new TextBox{Left=16,Top=40,Width=335,MaxLength=45,BackColor=Palette.Card2,ForeColor=Palette.Text,BorderStyle=BorderStyle.FixedSingle};var ok=MakeButton("Save",95,30);ok.Location=new Point(256,80);ok.DialogResult=DialogResult.OK;d.Controls.AddRange(new Control[]{input,ok});d.AcceptButton=ok;if(d.ShowDialog()!=DialogResult.OK)return;string name=input.Text.Trim();if(name.Length==0||Builtins().ContainsKey(name)){MessageBox.Show("Choose a non-empty name different from the built-in looks.");return;}if(looks.ContainsKey(name)&&MessageBox.Show("Replace this look?","Save look",MessageBoxButtons.YesNo)!=DialogResult.Yes)return;looks[name]=Current();Mark();SaveSettings();ShowPage("LOOKS");FillLooks(looksBox!,name);}
    void DeleteLook(){if(looksBox?.SelectedItem is not string name||Builtins().ContainsKey(name)){MessageBox.Show("Select a custom look to delete.");return;}if(MessageBox.Show("Delete '"+name+"'? Game profiles using it will no longer apply.","Delete look",MessageBoxButtons.YesNo)!=DialogResult.Yes)return;looks.Remove(name);Mark();SaveSettings();ShowPage("LOOKS");}
    void CrossPage(FlowLayoutPanel p){AddSection(p,"OVERLAY","The overlay is click-through and stays centred on the chosen monitor.");crossBox=Check("Show crosshair overlay",data.CrosshairEnabled);crossBox.CheckedChanged+=(_,__)=>{data.CrosshairEnabled=crossBox.Checked;UpdateCross();Mark();};p.Controls.Add(crossBox);
        var r=Row();shapeBox=Combo(170);shapeBox.Items.AddRange(new object[]{"Cross","Plus","Dot","Circle","T-shape","X-shape","Diamond","Chevron"});shapeBox.SelectedItem=data.CrossShape;if(shapeBox.SelectedIndex<0)shapeBox.SelectedIndex=0;shapeBox.SelectedIndexChanged+=(_,__)=>{data.CrossShape=(string)shapeBox.SelectedItem!;UpdateCross();Mark();};colorBox=Combo(170);colorBox.Items.AddRange(new object[]{"Lime","White","Cyan","Red","Yellow","Magenta","Custom"});colorBox.SelectedItem=string.IsNullOrEmpty(data.CrossColorHex)?data.CrossColor:"Custom";if(colorBox.SelectedIndex<0)colorBox.SelectedIndex=0;colorBox.SelectedIndexChanged+=(_,__)=>{if(colorBox.SelectedItem is not string chosen || chosen=="Custom")return;data.CrossColor=(string)colorBox.SelectedItem!;data.CrossColorHex="";UpdateCross();Mark();};r.Controls.Add(shapeBox);r.Controls.Add(colorBox);p.Controls.Add(r);
        var pick=MakeButton("◉   Choose custom colour",230);pick.Click+=(_,__)=>{using var dialog=new ColourWheelDialog(Crosshair.ResolveColor(data));if(dialog.ShowDialog(this)==DialogResult.OK){var c=dialog.SelectedColour;data.CrossColorHex=$"#{c.R:X2}{c.G:X2}{c.B:X2}";colorBox.SelectedItem="Custom";UpdateCross();Mark();}};p.Controls.Add(pick);p.Controls.Add(Label("Pick any colour from the wheel or enter a hex code.",38,8.5f));
        crossSize=CrossSlider(p,"Size",3,35,data.CrossSize,v=>data.CrossSize=v);crossGap=CrossSlider(p,"Gap",0,20,data.CrossGap,v=>data.CrossGap=v);crossThickness=CrossSlider(p,"Thickness",1,8,data.CrossThickness,v=>data.CrossThickness=v);
        p.Controls.Add(Label("POSITION & ROTATION",26,9,Palette.Violet,FontStyle.Bold));
        CrossSlider(p,"Horizontal offset (px)",-200,200,data.CrossOffsetX,v=>data.CrossOffsetX=v);CrossSlider(p,"Vertical offset (px)",-200,200,data.CrossOffsetY,v=>data.CrossOffsetY=v);CrossSlider(p,"Base angle (degrees)",-180,180,data.CrossAngle,v=>data.CrossAngle=v);
        p.Controls.Add(Label("MOTION",26,9,Palette.Violet,FontStyle.Bold));
        var spin=Check("Rotate the crosshair",data.CrossSpin);spin.CheckedChanged+=(_,__)=>{data.CrossSpin=spin.Checked;UpdateCross();Mark();};p.Controls.Add(spin);
        CrossSlider(p,"Rotation speed (degrees / sec)",-360,360,data.CrossSpinSpeed,v=>data.CrossSpinSpeed=v);
        p.Controls.Add(Label("Negative speed rotates the other way. Circles and dots do not visibly rotate.",38,8.5f));
        var animation=Combo(270);animation.Items.AddRange(new object[]{"None","Pulse","Breathe","Rainbow"});animation.SelectedItem=data.CrossAnimation;if(animation.SelectedIndex<0)animation.SelectedIndex=0;animation.SelectedIndexChanged+=(_,__)=>{data.CrossAnimation=(string)animation.SelectedItem!;UpdateCross();Mark();};p.Controls.Add(animation);
        CrossSlider(p,"Animation speed",25,300,data.CrossAnimationSpeed,v=>data.CrossAnimationSpeed=v);p.Controls.Add(Label("Rainbow temporarily overrides the selected colour.",32,8.5f));
        outlineBox=Check("Black outline",data.CrossOutline);outlineBox.CheckedChanged+=(_,__)=>{data.CrossOutline=outlineBox.Checked;UpdateCross();Mark();};p.Controls.Add(outlineBox);dotBox=Check("Centre dot",data.CrossDot);dotBox.CheckedChanged+=(_,__)=>{data.CrossDot=dotBox.Checked;UpdateCross();Mark();};p.Controls.Add(dotBox);
        p.Controls.Add(Label("MONITOR",28,9,Palette.Violet,FontStyle.Bold));monitorBox=Combo(300);var screens=Screen.AllScreens;for(int i=0;i<screens.Length;i++)monitorBox.Items.Add($"Display {i+1}  •  {screens[i].Bounds.Width} × {screens[i].Bounds.Height}"+(screens[i].Primary?"  •  Primary":""));monitorBox.SelectedIndex=Math.Clamp(data.Monitor,0,Math.Max(0,screens.Length-1));monitorBox.SelectedIndexChanged+=(_,__)=>{data.Monitor=monitorBox.SelectedIndex;UpdateCross();Mark();};p.Controls.Add(monitorBox);p.Controls.Add(Label("Some games or tournaments prohibit crosshair overlays. Check their rules.",45,8.5f));var reset=MakeButton("Reset crosshair",180);reset.Click+=(_,__)=>{data.CrossShape="Cross";data.CrossColor="Lime";data.CrossColorHex="";data.CrossSize=12;data.CrossGap=4;data.CrossThickness=2;data.CrossOffsetX=data.CrossOffsetY=data.CrossAngle=0;data.CrossSpin=false;data.CrossSpinSpeed=90;data.CrossAnimation="None";data.CrossAnimationSpeed=100;data.CrossOutline=true;data.CrossDot=false;UpdateCross();Mark();ShowPage("CROSSHAIR");};p.Controls.Add(reset);
    }
    SmoothSlider CrossSlider(FlowLayoutPanel p,string caption,int min,int max,int value,Action<int> update){var s=new SmoothSlider{Caption=caption,Min=min,Max=max,Value=Math.Clamp(value,min,max),Width=620};s.Changed+=(_,__)=>{update(s.Value);UpdateCross();Mark();};p.Controls.Add(s);return s;}
    void GamesPage(FlowLayoutPanel p){
        AddSection(p,"GAME PROFILES","Link a game executable to a saved look; Prism never reads game memory.");
        autoBox=Check("Enable automatic game profiles",data.AutoGames);autoBox.CheckedChanged+=(_,__)=>{data.AutoGames=autoBox.Checked;if(!data.AutoGames)RestoreManual();else CheckGame();Mark();};p.Controls.Add(autoBox);
        var matchRow=Row(45);matchRow.Controls.Add(new Label{Text="Match when",Width=100,Height=35,TextAlign=ContentAlignment.MiddleLeft,ForeColor=Palette.Text});var matching=Combo(250);matching.Items.AddRange(new object[]{"Focused window","Process running"});matching.SelectedItem=data.GameDetection;if(matching.SelectedIndex<0)matching.SelectedIndex=0;matching.SelectedIndexChanged+=(_,__)=>{RestoreManual();data.GameDetection=(string)matching.SelectedItem!;nextProcessPoll=0;CheckGame();Mark();};matchRow.Controls.Add(matching);p.Controls.Add(matchRow);
        gameStatus=Label("Looking for the active window…",30,8.5f,Palette.Muted);p.Controls.Add(gameStatus);
        p.Controls.Add(Label("GAME EXECUTABLE  /  CHOOSE A LOOK",25,8,Palette.Violet,FontStyle.Bold));
        var row=Row();processBox=new TextBox{Width=230,Height=33,Font=Palette.Font(10),BackColor=Palette.Card2,ForeColor=Palette.Text,BorderStyle=BorderStyle.FixedSingle,Margin=new Padding(0,7,8,8)};gameLook=Combo(220);FillLooks(gameLook);var add=MakeButton("Add / update",140);add.Click+=(_,__)=>AddGame();row.Controls.AddRange(new Control[]{processBox,gameLook,add});p.Controls.Add(row);
        var help=Row(48);var browse=MakeButton("Browse for .exe",170);browse.Click+=(_,__)=>{using var dialog=new OpenFileDialog{Filter="Windows programs (*.exe)|*.exe",Title="Select the game's actual executable"};if(dialog.ShowDialog(this)==DialogResult.OK)processBox.Text=Path.GetFileName(dialog.FileName);};help.Controls.Add(browse);var detect=MakeButton("Detect focused game in 5s",235);help.Controls.Add(detect);p.Controls.Add(help);
        var runningRow=Row(46);var running=Combo(370);var refreshRunning=MakeButton("Refresh running",160);void PopulateRunning(){running.Items.Clear();running.Items.Add("Choose a running application…");var names=new SortedSet<string>(StringComparer.OrdinalIgnoreCase);try{foreach(var proc in Process.GetProcesses()){try{if(proc.MainWindowHandle!=IntPtr.Zero)names.Add(proc.ProcessName+".exe");}catch{}finally{proc.Dispose();}}}catch{}foreach(var name in names)running.Items.Add(name);if(running.Items.Count==1)running.Items.Add("No visible windows found");running.SelectedIndex=0;}running.SelectedIndexChanged+=(_,__)=>{if(running.SelectedItem is string n&&n.EndsWith(".exe",StringComparison.OrdinalIgnoreCase))processBox.Text=n;};refreshRunning.Click+=(_,__)=>PopulateRunning();runningRow.Controls.Add(running);runningRow.Controls.Add(refreshRunning);p.Controls.Add(runningRow);PopulateRunning();
        var captureNote=Label("Use detection, browse for the game, or type its process name.",35,8.5f);p.Controls.Add(captureNote);var captureTimer=new System.Windows.Forms.Timer{Interval=1000};int seconds=0;detect.Click+=(_,__)=>{seconds=5;captureNote.Text="Switch to your game now — detecting in 5 seconds.";captureTimer.Start();};captureTimer.Tick+=(_,__)=>{seconds--;if(seconds>0){captureNote.Text=$"Switch to your game now — detecting in {seconds} seconds.";return;}captureTimer.Stop();string? found=ForegroundProcess();if(found==null||found.Equals(Path.GetFileNameWithoutExtension(Environment.ProcessPath),StringComparison.OrdinalIgnoreCase)){captureNote.Text="No game detected. Switch to its window before the countdown ends.";return;}processBox.Text=found+".exe";captureNote.Text="Detected "+found+".exe — choose a look, then click Add / update.";};p.Disposed+=(_,__)=>captureTimer.Dispose();
        gamesList=new ListBox{Width=620,Height=150,BackColor=Palette.Card2,ForeColor=Palette.Text,BorderStyle=BorderStyle.None,Font=Palette.Font(10),IntegralHeight=false};p.Controls.Add(gamesList);RefreshGames();gamesList.SelectedIndexChanged+=(_,__)=>{if(gamesList.SelectedItem is not string selected)return;string key=selected.Split(".exe    →    ")[0];processBox.Text=key+".exe";if(data.Games.TryGetValue(key,out var selectedLook)&&gameLook.Items.Contains(selectedLook))gameLook.SelectedItem=selectedLook;};var remove=MakeButton("Remove selected",180);remove.Click+=(_,__)=>RemoveGame();p.Controls.Add(remove);
        p.Controls.Add(Label("Use the actual game process, not its launcher. Focus mode restores the previous look when you switch away. Running mode holds the look until the process exits; if several match, the focused game wins, otherwise the first name alphabetically wins.",92,8.5f));
        p.Controls.Add(Label("Colour applies to the whole desktop. Some games block overlays or colour tools; check their rules.",47,8.5f));CheckGame();
    }
    void AddGame(){string proc=(processBox?.Text??"").Trim();proc=Path.GetFileName(proc);if(proc.EndsWith(".exe",StringComparison.OrdinalIgnoreCase))proc=proc[..^4];if(string.IsNullOrWhiteSpace(proc)||proc.IndexOfAny(Path.GetInvalidFileNameChars())>=0||gameLook?.SelectedItem is not string look){MessageBox.Show("Enter an executable name and choose a look.");return;}data.Games[proc.ToLowerInvariant()]=look;processBox!.Clear();RefreshGames();Mark();}
    void RefreshGames(){if(gamesList==null)return;gamesList.Items.Clear();foreach(var item in data.Games.OrderBy(x=>x.Key))gamesList.Items.Add(item.Key+".exe    →    "+item.Value);}
    void RemoveGame(){if(gamesList?.SelectedItem is not string selected)return;string proc=selected.Split(".exe    →    ")[0];data.Games.Remove(proc);RefreshGames();Mark();}
    void ResolutionPage(FlowLayoutPanel p){
        AddSection(p,"DISPLAY MODES","Choose a mode reported by Windows for the selected display.");
        var r=Row(46);var displays=Combo(280);var screens=Screen.AllScreens;for(int i=0;i<screens.Length;i++)displays.Items.Add($"Display {i+1}  •  {screens[i].DeviceName}"+(screens[i].Primary?"  •  Primary":""));displays.SelectedIndex=Math.Clamp(data.Monitor,0,Math.Max(0,screens.Length-1));var ratio=Combo(200);ratio.Items.AddRange(new object[]{"All aspect ratios","4:3","16:9","16:10","5:4","21:9"});ratio.SelectedIndex=0;r.Controls.Add(displays);r.Controls.Add(ratio);p.Controls.Add(r);
        var current=Label("",35,9,Palette.Muted);p.Controls.Add(current);var modes=new ListBox{Width=620,Height=215,BackColor=Palette.Card2,ForeColor=Palette.Text,BorderStyle=BorderStyle.None,Font=Palette.Font(10),IntegralHeight=false};p.Controls.Add(modes);
        void RefreshModes(){modes.Items.Clear();if(displays.SelectedIndex<0)return;string device=screens[displays.SelectedIndex].DeviceName;var now=DisplayController.Current(device);current.Text=now.HasValue?$"CURRENT   {now.Value.dmPelsWidth} × {now.Value.dmPelsHeight}  •  {now.Value.dmDisplayFrequency} Hz":"Current mode unavailable";string filter=ratio.SelectedItem?.ToString()??"All aspect ratios";foreach(var m in DisplayController.Modes(device).Where(x=>DisplayController.Matches(x,filter)))modes.Items.Add(m);if(modes.Items.Count>0)modes.SelectedIndex=0;else modes.Items.Add("No supported modes for this aspect ratio.");}
        displays.SelectedIndexChanged+=(_,__)=>RefreshModes();ratio.SelectedIndexChanged+=(_,__)=>RefreshModes();RefreshModes();
        var apply=MakeButton("Apply selected mode",210);apply.Click+=(_,__)=>{if(displays.SelectedIndex<0||modes.SelectedItem is not DisplayMode selected){MessageBox.Show("Select a supported display mode first.");return;}string device=screens[displays.SelectedIndex].DeviceName;DisplayController.TryChange(this,folder,device,selected);lastGammaRequest=int.MinValue;Apply();if(data.CrosshairEnabled)UpdateCross();RefreshModes();};p.Controls.Add(apply);
        p.Controls.Add(Label("HOW STRETCHED 4:3 WORKS",26,9,Palette.Violet,FontStyle.Bold));
        p.Controls.Add(Label("Pick a supported 4:3 mode above. To fill the screen, set NVIDIA Control Panel → Adjust desktop size and position → Scaling → Full-screen, then choose GPU or display scaling and Apply. Your monitor, GPU and game must support the mode.",88,9));
        var nvidia=MakeButton("Open NVIDIA Control Panel",235);nvidia.Click+=(_,__)=>{try{string exe=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),"NVIDIA Corporation","Control Panel Client","nvcplui.exe");if(!File.Exists(exe)){MessageBox.Show("NVIDIA Control Panel was not found in its standard location. Open it from the Windows Start menu, or install it from Microsoft Store.");return;}Process.Start(new ProcessStartInfo(exe){UseShellExecute=true});}catch(Exception ex){MessageBox.Show("Could not open NVIDIA Control Panel: "+ex.Message);}};p.Controls.Add(nvidia);
        p.Controls.Add(Label("Prism changes the Windows display mode only. It does not set NVIDIA GPU scaling, add unsupported custom resolutions, or force a game to use a mode. A timed prompt and separate recovery process revert an unconfirmed change.",85,8.5f));
    }
    SmoothSlider SettingSlider(FlowLayoutPanel p,string caption,int min,int max,int value,string unit,Action<int> update){var slider=new SmoothSlider{Caption=caption,Min=min,Max=max,Value=Math.Clamp(value,min,max),Unit=unit,Width=620};slider.Changed+=(_,__)=>{update(slider.Value);Apply();Mark();};p.Controls.Add(slider);return slider;}
    void DarkPage(FlowLayoutPanel p){
        AddSection(p,"DARK BOOST","Amplifies existing dark detail. Black areas with no detail cannot be recovered.");
        p.Controls.Add(Label("Game rules vary. Filters that reveal dark areas may lead to penalties or bans.",40,9,Color.FromArgb(255,145,135),FontStyle.Bold));
        var enabled=Check("Enable Dark Boost shortcut",data.DarkEnabled);enabled.CheckedChanged+=(_,__)=>{data.DarkEnabled=enabled.Checked;if(!enabled.Checked)darkToggle=false;Mark();};p.Controls.Add(enabled);
        p.Controls.Add(Label("COLOUR",24,8,Palette.Violet,FontStyle.Bold));var colour=Combo(240);colour.Items.AddRange(new object[]{"Green","Clean","Amber","Cyan","White"});colour.SelectedItem=data.DarkColour;if(colour.SelectedIndex<0)colour.SelectedIndex=0;colour.SelectedIndexChanged+=(_,__)=>{data.DarkColour=(string)colour.SelectedItem!;Apply();Mark();};p.Controls.Add(colour);
        SettingSlider(p,"Strength",0,100,data.DarkStrength,"%",v=>data.DarkStrength=v);SettingSlider(p,"Black level",0,100,data.DarkBlackLevel,"%",v=>data.DarkBlackLevel=v);
        p.Controls.Add(Label("Higher black level suppresses more dark pixels; high strength may wash out or clip highlights.",38,8.5f));
        var keep=Check("Keep some original colours",data.DarkKeepColours);keep.CheckedChanged+=(_,__)=>{data.DarkKeepColours=keep.Checked;Apply();Mark();};p.Controls.Add(keep);
        var modeRow=Row();modeRow.Controls.Add(new Label{Text="Activation",Width=100,Height=35,TextAlign=ContentAlignment.MiddleLeft,ForeColor=Palette.Text});var mode=Combo(145);mode.Items.AddRange(new object[]{"Hold","Toggle"});mode.SelectedItem=data.DarkMode;if(mode.SelectedIndex<0)mode.SelectedIndex=0;mode.SelectedIndexChanged+=(_,__)=>{data.DarkMode=(string)mode.SelectedItem!;darkToggle=false;lastDarkKey=false;Mark();};modeRow.Controls.Add(mode);var key=MakeButton("Key: "+KeyName(data.DarkKey,data.DarkModifiers),270);key.Click+=(_,__)=>{using var d=new ShortcutDialog("Dark Boost key");if(d.ShowDialog(this)==DialogResult.OK){data.DarkKey=d.KeyCode;data.DarkModifiers=d.Modifiers;key.Text="Key: "+KeyName(data.DarkKey,data.DarkModifiers);lastDarkKey=false;Mark();}};modeRow.Controls.Add(key);p.Controls.Add(modeRow);
        var only=Check("Only while a mapped game is running",data.DarkOnlyGames);only.CheckedChanged+=(_,__)=>{data.DarkOnlyGames=only.Checked;nextDarkProcessPoll=0;darkToggle=false;Mark();};p.Controls.Add(only);var fade=Check("Smooth fade",data.DarkFade);fade.CheckedChanged+=(_,__)=>{data.DarkFade=fade.Checked;Mark();};p.Controls.Add(fade);
        p.Controls.Add(Label("QUICK SETUPS",24,8,Palette.Violet,FontStyle.Bold));var row=Row();foreach(string preset in new[]{"Green night","Clean lift","Amber","Soft"}){var button=MakeButton(preset,140);button.Click+=(_,__)=>{(data.DarkColour,data.DarkStrength,data.DarkBlackLevel,data.DarkKeepColours)=preset switch{"Green night"=>("Green",70,5,false),"Clean lift"=>("Clean",55,0,true),"Amber"=>("Amber",60,5,false),_=>("White",30,0,true)};Mark();Apply();ShowPage("DARK BOOST");};row.Controls.Add(button);}p.Controls.Add(row);
        p.Controls.Add(Label("These are linear display transforms, not adaptive night vision. Availability varies with HDR, exclusive fullscreen and your driver.",55,8.5f));
    }
    void VisionPage(FlowLayoutPanel p){
        AddSection(p,"COLOUR VISION","Optional approximate channel separation. Results vary; these are not medically validated corrections.");
        var vision=Combo(240);vision.Items.AddRange(new object[]{"Normal","Red-weak","Green-weak","Blue-weak"});vision.SelectedItem=data.VisionMode;if(vision.SelectedIndex<0)vision.SelectedIndex=0;vision.SelectedIndexChanged+=(_,__)=>{data.VisionMode=(string)vision.SelectedItem!;Apply();Mark();};p.Controls.Add(vision);
        AddSection(p,"EYE CARE","Applies on top of the current look while colour effects are enabled.");SettingSlider(p,"Eye care warmth",0,100,data.EyeWarmth,"%",v=>data.EyeWarmth=v);SettingSlider(p,"Screen dim",0,80,data.EyeDim,"%",v=>data.EyeDim=v);
        p.Controls.Add(Label("Eye care is an optional display adjustment, not medical advice. Turning the effect off restores the previous filter.",55,8.5f));
    }
    static bool IsMouseKey(int key)=>key is 1 or 2 or 4 or 5 or 6;
    static string KeyName(int key,int mods){if(key==0)return "Not set";string prefix=((mods&2)!=0?"Ctrl + ":"")+((mods&1)!=0?"Alt + ":"")+((mods&4)!=0?"Shift + ":"");string name=key switch{1=>"Mouse left",2=>"Mouse right",4=>"Mouse middle",5=>"Mouse side 1",6=>"Mouse side 2",_=>((Keys)key).ToString()};return prefix+name;}
    static bool BindingDown(int key,int mods)=>key!=0&&Native.Down(key)&&((mods&2)==0||Native.Down(0x11))&&((mods&1)==0||Native.Down(0x12))&&((mods&4)==0||Native.Down(0x10));
    void MediaPage(FlowLayoutPanel p){
        AddSection(p,"MEDIA CONTROLS","Send Windows media commands to the active player. Track information is not shown.");var row=Row();var mini=MakeButton("Open mini controls",185);mini.Click+=(_,__)=>ShowMediaPopup();row.Controls.Add(mini);foreach(var pair in new[]{("Previous",(byte)0xB1),("Play / pause",(byte)0xB3),("Next",(byte)0xB0)}){var b=MakeButton(pair.Item1,125);b.Click+=(_,__)=>SendMedia(pair.Item2);row.Controls.Add(b);}p.Controls.Add(row);
        var enabled=Check("Enable optional media shortcuts",data.MediaShortcuts);enabled.CheckedChanged+=(_,__)=>{data.MediaShortcuts=enabled.Checked;RegisterShortcuts();Mark();};p.Controls.Add(enabled);
        void Bind(string label,Func<(int,int)> get,Action<int,int> set){var r=Row();r.Controls.Add(new Label{Text=label,Width=160,Height=35,TextAlign=ContentAlignment.MiddleLeft,ForeColor=Palette.Text});var (key,mods)=get();var setButton=MakeButton(KeyName(key,mods),220);setButton.Click+=(_,__)=>{using var d=new ShortcutDialog(label);if(d.ShowDialog(this)==DialogResult.OK){if(d.KeyCode>=0xB0&&d.KeyCode<=0xB3){MessageBox.Show("Choose a different key; media keys cannot trigger other media keys.");return;}set(d.KeyCode,d.Modifiers);setButton.Text=KeyName(d.KeyCode,d.Modifiers);if(data.MediaShortcuts)RegisterShortcuts();Mark();}};r.Controls.Add(setButton);var clear=MakeButton("Clear",80);clear.Click+=(_,__)=>{set(0,0);setButton.Text="Not set";if(data.MediaShortcuts)RegisterShortcuts();Mark();};r.Controls.Add(clear);p.Controls.Add(r);}
        Bind("Open mini controls",()=> (data.MediaPopupKey,data.MediaPopupModifiers),(k,m)=>{data.MediaPopupKey=k;data.MediaPopupModifiers=m;});Bind("Play / pause",()=> (data.MediaPlayKey,data.MediaPlayModifiers),(k,m)=>{data.MediaPlayKey=k;data.MediaPlayModifiers=m;});Bind("Next track",()=> (data.MediaNextKey,data.MediaNextModifiers),(k,m)=>{data.MediaNextKey=k;data.MediaNextModifiers=m;});Bind("Previous track",()=> (data.MediaPrevKey,data.MediaPrevModifiers),(k,m)=>{data.MediaPrevKey=k;data.MediaPrevModifiers=m;});
        p.Controls.Add(Label("LOUDNESS BALANCE  /  EXPERIMENTAL",26,9,Palette.Violet,FontStyle.Bold));
        p.Controls.Add(Label("Makes quiet sounds more noticeable relative to loud ones by automatically lowering the Windows output volume when audio peaks. It affects every sound, including voice chat and music; it cannot identify footsteps.",80,9));
        audioToggle=Check("Enable system-wide loudness balance",ducker!=null);audioToggle.CheckedChanged+=(_,__)=>{if(syncingAudio)return;if(audioToggle.Checked){if(!StartAudio()){syncingAudio=true;audioToggle.Checked=false;syncingAudio=false;}}else StopAudio();};p.Controls.Add(audioToggle);
        var threshold=new SmoothSlider{Caption="Hold sounds above",Min=10,Max=100,Value=Math.Clamp(data.LoudnessThreshold,10,100),Unit="%",Width=620};threshold.Changed+=(_,__)=>{data.LoudnessThreshold=threshold.Value;Mark();};p.Controls.Add(threshold);
        audioInfo=Label(ducker==null?"Off · check Windows volume after use":"On · monitoring the default playback device",36,8.5f,Palette.Muted);p.Controls.Add(audioInfo);
        p.Controls.Add(Label("This experimental peak-based volume ducking is not a true sample-level compressor or hearing protection. It may pump, react late, or miss brief sounds. It never intentionally raises the output above your baseline volume.",78,8.5f));
        var sound=MakeButton("Open Windows sound settings",245);sound.Click+=(_,__)=>{try{Process.Start(new ProcessStartInfo("ms-settings:sound"){UseShellExecute=true});}catch(Exception ex){MessageBox.Show("Could not open sound settings: "+ex.Message);}};p.Controls.Add(sound);
    }
    bool StartAudio(){
        if(MessageBox.Show("Prism will monitor the peak level and temporarily change the Windows master volume of the current playback device. This affects games, music and voice chat. It is not hearing protection and cannot isolate footsteps. When you turn it off, it may restore the previous volume. Start at a safe volume.\n\nEnable it?","Loudness balance",MessageBoxButtons.YesNo,MessageBoxIcon.Warning)!=DialogResult.Yes)return false;
        try{ducker=new AudioDucker();audioTimer.Start();if(audioInfo!=null&&!audioInfo.IsDisposed)audioInfo.Text="On · "+ducker.DeviceName;return true;}catch(Exception ex){ducker?.Dispose();ducker=null;MessageBox.Show("Could not start loudness balance: "+ex.Message,"Prism",MessageBoxButtons.OK,MessageBoxIcon.Warning);return false;}
    }
    void AudioTick(){if(ducker==null)return;try{ducker.Tick(data.LoudnessThreshold);if(++audioInfoCounter>=5){audioInfoCounter=0;if(audioInfo!=null&&!audioInfo.IsDisposed)audioInfo.Text=$"On · peak {ducker.Peak*100:0}% · output {ducker.Current*100:0}%";}}catch(Exception ex){string message=ex.Message;StopAudio();if(audioInfo!=null&&!audioInfo.IsDisposed)audioInfo.Text="Stopped · "+message;if(status!=null)status.Text="Audio balance stopped · "+message;tray?.ShowBalloonTip(3000,"Prism audio stopped",message,ToolTipIcon.Warning);}}
    void StopAudio(){audioTimer.Stop();var old=ducker;ducker=null;try{old?.Dispose();}catch{}if(audioToggle!=null&&!audioToggle.IsDisposed){syncingAudio=true;audioToggle.Checked=false;syncingAudio=false;}if(audioInfo!=null&&!audioInfo.IsDisposed)audioInfo.Text="Off · check Windows volume after use";}
    void RecordingPage(FlowLayoutPanel p){
        AddSection(p,"CAPTURE COMPATIBILITY","Your display filter may not appear in recordings or screen sharing.");p.Controls.Add(Label("Windows Magnification can be applied after some capture paths. OBS Display Capture, Game Capture, Discord and screenshots can produce different results; this app cannot force the effect into every recording.",72,9));
        p.Controls.Add(Label("MANUAL CAPTURE TEST",26,9,Palette.Violet,FontStyle.Bold));p.Controls.Add(Label("Save a primary-display screenshot and compare it with what you see. This captures your visible screen locally; it does not upload anything. It tests Windows screen capture only, not OBS or Discord.",75,9));var test=MakeButton("Save test screenshot…",215);test.Click+=(_,__)=>{using var dialog=new SaveFileDialog{Filter="PNG image (*.png)|*.png",FileName="Prism-capture-test.png",Title="Save screenshot of the primary display"};if(dialog.ShowDialog(this)!=DialogResult.OK)return;try{var b=Screen.PrimaryScreen!.Bounds;using var image=new Bitmap(b.Width,b.Height);using(var g=Graphics.FromImage(image))g.CopyFromScreen(b.Location,Point.Empty,b.Size);image.Save(dialog.FileName,System.Drawing.Imaging.ImageFormat.Png);MessageBox.Show("Screenshot saved. Compare it with your screen; this does not test OBS or Discord.","Capture test");}catch(Exception ex){MessageBox.Show("Could not save screenshot: "+ex.Message);}};p.Controls.Add(test);
        p.Controls.Add(Label("For OBS, make a short local recording using your actual capture source and inspect it. Gamma and display-driver colour changes may not appear in captures.",65,8.5f));
    }
    void DiagnosticsPage(FlowLayoutPanel p){
        AddSection(p,"SYSTEM STATUS","A local snapshot to help troubleshoot. It is not sent anywhere.");
        var box=new TextBox{Multiline=true,ReadOnly=true,ScrollBars=ScrollBars.Vertical,Width=620,Height=310,BorderStyle=BorderStyle.FixedSingle,BackColor=Palette.Card2,ForeColor=Palette.Text,Font=Palette.Font(9)};p.Controls.Add(box);
        string Snapshot(){var lines=new List<string>{"Prism 8",RuntimeInformation.OSDescription,"Colour effect: "+(data.EffectEnabled?"enabled":"off"),"Magnification API: "+(initialized?"initialised":"unavailable"),"Gamma: "+(gammaError??(Current().Gamma+"%")),"Crosshair: "+(data.CrosshairEnabled?"on":"off"),"Animation: "+(data.CrossSpin?"rotation on; ":"")+data.CrossAnimation,"Game profiles: "+(data.AutoGames?data.GameDetection:"off"),"Focused process: "+(ForegroundProcess()??"unavailable"),"Matched game: "+(autoGame??"none"),"Displays:"};foreach(var screen in Screen.AllScreens){var m=DisplayController.Current(screen.DeviceName);lines.Add("  "+screen.DeviceName+"  "+screen.Bounds.Width+" × "+screen.Bounds.Height+(m.HasValue?"  "+m.Value.dmDisplayFrequency+" Hz":"")+(screen.Primary?"  primary":""));}return string.Join(Environment.NewLine,lines);}
        box.Text=Snapshot();var row=Row();var refresh=MakeButton("Refresh",160);refresh.Click+=(_,__)=>box.Text=Snapshot();var copy=MakeButton("Copy report",160);copy.Click+=(_,__)=>{try{Clipboard.SetText(box.Text);}catch(Exception ex){MessageBox.Show("Could not copy: "+ex.Message);}};row.Controls.AddRange(new Control[]{refresh,copy});p.Controls.Add(row);p.Controls.Add(Label("The report includes monitor identifiers and the name of the currently focused process. Review it before sharing.",55,8.5f));
        p.Controls.Add(Label("VERIFY THIS FILE",26,9,Palette.Violet,FontStyle.Bold));var hashBox=new TextBox{ReadOnly=true,Width=620,BackColor=Palette.Card2,ForeColor=Palette.Text,BorderStyle=BorderStyle.FixedSingle,Font=Palette.Font(9)};p.Controls.Add(hashBox);var hashRow=Row();var calculate=MakeButton("Calculate SHA-256",190);calculate.Click+=async (_,__)=>{calculate.Enabled=false;try{string path=Environment.ProcessPath??throw new InvalidOperationException("Executable path unavailable.");string value=await Task.Run(()=>{using var stream=File.OpenRead(path);return Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(stream));});if(!hashBox.IsDisposed)hashBox.Text=value;}catch(Exception ex){MessageBox.Show("Could not calculate hash: "+ex.Message);}finally{if(!calculate.IsDisposed)calculate.Enabled=true;}};var copyHash=MakeButton("Copy hash",140);copyHash.Click+=(_,__)=>{if(!string.IsNullOrEmpty(hashBox.Text))try{Clipboard.SetText(hashBox.Text);}catch(Exception ex){MessageBox.Show("Could not copy: "+ex.Message);}};hashRow.Controls.AddRange(new Control[]{calculate,copyHash});p.Controls.Add(hashRow);p.Controls.Add(Label("A hash identifies this executable; it does not establish that it is safe. No file is uploaded.",50,8.5f));
    }
    void SettingsPage(FlowLayoutPanel p){
        AddSection(p,"BACKGROUND & STARTUP","Keep Prism running in the notification area while you play.");
        var toTray=Check("Minimise and close to the system tray",data.CloseToTray);toTray.CheckedChanged+=(_,__)=>{data.CloseToTray=toTray.Checked;Mark();};p.Controls.Add(toTray);
        p.Controls.Add(Label("When enabled, closing the window hides it. Right-click the Prism tray icon and choose Exit to stop effects and quit completely.",58,9));
        var startup=Check("Start Prism in the tray when I sign in",StartupEnabled());bool changing=false;startup.CheckedChanged+=(_,__)=>{if(changing)return;try{SetStartup(startup.Checked);}catch(Exception ex){changing=true;startup.Checked=!startup.Checked;changing=false;MessageBox.Show("Could not change startup settings: "+ex.Message,"Prism",MessageBoxButtons.OK,MessageBoxIcon.Warning);}};p.Controls.Add(startup);
        p.Controls.Add(Label("Optional. Uses the current Windows user's startup registry entry. If you move the app, turn this off and on again.",55,8.5f));
        p.Controls.Add(Label("CUSTOM SHORTCUTS",26,9,Palette.Violet,FontStyle.Bold));var shortcutEnable=Check("Enable global utility shortcuts",data.UtilityShortcuts);shortcutEnable.CheckedChanged+=(_,__)=>{data.UtilityShortcuts=shortcutEnable.Checked;RegisterShortcuts();Mark();};p.Controls.Add(shortcutEnable);
        var openRow=Row();openRow.Controls.Add(new Label{Text="Open Prism",Width=150,Height=35,TextAlign=ContentAlignment.MiddleLeft,ForeColor=Palette.Text});var openKey=MakeButton(KeyName(data.WindowKey,data.WindowModifiers),240);openKey.Click+=(_,__)=>{using var d=new ShortcutDialog("Open Prism");if(d.ShowDialog(this)==DialogResult.OK){data.WindowKey=d.KeyCode;data.WindowModifiers=d.Modifiers;openKey.Text=KeyName(data.WindowKey,data.WindowModifiers);if(data.UtilityShortcuts)RegisterShortcuts();Mark();}};openRow.Controls.Add(openKey);p.Controls.Add(openRow);
        var quickRow=Row();quickRow.Controls.Add(new Label{Text="Quick colour",Width=150,Height=35,TextAlign=ContentAlignment.MiddleLeft,ForeColor=Palette.Text});var quickKey=MakeButton(KeyName(data.QuickKey,data.QuickModifiers),240);quickKey.Click+=(_,__)=>{using var d=new ShortcutDialog("Quick colour");if(d.ShowDialog(this)==DialogResult.OK){data.QuickKey=d.KeyCode;data.QuickModifiers=d.Modifiers;quickKey.Text=KeyName(data.QuickKey,data.QuickModifiers);if(data.UtilityShortcuts)RegisterShortcuts();Mark();}};quickRow.Controls.Add(quickKey);p.Controls.Add(quickRow);var quickOpen=MakeButton("Open quick colour",190);quickOpen.Click+=(_,__)=>ShowQuickPopup();p.Controls.Add(quickOpen);
        var crossRow=Row();crossRow.Controls.Add(new Label{Text="Toggle crosshair",Width=150,Height=35,TextAlign=ContentAlignment.MiddleLeft,ForeColor=Palette.Text});var crossKey=MakeButton(KeyName(data.CrossKey,data.CrossModifiers),240);crossKey.Click+=(_,__)=>{using var d=new ShortcutDialog("Toggle crosshair");if(d.ShowDialog(this)==DialogResult.OK){data.CrossKey=d.KeyCode;data.CrossModifiers=d.Modifiers;crossKey.Text=KeyName(data.CrossKey,data.CrossModifiers);if(data.UtilityShortcuts)RegisterShortcuts();Mark();}};crossRow.Controls.Add(crossKey);p.Controls.Add(crossRow);
        p.Controls.Add(Label("Click a shortcut to set it. Dark Boost's key is set on its own page. Windows and other apps may reserve a combination.",55,8.5f));
        p.Controls.Add(Label("SAFETY",26,9,Palette.Violet,FontStyle.Bold));p.Controls.Add(Label("Ctrl + Alt + F12 disables the filter and crosshair, even when Prism is in the tray (if Windows receives the shortcut).",54,9));
        var hide=MakeButton("Hide to tray",180);hide.Click+=(_,__)=>HideToTray();p.Controls.Add(hide);var quit=MakeButton("Exit Prism completely",220);quit.Click+=(_,__)=>{exiting=true;Close();};p.Controls.Add(quit);
    }
    const string StartupPath=@"Software\Microsoft\Windows\CurrentVersion\Run";
    bool StartupEnabled(){try{using var k=Microsoft.Win32.Registry.CurrentUser.OpenSubKey(StartupPath);return k?.GetValue("PrismDisplay") is string v && !string.IsNullOrWhiteSpace(v);}catch{return false;}}
    void SetStartup(bool enabled){using var k=Microsoft.Win32.Registry.CurrentUser.CreateSubKey(StartupPath,true)??throw new InvalidOperationException("Could not open the startup key.");if(enabled){string path=Environment.ProcessPath??throw new InvalidOperationException("Executable path unavailable.");k.SetValue("PrismDisplay","\""+path+"\" --start-minimized");}else k.DeleteValue("PrismDisplay",false);}
    void SetupTray(){
        using(var bitmap=new Bitmap(32,32)){using var g=Graphics.FromImage(bitmap);g.SmoothingMode=SmoothingMode.AntiAlias;g.Clear(Color.Transparent);using var bg=new SolidBrush(Color.FromArgb(25,25,29));g.FillEllipse(bg,0,0,32,32);using var front=new SolidBrush(Color.FromArgb(246,247,251));using var side=new SolidBrush(Color.FromArgb(164,174,200));using var edge=new Pen(Color.FromArgb(191,185,255),1.2f);g.FillPolygon(front,new[]{new PointF(16,5),new PointF(26,24),new PointF(16,19),new PointF(6,24)});g.FillPolygon(side,new[]{new PointF(16,5),new PointF(16,19),new PointF(26,24)});g.DrawLine(edge,6,24,26,24);IntPtr raw=bitmap.GetHicon();using var temp=Icon.FromHandle(raw);trayIcon=(Icon)temp.Clone();Native.DestroyIcon(raw);}
        trayMenu=new ContextMenuStrip();trayMenu.Items.Add("Open Prism",null,(_,__)=>ShowFromTray());trayMenu.Items.Add("Quick colour",null,(_,__)=>ShowQuickPopup());trayMenu.Items.Add("Media controls",null,(_,__)=>ShowMediaPopup());var toggleEffect=new ToolStripMenuItem("Colour effect"){CheckOnClick=false};toggleEffect.Click+=(_,__)=>{data.EffectEnabled=!data.EffectEnabled;if(!data.EffectEnabled){darkToggle=false;boostLevel=0;}UpdatePower();Apply();Mark();};trayMenu.Items.Add(toggleEffect);var toggleCross=new ToolStripMenuItem("Crosshair"){CheckOnClick=false};toggleCross.Click+=(_,__)=>{data.CrosshairEnabled=!data.CrosshairEnabled;UpdateCross();if(crossBox!=null&&!crossBox.IsDisposed)crossBox.Checked=data.CrosshairEnabled;Mark();};trayMenu.Items.Add(toggleCross);trayMenu.Items.Add(new ToolStripSeparator());trayMenu.Items.Add("Exit Prism",null,(_,__)=>{exiting=true;Close();});trayMenu.Opening+=(_,__)=>{toggleEffect.Checked=data.EffectEnabled;toggleCross.Checked=data.CrosshairEnabled;};tray=new NotifyIcon{Icon=trayIcon,Text="Prism",ContextMenuStrip=trayMenu,Visible=true};tray.DoubleClick+=(_,__)=>ShowFromTray();
    }
    void ShowFromTray(){Show();if(WindowState==FormWindowState.Minimized)WindowState=FormWindowState.Normal;Activate();BringToFront();}
    void HideToTray(){Hide();if(!notified&&tray!=null){notified=true;tray.BalloonTipTitle="Prism is still running";tray.BalloonTipText="Open Prism or exit it from the notification area.";tray.ShowBalloonTip(2500);}}
    void OnClosing(object? sender,FormClosingEventArgs e){if(!exiting&&e.CloseReason==CloseReason.UserClosing&&data.CloseToTray){e.Cancel=true;HideToTray();return;}Cleanup();}
    void AppearancePage(FlowLayoutPanel p){
        AddSection(p,"WORKSPACE THEME","Personalise the app interface. These settings do not change game colours.");
        p.Controls.Add(Label("THEME",25,9,Palette.Violet,FontStyle.Bold));
        var theme=Combo(300);theme.Items.AddRange(new object[]{"Carbon","Obsidian","Midnight","Slate","Aubergine"});theme.SelectedItem=data.Theme;if(theme.SelectedIndex<0)theme.SelectedIndex=0;theme.SelectedIndexChanged+=(_,__)=>{data.Theme=(string)theme.SelectedItem!;Mark();BeginInvoke(new Action(RebuildAppearance));};p.Controls.Add(theme);
        p.Controls.Add(Label("ACCENT COLOUR",25,9,Palette.Violet,FontStyle.Bold));
        var accent=Combo(300);accent.Items.AddRange(new object[]{"Ice","Violet","Cyan","Blue","Rose","Mint","Amber"});accent.SelectedItem=data.Accent;if(accent.SelectedIndex<0)accent.SelectedIndex=0;accent.SelectedIndexChanged+=(_,__)=>{data.Accent=(string)accent.SelectedItem!;Mark();BeginInvoke(new Action(RebuildAppearance));};p.Controls.Add(accent);
        p.Controls.Add(Label("Your theme and accent are saved on this PC.",48,9));
        p.Controls.Add(Label("PREVIEW",26,9,Palette.Violet,FontStyle.Bold));
        var preview=Row(50);var pill=MakeButton("●  Active control",190,36);pill.ForeColor=Palette.Violet;preview.Controls.Add(pill);p.Controls.Add(preview);
    }
    void ApplyDarkCaption(){if(IsHandleCreated)WindowChrome.Apply(this);}
    void ShowQuickPopup(){if(quickPopup==null||quickPopup.IsDisposed){quickPopup=new QuickColourPopup(Current().Saturation,v=>SetLook(Current() with {Saturation=v}),()=>{data.EffectEnabled=!data.EffectEnabled;if(!data.EffectEnabled){darkToggle=false;boostLevel=0;}UpdatePower();Apply();Mark();});quickPopup.FormClosed+=(_,__)=>quickPopup=null;quickPopup.Show();}else{quickPopup.SetSaturation(Current().Saturation);quickPopup.Show();quickPopup.Activate();}}
    static void SendMedia(byte key){Native.keybd_event(key,0,0,UIntPtr.Zero);Native.keybd_event(key,0,2,UIntPtr.Zero);}
    void ShowMediaPopup(){if(mediaPopup==null||mediaPopup.IsDisposed){mediaPopup=new MediaPopup(SendMedia);mediaPopup.FormClosed+=(_,__)=>mediaPopup=null;mediaPopup.Show();}else{mediaPopup.Show();mediaPopup.Activate();}}
    void RebuildAppearance(){if(IsDisposed)return;data.Current=Current();sliders.Clear();nav.Clear();foreach(Control c in Controls.Cast<Control>().ToArray())c.Dispose();Palette.Configure(data.Theme,data.Accent);BackColor=Palette.Bg;content=new Panel{Dock=DockStyle.Fill,BackColor=Palette.Bg};Build("APPEARANCE");ApplyDarkCaption();UpdatePower();Apply();}
    void UpdatePower(){if(power==null)return;power.Text=data.EffectEnabled?"●  EFFECT ON":"○  EFFECT OFF";power.BackColor=data.EffectEnabled?Color.FromArgb(44,61,58):Palette.Card2;power.ForeColor=data.EffectEnabled?Palette.Green:Palette.Muted;}
    void Apply(){if(!initialized)return;SyncGamma();if(!data.EffectEnabled){bool restored=Native.MagSetFullscreenColorEffect(ref previous);if(status!=null)status.Text=gammaError??(restored?"Effect off · previous colour matrix restored":"Could not restore previous colour matrix");return;}
        var effect=new Native.Effect{matrix=ColourEngine.Build(Current(),data,boostLevel)};bool ok=Native.MagSetFullscreenColorEffect(ref effect);if(status!=null)status.Text=gammaError!=null?gammaError:ok?(boostLevel>0?"●  Dark Boost active":autoGame!=null?"●  Game look active: "+autoGame:"●  Live colour active") : "Windows rejected the effect · check HDR or display configuration";if(!ok&&!failed){failed=true;MessageBox.Show("Windows rejected the colour effect. Try disabling HDR, leaving Remote Desktop, and using a borderless game.","Prism",MessageBoxButtons.OK,MessageBoxIcon.Warning);}}
    void SyncGamma(){int request=data.EffectEnabled?Current().Gamma:100;if(request==lastGammaRequest)return;lastGammaRequest=request;gammaError=gamma?.SetGamma(request);}
    void OnTick(){bool emergency=Native.Down(0x11)&&Native.Down(0x12)&&Native.Down(0x7B);if(emergency&&!emergencyLatch){data.EffectEnabled=false;data.CrosshairEnabled=false;darkToggle=false;boostLevel=0;StopAudio();UpdatePower();UpdateCross();Apply();Mark();if(crossBox!=null&&!crossBox.IsDisposed)crossBox.Checked=false;}emergencyLatch=emergency;
        if(data.DarkOnlyGames&&Environment.TickCount64>=nextDarkProcessPoll){nextDarkProcessPoll=Environment.TickCount64+1500;darkGameRunning=false;try{foreach(var p in Process.GetProcesses()){try{if(data.Games.ContainsKey(p.ProcessName))darkGameRunning=true;}catch{}finally{p.Dispose();}}}catch{}}
        bool keyDown=BindingDown(data.DarkKey,data.DarkModifiers);bool gate=data.DarkEnabled&&data.EffectEnabled&&!ShortcutDialog.Capturing&&(!IsMouseKey(data.DarkKey)||Environment.TickCount64>=ShortcutDialog.SuppressMouseUntil)&&(!data.DarkOnlyGames||darkGameRunning);
        if(!gate)darkToggle=false;else if(data.DarkMode=="Toggle"&&keyDown&&!lastDarkKey)darkToggle=!darkToggle;lastDarkKey=keyDown;boostHeld=gate&&(data.DarkMode=="Toggle"?darkToggle:keyDown);
        float target=boostHeld?1f:0f;float previousLevel=boostLevel;if(data.DarkFade)boostLevel=target>boostLevel?Math.Min(target,boostLevel+.0667f):Math.Max(target,boostLevel-.0667f);else boostLevel=target;if(Math.Abs(previousLevel-boostLevel)>.001f&&data.EffectEnabled)Apply();
        PollMouseShortcuts();elapsed+=20;saveElapsed+=20;if(elapsed>=300){elapsed=0;CheckGame();}if(saveElapsed>=1800){saveElapsed=0;if(data.EffectEnabled)Apply();if(dirty)SaveSettings();}}
    static string? ForegroundProcess(){try{var h=Native.GetForegroundWindow();Native.GetWindowThreadProcessId(h,out uint id);if(id!=0){using var p=Process.GetProcessById((int)id);return p.ProcessName.ToLowerInvariant();}}catch{}return null;}
    void CheckGame(){
        string? focused=ForegroundProcess();
        if(!data.AutoGames){if(autoGame!=null)RestoreManual();if(gameStatus!=null&&!gameStatus.IsDisposed)gameStatus.Text="Automatic profiles off  •  Focus: "+(focused??"unavailable");return;}
        string? name=focused!=null&&data.Games.ContainsKey(focused)?focused:null;
        if(name==null&&data.GameDetection=="Process running"){
            if(Environment.TickCount64>=nextProcessPoll){nextProcessPoll=Environment.TickCount64+1500;runningGame=null;var found=new HashSet<string>(StringComparer.OrdinalIgnoreCase);try{foreach(var proc in Process.GetProcesses()){try{found.Add(proc.ProcessName);}catch{}finally{proc.Dispose();}}runningGame=data.Games.Keys.Where(k=>found.Contains(k)&&looks.ContainsKey(data.Games[k])).OrderBy(k=>k,StringComparer.OrdinalIgnoreCase).FirstOrDefault();}catch{}}
            name=runningGame;
        }
        if(name!=null&&data.Games.TryGetValue(name,out var lookName)&&looks.TryGetValue(lookName,out var look)){
            if(!string.Equals(autoGame,name,StringComparison.OrdinalIgnoreCase)||autoLook!=lookName||autoAppliedLook!=look){if(autoGame==null)manualBeforeGame=Current();autoGame=name;autoLook=lookName;autoAppliedLook=look;SetLook(look);}
        }else if(autoGame!=null)RestoreManual();
        if(gameStatus!=null&&!gameStatus.IsDisposed)gameStatus.Text=autoGame!=null?"Active: "+autoGame+".exe  →  "+autoLook:"Waiting for a matched game  •  Focus: "+(focused??"unavailable");
    }
    void RestoreManual(){if(autoGame==null)return;autoGame=null;autoLook=null;autoAppliedLook=null;if(manualBeforeGame!=null){var l=manualBeforeGame;manualBeforeGame=null;SetLook(l);}}
    void RegisterShortcuts(){
        UnregisterShortcuts();int failedCount=0;
        void Register(int id,int key,int mods){if(key!=0&&!IsMouseKey(key)&&!Native.RegisterHotKey(Handle,id,(uint)(mods|0x4000),(uint)key))failedCount++;}
        if(data.Hotkeys)for(int i=0;i<4;i++)Register(100+i,0x31+i,3);
        if(data.UtilityShortcuts){Register(200,data.WindowKey,data.WindowModifiers);Register(201,data.CrossKey,data.CrossModifiers);Register(202,data.QuickKey,data.QuickModifiers);}
        if(data.MediaShortcuts){Register(300,data.MediaPopupKey,data.MediaPopupModifiers);Register(301,data.MediaPlayKey,data.MediaPlayModifiers);Register(302,data.MediaNextKey,data.MediaNextModifiers);Register(303,data.MediaPrevKey,data.MediaPrevModifiers);}
        registered=true;if(failedCount>0)MessageBox.Show($"{failedCount} keyboard shortcut(s) could not be registered. Windows or another app may already use them.","Prism",MessageBoxButtons.OK,MessageBoxIcon.Information);
    }
    void UnregisterShortcuts(){if(registered){foreach(int id in new[]{100,101,102,103,200,201,202,300,301,302,303})Native.UnregisterHotKey(Handle,id);registered=false;}}
    void ToggleCrosshair(){data.CrosshairEnabled=!data.CrosshairEnabled;UpdateCross();if(crossBox!=null&&!crossBox.IsDisposed)crossBox.Checked=data.CrosshairEnabled;Mark();}
    void ShortcutAction(int id){if(id==200)ShowFromTray();else if(id==201)ToggleCrosshair();else if(id==202)ShowQuickPopup();else if(id==300)ShowMediaPopup();else if(id==301)SendMedia(0xB3);else if(id==302)SendMedia(0xB0);else if(id==303)SendMedia(0xB1);}
    void PollMouseShortcuts(){void Poll(int id,bool enabled,int key,int mods){bool down=enabled&&IsMouseKey(key)&&BindingDown(key,mods);bool was=mouseEdges.TryGetValue(id,out bool previous)&&previous;mouseEdges[id]=down;if(down&&!was&&!ShortcutDialog.Capturing&&Environment.TickCount64>=ShortcutDialog.SuppressMouseUntil)ShortcutAction(id);}Poll(200,data.UtilityShortcuts,data.WindowKey,data.WindowModifiers);Poll(201,data.UtilityShortcuts,data.CrossKey,data.CrossModifiers);Poll(202,data.UtilityShortcuts,data.QuickKey,data.QuickModifiers);Poll(300,data.MediaShortcuts,data.MediaPopupKey,data.MediaPopupModifiers);Poll(301,data.MediaShortcuts,data.MediaPlayKey,data.MediaPlayModifiers);Poll(302,data.MediaShortcuts,data.MediaNextKey,data.MediaNextModifiers);Poll(303,data.MediaShortcuts,data.MediaPrevKey,data.MediaPrevModifiers);}
    protected override void WndProc(ref Message m){if(m.Msg==0x0312&&!ShortcutDialog.Capturing){int id=m.WParam.ToInt32();if(data.Hotkeys){int i=id-100;if(i>=0&&i<4&&looks.TryGetValue(data.QuickLooks[i],out var l))SetLook(l);}if((data.UtilityShortcuts&&id>=200&&id<=202)||(data.MediaShortcuts&&id>=300&&id<=303))ShortcutAction(id);}base.WndProc(ref m);}
    void UpdateCross(){if(data.CrosshairEnabled){if(cross==null||cross.IsDisposed){cross=new Crosshair();cross.Show();}cross.UpdateFrom(data);if(data.CrossSpin||data.CrossAnimation!="None")animationTimer.Start();else animationTimer.Stop();}else{animationTimer.Stop();cross?.Close();cross=null;}}
    void Cleanup(){timer.Stop();animationTimer.Stop();StopAudio();audioTimer.Dispose();UnregisterShortcuts();SaveSettings();cross?.Close();quickPopup?.Close();mediaPopup?.Close();if(tray!=null){tray.Visible=false;tray.Dispose();}trayMenu?.Dispose();trayIcon?.Dispose();gamma?.Restore();if(initialized){Native.MagSetFullscreenColorEffect(ref previous);Native.MagUninitialize();initialized=false;}}
}
sealed class Crosshair:Form {
    Settings? settings; readonly Stopwatch clock=Stopwatch.StartNew();
    public Crosshair(){FormBorderStyle=FormBorderStyle.None;ShowInTaskbar=false;TopMost=true;BackColor=Color.FromArgb(1,2,3);TransparencyKey=Color.FromArgb(1,2,3);Size=new Size(180,180);StartPosition=FormStartPosition.Manual;}
    protected override bool ShowWithoutActivation=>true;protected override CreateParams CreateParams{get{var p=base.CreateParams;p.ExStyle|=0x20|0x80|0x08000000;return p;}}
    public void UpdateFrom(Settings s){settings=s;var screens=Screen.AllScreens;var b=screens[Math.Clamp(s.Monitor,0,screens.Length-1)].Bounds;Location=new Point(b.Left+(b.Width-Width)/2+Math.Clamp(s.CrossOffsetX,-200,200),b.Top+(b.Height-Height)/2+Math.Clamp(s.CrossOffsetY,-200,200));Invalidate();}
    public static Color ResolveColor(Settings s){if(ColourWheel.TryHex(s.CrossColorHex,out Color c))return c.ToArgb()==Color.FromArgb(1,2,3).ToArgb()?Color.FromArgb(2,2,3):c;return s.CrossColor switch{"White"=>Color.White,"Cyan"=>Color.Cyan,"Red"=>Color.Red,"Yellow"=>Color.Yellow,"Magenta"=>Color.Magenta,_=>Color.Lime};}
    protected override void OnPaint(PaintEventArgs e){base.OnPaint(e);if(settings==null)return;var s=settings;var color=ResolveColor(s);var g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;float x=Width/2f,y=Height/2f,size=Math.Clamp(s.CrossSize,3,35),gap=Math.Clamp(s.CrossGap,0,20),thick=Math.Clamp(s.CrossThickness,1,8);double time=clock.Elapsed.TotalSeconds;double speed=Math.Clamp(s.CrossAnimationSpeed,25,300)/100d;if(s.CrossAnimation=="Pulse")size*=(float)(.88+.22*Math.Sin(time*2*Math.PI*speed));else if(s.CrossAnimation=="Breathe")gap+=(float)(3+3*Math.Sin(time*2*Math.PI*.5*speed));else if(s.CrossAnimation=="Rainbow")color=ColourWheel.FromHsv((time*100*speed)%360,1,1);float angle=Math.Clamp(s.CrossAngle,-180,180)+(s.CrossSpin?(float)((time*Math.Clamp(s.CrossSpinSpeed,-360,360))%360):0);if(angle!=0){g.TranslateTransform(x,y);g.RotateTransform(angle);g.TranslateTransform(-x,-y);}using var pen=new Pen(color,thick);using var edge=new Pen(Color.Black,thick+2);using var fill=new SolidBrush(color);using var black=new SolidBrush(Color.Black);
        void PaintShape(Pen p){if(s.CrossShape=="Circle")g.DrawEllipse(p,x-size,y-size,size*2,size*2);else if(s.CrossShape=="Plus"){g.DrawLine(p,x-size,y,x+size,y);g.DrawLine(p,x,y-size,x,y+size);}else if(s.CrossShape=="Diamond"){g.DrawPolygon(p,new[]{new PointF(x,y-size),new PointF(x+size,y),new PointF(x,y+size),new PointF(x-size,y)});}else if(s.CrossShape=="Chevron"){g.DrawLine(p,x-size,y+size/2,x,y-size/2);g.DrawLine(p,x,y-size/2,x+size,y+size/2);}else if(s.CrossShape=="X-shape"){float a=gap*.707f,b=(gap+size)*.707f;g.DrawLine(p,x-a,y-a,x-b,y-b);g.DrawLine(p,x+a,y+a,x+b,y+b);g.DrawLine(p,x-a,y+a,x-b,y+b);g.DrawLine(p,x+a,y-a,x+b,y-b);}else if(s.CrossShape!="Dot"){g.DrawLine(p,x-gap-size,y,x-gap,y);g.DrawLine(p,x+gap,y,x+gap+size,y);if(s.CrossShape!="T-shape")g.DrawLine(p,x,y-gap-size,x,y-gap);g.DrawLine(p,x,y+gap,x,y+gap+size);}}
        if(s.CrossOutline)PaintShape(edge);PaintShape(pen);if(s.CrossDot||s.CrossShape=="Dot"){float rad=Math.Max(2,thick*1.4f);if(s.CrossOutline)g.FillEllipse(black,x-rad-1,y-rad-1,2*rad+2,2*rad+2);g.FillEllipse(fill,x-rad,y-rad,rad*2,rad*2);}}
}

// GDI gamma is separate from the linear Magnification colour matrix. Save the
// original ramps before modifying them, and recover them after an abnormal exit.
sealed class GammaController {
    readonly string path; Dictionary<string,ushort[]>? backup; string? blocked;
    public GammaController(string folder){path=Path.Combine(folder,"gamma-recovery-v3.json");}
    static IntPtr Open(string device)=>Native.CreateDC("DISPLAY",device,IntPtr.Zero,IntPtr.Zero);
    static ushort[]? Read(string device){var dc=Open(device);if(dc==IntPtr.Zero)return null;try{var a=new ushort[768];return Native.GetDeviceGammaRamp(dc,a)?a:null;}finally{Native.DeleteDC(dc);}}
    static bool Write(string device,ushort[] ramp){var dc=Open(device);if(dc==IntPtr.Zero)return false;try{return Native.SetDeviceGammaRamp(dc,ramp);}finally{Native.DeleteDC(dc);}}
    public string? Recover(){if(!File.Exists(path))return null;try{backup=JsonSerializer.Deserialize<Dictionary<string,ushort[]>>(File.ReadAllText(path));if(backup==null||backup.Count==0||backup.Any(p=>p.Value==null||p.Value.Length!=768)){blocked="Gamma recovery file is invalid; gamma disabled.";return blocked;}string? error=Restore();if(error!=null){blocked="Gamma recovery failed; reconnect displays or restart Windows.";return blocked;}return null;}catch{blocked="Gamma recovery failed; gamma disabled. Restart Windows.";return blocked;}}
    public string? SetGamma(int value){
        if(blocked!=null)return blocked;
        if(value==100)return Restore();
        try{
            if(backup==null){
                var ramps=new Dictionary<string,ushort[]>();foreach(var screen in Screen.AllScreens){var ramp=Read(screen.DeviceName);if(ramp==null)return "Gamma unavailable on one or more displays; try HDR off.";ramps[screen.DeviceName]=ramp;}
                if(ramps.Count==0)return "No displays available for gamma.";
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);File.WriteAllText(path,JsonSerializer.Serialize(ramps));backup=ramps;
            }
            double gamma=Math.Clamp(value,50,200)/100.0;
            foreach(var (device,original) in backup){
                var ramp=new ushort[768];
                for(int channel=0;channel<3;channel++)for(int i=0;i<256;i++){
                    double x=Math.Pow(i/255.0,1.0/gamma)*255.0;int lo=(int)Math.Floor(x), hi=Math.Min(255,lo+1);double t=x-lo;
                    ramp[channel*256+i]=(ushort)Math.Clamp((int)Math.Round(original[channel*256+lo]*(1-t)+original[channel*256+hi]*t),0,65535);
                }
                if(!Write(device,ramp)){string? error=Restore();return error??"Gamma rejected by a display driver; try HDR off.";}
            }
            return null;
        }catch{return "Gamma unavailable; the recovery file has been retained.";}
    }
    public string? Restore(){if(backup==null)return blocked;bool all=true;foreach(var p in backup)if(!Write(p.Key,p.Value))all=false;if(!all)return "Gamma restore failed; reconnect displays or restart Windows.";try{if(File.Exists(path))File.Delete(path);}catch{return "Gamma restored, but recovery file could not be removed.";}backup=null;return null;}
}

sealed class ColourWheel:Control {
    public event EventHandler? Changed;
    public double Hue{get;private set;} public double Saturation{get;private set;} public double Brightness{get;private set;}=1;
    Bitmap? image; const int Extent=244; const float Center=122,Radius=109;
    public ColourWheel(){Size=new Size(Extent,Extent);MinimumSize=Size;MaximumSize=Size;BackColor=Palette.Card;DoubleBuffered=true;Cursor=Cursors.Cross;}
    public static bool TryHex(string? text,out Color color){color=Color.Empty;string t=(text??"").Trim().TrimStart('#');if(t.Length!=6||!int.TryParse(t,System.Globalization.NumberStyles.HexNumber,null,out int v))return false;color=Color.FromArgb((v>>16)&255,(v>>8)&255,v&255);return true;}
    public static Color FromHsv(double h,double s,double v){h=(h%360+360)%360;s=Math.Clamp(s,0,1);v=Math.Clamp(v,0,1);double c=v*s,x=c*(1-Math.Abs(h/60%2-1)),m=v-c;(double r,double g,double b)=h switch{<60=>(c,x,0d),<120=>(x,c,0d),<180=>(0d,c,x),<240=>(0d,x,c),<300=>(x,0d,c),_=>(c,0d,x)};return Color.FromArgb((int)Math.Round((r+m)*255),(int)Math.Round((g+m)*255),(int)Math.Round((b+m)*255));}
    public Color Selected=>FromHsv(Hue,Saturation,Brightness);
    public void SetColour(Color c){double r=c.R/255d,g=c.G/255d,b=c.B/255d,max=Math.Max(r,Math.Max(g,b)),min=Math.Min(r,Math.Min(g,b)),d=max-min;Brightness=max;Saturation=max==0?0:d/max;if(d==0)Hue=0;else if(max==r)Hue=60*((g-b)/d%6);else if(max==g)Hue=60*((b-r)/d+2);else Hue=60*((r-g)/d+4);if(Hue<0)Hue+=360;Invalidate();Changed?.Invoke(this,EventArgs.Empty);}
    public void SetBrightness(double value){Brightness=Math.Clamp(value,0,1);Invalidate();Changed?.Invoke(this,EventArgs.Empty);}
    void BuildImage(){image=new Bitmap(Extent,Extent,System.Drawing.Imaging.PixelFormat.Format32bppArgb);for(int y=0;y<Extent;y++)for(int x=0;x<Extent;x++){double dx=x-Center,dy=y-Center,dist=Math.Sqrt(dx*dx+dy*dy);if(dist<=Radius){double hue=Math.Atan2(dy,dx)*180/Math.PI;if(hue<0)hue+=360;image.SetPixel(x,y,FromHsv(hue,dist/Radius,1));}}}
    protected override void OnPaint(PaintEventArgs e){base.OnPaint(e);if(image==null)BuildImage();var g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;g.DrawImageUnscaled(image!,0,0);double a=Hue*Math.PI/180;float x=Center+(float)(Math.Cos(a)*Radius*Saturation),y=Center+(float)(Math.Sin(a)*Radius*Saturation);using var border=new Pen(Color.Black,3);using var white=new Pen(Color.White,1.5f);g.DrawEllipse(border,x-6,y-6,12,12);g.DrawEllipse(white,x-6,y-6,12,12);}
    void SelectAt(int x,int y){double dx=x-Center,dy=y-Center;Saturation=Math.Clamp(Math.Sqrt(dx*dx+dy*dy)/Radius,0,1);Hue=Math.Atan2(dy,dx)*180/Math.PI;if(Hue<0)Hue+=360;Invalidate();Changed?.Invoke(this,EventArgs.Empty);}
    protected override void OnMouseDown(MouseEventArgs e){base.OnMouseDown(e);if(e.Button==MouseButtons.Left){Capture=true;SelectAt(e.X,e.Y);}}
    protected override void OnMouseMove(MouseEventArgs e){base.OnMouseMove(e);if(Capture&&e.Button==MouseButtons.Left)SelectAt(e.X,e.Y);}
    protected override void OnMouseUp(MouseEventArgs e){base.OnMouseUp(e);Capture=false;}
    protected override void Dispose(bool disposing){if(disposing)image?.Dispose();base.Dispose(disposing);}
}
sealed class ColourWheelDialog:Form {
    readonly ColourWheel wheel=new(); readonly SmoothSlider brightness=new(){Caption="Brightness",Min=0,Max=100,Unit="%",Width=185}; readonly TextBox hex=new(); readonly Panel swatch=new(); bool updating;
    public Color SelectedColour=>wheel.Selected;
    public ColourWheelDialog(Color initial){Text="Crosshair colour";ClientSize=new Size(510,370);StartPosition=FormStartPosition.CenterParent;FormBorderStyle=FormBorderStyle.FixedDialog;MaximizeBox=false;MinimizeBox=false;BackColor=Palette.Card;ForeColor=Palette.Text;Font=Palette.Font(9);AutoScaleMode=AutoScaleMode.Dpi;
        var title=new Label{Text="Choose a crosshair colour",Left=20,Top=15,Width=460,Height=35,Font=Palette.Font(14,FontStyle.Bold),ForeColor=Palette.Text};Controls.Add(title);
        var note=new Label{Text="Select a hue and saturation, then adjust brightness.",Left=22,Top=50,Width=450,Height=25,ForeColor=Palette.Muted};Controls.Add(note);wheel.Location=new Point(17,80);Controls.Add(wheel);
        var hint=new Label{Text="COLOUR",Left=280,Top=88,Width=180,Height=24,ForeColor=Palette.Violet,Font=Palette.Font(8,FontStyle.Bold)};Controls.Add(hint);swatch.SetBounds(280,115,185,38);swatch.BackColor=initial;Controls.Add(swatch);
        brightness.Location=new Point(280,164);Controls.Add(brightness);var label=new Label{Text="HEX",Left=280,Top=243,Width=80,Height=20,ForeColor=Palette.Muted};Controls.Add(label);hex.SetBounds(280,266,105,28);hex.BackColor=Palette.Card2;hex.ForeColor=Palette.Text;hex.BorderStyle=BorderStyle.FixedSingle;Controls.Add(hex);
        var hexApply=new RoundedButton{Text="Apply",Left=393,Top=263,Width=72,Height=30,BackColor=Palette.Card2,ForeColor=Palette.Text,FlatStyle=FlatStyle.Flat};hexApply.Click+=(_,__)=>{if(ColourWheel.TryHex(hex.Text,out var c))wheel.SetColour(c);else MessageBox.Show("Enter a six-digit colour such as #FF00AA.","Invalid colour");};Controls.Add(hexApply);
        var ok=new RoundedButton{Text="Use colour",Left=350,Top=320,Width=115,Height=32,BackColor=Palette.Violet,ForeColor=Palette.Bg,DialogResult=DialogResult.OK,FlatStyle=FlatStyle.Flat};var cancel=new RoundedButton{Text="Cancel",Left=230,Top=320,Width=110,Height=32,BackColor=Palette.Card2,ForeColor=Palette.Text,DialogResult=DialogResult.Cancel,FlatStyle=FlatStyle.Flat};Controls.Add(ok);Controls.Add(cancel);AcceptButton=ok;CancelButton=cancel;
        wheel.Changed+=(_,__)=>UpdateUi();brightness.Changed+=(_,__)=>{if(!updating)wheel.SetBrightness(brightness.Value/100d);};wheel.SetColour(initial);
    }
    void UpdateUi(){updating=true;var c=wheel.Selected;swatch.BackColor=c;hex.Text=$"#{c.R:X2}{c.G:X2}{c.B:X2}";brightness.Value=(int)Math.Round(wheel.Brightness*100);updating=false;}
}

sealed record DisplayMode(int Width,int Height,int Hertz,int Bits){
    public override string ToString()=>$"{Width} × {Height}     •     {Hertz} Hz     •     {DisplayController.Aspect(Width,Height)}";
}
sealed class ResolutionBackup { public string Device {get;set;}=""; public Native.DisplaySettings Original {get;set;} }
static class DisplayController {
    public const int Fields=0x00040000|0x00080000|0x00100000|0x00400000;
    public static Native.DisplaySettings Blank()=>new(){dmSize=(short)Marshal.SizeOf<Native.DisplaySettings>(),dmDeviceName="",dmFormName=""};
    public static Native.DisplaySettings? Current(string device){var m=Blank();return Native.EnumDisplaySettingsEx(device,-1,ref m,0)?m:null;}
    public static List<DisplayMode> Modes(string device){var list=new List<DisplayMode>();var set=new HashSet<DisplayMode>();for(int i=0;i<3000;i++){var m=Blank();if(!Native.EnumDisplaySettingsEx(device,i,ref m,0))break;if(m.dmPelsWidth<640||m.dmPelsHeight<480||m.dmDisplayFrequency<1||m.dmBitsPerPel!=32||(m.dmDisplayFlags&2)!=0)continue;var mode=new DisplayMode(m.dmPelsWidth,m.dmPelsHeight,m.dmDisplayFrequency,m.dmBitsPerPel);if(set.Add(mode))list.Add(mode);}return list.OrderByDescending(x=>x.Width).ThenByDescending(x=>x.Height).ThenByDescending(x=>x.Hertz).ToList();}
    public static string Aspect(int w,int h){double r=w/(double)h;if(Math.Abs(r-4d/3)<.02)return "4:3";if(Math.Abs(r-16d/9)<.02)return "16:9";if(Math.Abs(r-16d/10)<.02)return "16:10";if(Math.Abs(r-5d/4)<.02)return "5:4";if(r>2.25&&r<2.45)return "21:9 (ultrawide)";return "Other";}
    public static bool Matches(DisplayMode mode,string filter)=>filter=="All aspect ratios"||(filter=="21:9"?Aspect(mode.Width,mode.Height).StartsWith("21:9"):Aspect(mode.Width,mode.Height)==filter);
    public static int Change(string device,Native.DisplaySettings settings,int flags){settings.dmFields=Fields;settings.dmSize=(short)Marshal.SizeOf<Native.DisplaySettings>();return Native.ChangeDisplaySettingsEx(device,ref settings,IntPtr.Zero,flags,IntPtr.Zero);}
    public static void TryChange(Form owner,string folder,string device,DisplayMode target){
        var current=Current(device);if(current==null){MessageBox.Show("Could not read the current display mode.","Resolution",MessageBoxButtons.OK,MessageBoxIcon.Warning);return;}var before=current.Value;var next=before;next.dmPelsWidth=target.Width;next.dmPelsHeight=target.Height;next.dmDisplayFrequency=target.Hertz;next.dmBitsPerPel=target.Bits;
        int test=Change(device,next,2);if(test!=0){MessageBox.Show($"Windows could not use that mode (code {test}). No change was made.","Resolution",MessageBoxButtons.OK,MessageBoxIcon.Warning);return;}
        string? backupPath=null;
        try{
            Directory.CreateDirectory(folder);backupPath=Path.Combine(folder,"resolution-"+Guid.NewGuid().ToString("N")+".json");File.WriteAllText(backupPath,JsonSerializer.Serialize(new ResolutionBackup{Device=device,Original=before},ResolutionGuard.JsonOptions));
            string? exe=Environment.ProcessPath;if(string.IsNullOrEmpty(exe))throw new Exception("Could not locate the application executable.");var info=new ProcessStartInfo(exe){UseShellExecute=false,CreateNoWindow=true};info.ArgumentList.Add("--resolution-guard");info.ArgumentList.Add(backupPath);using var guard=Process.Start(info);if(guard==null)throw new Exception("Could not start the recovery helper.");
        }catch(Exception ex){if(backupPath!=null)try{File.Delete(backupPath);}catch{}MessageBox.Show("No change was made because the recovery helper could not start.\n\n"+ex.Message,"Resolution",MessageBoxButtons.OK,MessageBoxIcon.Warning);return;}
        int result=Change(device,next,0);if(result!=0){try{File.WriteAllText(backupPath+".ok","not applied");}catch{}MessageBox.Show($"Windows rejected that display mode (code {result}).","Resolution",MessageBoxButtons.OK,MessageBoxIcon.Warning);return;}
        using var confirm=new ResolutionConfirmDialog(target.ToString());bool keep=confirm.ShowDialog(owner)==DialogResult.OK;
        if(keep){try{File.WriteAllText(backupPath+".ok","confirmed");}catch{int restore=Change(device,before,0);MessageBox.Show(restore==0?"The change was reverted because the safety confirmation could not be saved.":"Could not confirm or restore the display mode. The safety helper will retry.","Resolution",MessageBoxButtons.OK,MessageBoxIcon.Warning);}}
        else{int restore=Change(device,before,0);if(restore==0){try{File.WriteAllText(backupPath+".ok","reverted");}catch{}}else MessageBox.Show("Windows could not restore the previous mode. The safety helper will retry shortly. If needed, restart Windows in low-resolution video mode.","Resolution",MessageBoxButtons.OK,MessageBoxIcon.Warning);}
    }
}
static class ResolutionGuard {
    public static readonly JsonSerializerOptions JsonOptions=new(){IncludeFields=true};
    public static void Run(string path){try{string folder=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"PrismDisplay");if(!Path.GetFullPath(path).StartsWith(Path.GetFullPath(folder)+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase))return;var backup=JsonSerializer.Deserialize<ResolutionBackup>(File.ReadAllText(path),JsonOptions);if(backup==null||string.IsNullOrEmpty(backup.Device))return;System.Threading.Thread.Sleep(25000);string ok=path+".ok";if(File.Exists(ok)){File.Delete(ok);File.Delete(path);return;}int result=DisplayController.Change(backup.Device,backup.Original,0);if(result==0)File.Delete(path);else File.WriteAllText(path+".error","Windows restore failed: "+result);}catch{}}
}
sealed class ResolutionConfirmDialog:Form {
    int remaining=15;readonly System.Windows.Forms.Timer timer=new(){Interval=1000};readonly Label countdown;
    public ResolutionConfirmDialog(string mode){Text="Confirm display mode";ClientSize=new Size(450,190);StartPosition=FormStartPosition.CenterScreen;FormBorderStyle=FormBorderStyle.FixedDialog;MaximizeBox=false;MinimizeBox=false;TopMost=true;BackColor=Palette.Card;ForeColor=Palette.Text;Font=Palette.Font(9);Controls.Add(new Label{Text="Keep this display mode?",Left=20,Top=16,Width=410,Height=30,Font=Palette.Font(14,FontStyle.Bold),ForeColor=Palette.Text});Controls.Add(new Label{Text=mode,Left=22,Top=50,Width=405,Height=25,ForeColor=Palette.Muted});countdown=new Label{Text="Reverting automatically in 15 seconds.",Left=22,Top=81,Width=405,Height=25,ForeColor=Palette.Violet};Controls.Add(countdown);var keep=new RoundedButton{Text="Keep changes",Left=193,Top=128,Width=120,Height=36,BackColor=Palette.Violet,ForeColor=Palette.Bg,DialogResult=DialogResult.OK};var revert=new RoundedButton{Text="Revert",Left=320,Top=128,Width=110,Height=36,BackColor=Palette.Card2,ForeColor=Palette.Text,DialogResult=DialogResult.Cancel};Controls.Add(keep);Controls.Add(revert);AcceptButton=keep;CancelButton=revert;timer.Tick+=(_,__)=>{remaining--;countdown.Text=$"Reverting automatically in {remaining} seconds.";if(remaining<=0){timer.Stop();DialogResult=DialogResult.Cancel;Close();}};Shown+=(_,__)=>timer.Start();FormClosed+=(_,__)=>timer.Stop();}
    protected override void Dispose(bool disposing){if(disposing)timer.Dispose();base.Dispose(disposing);}
}

sealed class ShortcutDialog:Form {
    public static bool Capturing {get;private set;} public static long SuppressMouseUntil {get;private set;}
    public int KeyCode{get;private set;} public int Modifiers{get;private set;}
    readonly System.Windows.Forms.Timer mouseTimer=new(){Interval=20}; long readyAt; bool armed;
    public ShortcutDialog(string label){Text="Set shortcut";ClientSize=new Size(470,180);StartPosition=FormStartPosition.CenterParent;FormBorderStyle=FormBorderStyle.FixedDialog;MaximizeBox=false;MinimizeBox=false;KeyPreview=true;BackColor=Palette.Card;ForeColor=Palette.Text;Font=Palette.Font(9);Controls.Add(new Label{Text=label,Left=18,Top=15,Width=430,Height=32,ForeColor=Palette.Text,Font=Palette.Font(13,FontStyle.Bold)});Controls.Add(new Label{Text="Press a keyboard key or a mouse button. Esc cancels.\nRelease all mouse buttons first, then press the button you want.\nLeft/right click bindings will also fire during ordinary clicks.",Left=20,Top=53,Width=430,Height=75,ForeColor=Palette.Muted});KeyDown+=CaptureKey;Shown+=(_,__)=>{Capturing=true;readyAt=Environment.TickCount64+400;mouseTimer.Start();WindowChrome.Apply(this);};mouseTimer.Tick+=(_,__)=>CaptureMouse();FormClosed+=(_,__)=>{mouseTimer.Stop();Capturing=false;SuppressMouseUntil=Environment.TickCount64+400;};}
    void Finish(int key,int mods){if(key==(int)Keys.F12&&mods==3){MessageBox.Show("Ctrl + Alt + F12 is reserved for emergency off.");return;}KeyCode=key;Modifiers=mods;DialogResult=DialogResult.OK;Close();}
    void CaptureKey(object? sender,KeyEventArgs e){if(e.KeyCode==Keys.Escape){DialogResult=DialogResult.Cancel;Close();return;}if(e.KeyCode is Keys.ControlKey or Keys.ShiftKey or Keys.Menu or Keys.LWin or Keys.RWin)return;e.SuppressKeyPress=true;Finish((int)e.KeyCode,(e.Control?2:0)|(e.Alt?1:0)|(e.Shift?4:0));}
    void CaptureMouse(){if(Environment.TickCount64<readyAt)return;int[] keys={1,2,4,5,6};if(!armed){if(!keys.Any(Native.Down))armed=true;return;}foreach(int k in keys)if(Native.Down(k)){int mods=(Native.Down(0x11)?2:0)|(Native.Down(0x12)?1:0)|(Native.Down(0x10)?4:0);Finish(k,mods);return;}}
    protected override void Dispose(bool disposing){if(disposing){mouseTimer.Dispose();Capturing=false;}base.Dispose(disposing);}
}

static class ColourEngine {
    public static float[] Build(Look look,Settings data,float boost){
        var zero=Evaluate(0,0,0,look,data,boost);var r=Evaluate(1,0,0,look,data,boost);var g=Evaluate(0,1,0,look,data,boost);var b=Evaluate(0,0,1,look,data,boost);float[] m=new float[25];for(int i=0;i<3;i++){m[i]=r[i]-zero[i];m[5+i]=g[i]-zero[i];m[10+i]=b[i]-zero[i];m[20+i]=zero[i];}m[18]=1;m[24]=1;return m;
    }
    static float[] Evaluate(float r,float g,float b,Look l,Settings data,float boost){
        float saturation=l.Saturation/100f,grey=.2126f*r+.7152f*g+.0722f*b;r=grey+(r-grey)*saturation;g=grey+(g-grey)*saturation;b=grey+(b-grey)*saturation;
        double a=Math.Clamp(l.Hue,-180,180)*Math.PI/180.0;float c=(float)Math.Cos(a),sn=(float)Math.Sin(a);
        float hr=(.213f+c*.787f-sn*.213f)*r+(.715f-c*.715f-sn*.715f)*g+(.072f-c*.072f+sn*.928f)*b;
        float hg=(.213f-c*.213f+sn*.143f)*r+(.715f+c*.285f+sn*.140f)*g+(.072f-c*.072f-sn*.283f)*b;
        float hb=(.213f-c*.213f-sn*.787f)*r+(.715f-c*.715f+sn*.715f)*g+(.072f+c*.928f+sn*.072f)*b;
        float contrast=l.Contrast/100f,o=l.Brightness/100f+(1-contrast)*.5f,w=l.Warmth/200f;r=contrast*hr*(l.Red/100f)+o+w;g=contrast*hg*(l.Green/100f)+o;b=contrast*hb*(l.Blue/100f)+o-w;
        switch(data.VisionMode){case "Red-weak":r+=.35f*(r-g);break;case "Green-weak":g+=.35f*(g-r);break;case "Blue-weak":b+=.35f*(b-g);break;}
        float strength=Math.Clamp(data.DarkStrength,0,100)/100f,mix=Math.Clamp(boost,0,1)*Math.Min(1f,strength*2f),gain=1+3*strength,black=Math.Clamp(data.DarkBlackLevel,0,100)/1000f*gain;
        if(mix>0){float y=.2126f*r+.7152f*g+.0722f*b;float tr,tg,tb;switch(data.DarkColour){case "Green":tr=.12f*y*gain;tg=y*gain;tb=.12f*y*gain;break;case "Amber":tr=y*gain;tg=.55f*y*gain;tb=.08f*y*gain;break;case "Cyan":tr=.08f*y*gain;tg=y*gain;tb=y*gain;break;case "White":tr=tg=tb=y*gain;break;default:tr=r*gain;tg=g*gain;tb=b*gain;break;}if(data.DarkKeepColours&&data.DarkColour!="Clean"){tr=.7f*tr+.3f*r*gain;tg=.7f*tg+.3f*g*gain;tb=.7f*tb+.3f*b*gain;}tr-=black;tg-=black;tb-=black;r+=(tr-r)*mix;g+=(tg-g)*mix;b+=(tb-b)*mix;}
        float warm=Math.Clamp(data.EyeWarmth,0,100)/100f,dim=1-Math.Clamp(data.EyeDim,0,80)/100f;g*=1-.1f*warm;b*=1-.45f*warm;r*=dim;g*=dim;b*=dim;return new[]{r,g,b};
    }
}

static class WindowChrome {
    public static void Apply(Form form){try{int enabled=1;Native.DwmSetWindowAttribute(form.Handle,20,ref enabled,4);int colour=Palette.Bg.R|(Palette.Bg.G<<8)|(Palette.Bg.B<<16);int text=Palette.Text.R|(Palette.Text.G<<8)|(Palette.Text.B<<16);Native.DwmSetWindowAttribute(form.Handle,35,ref colour,4);Native.DwmSetWindowAttribute(form.Handle,36,ref text,4);}catch{}}
}
sealed class QuickColourPopup:Form {
    readonly SmoothSlider slider; bool updating;
    public QuickColourPopup(int initial,Action<int> changed,Action toggle){Text="Quick colour";ClientSize=new Size(420,165);StartPosition=FormStartPosition.CenterScreen;FormBorderStyle=FormBorderStyle.FixedToolWindow;TopMost=true;BackColor=Palette.Card;ForeColor=Palette.Text;Font=Palette.Font(9);Controls.Add(new Label{Text="QUICK COLOUR",Font=Palette.Font(10,FontStyle.Bold),ForeColor=Palette.Violet,Left=18,Top=15,Width=360,Height=25});slider=new SmoothSlider{Caption="Saturation",Min=0,Max=350,SnapStep=50,Value=Math.Clamp(initial,0,350),Unit="%",Left=18,Top=46,Width=380};slider.Changed+=(_,__)=>{if(!updating)changed(slider.Value);};Controls.Add(slider);var button=new RoundedButton{Text="Toggle colour effect",Left=18,Top=118,Width=180,Height=33,BackColor=Palette.Card2,ForeColor=Palette.Text};button.Click+=(_,__)=>toggle();Controls.Add(button);HandleCreated+=(_,__)=>WindowChrome.Apply(this);}
    public void SetSaturation(int value){if(IsDisposed)return;updating=true;slider.Value=Math.Clamp(value,0,350);updating=false;}
}
sealed class MediaPopup:Form {
    public MediaPopup(Action<byte> send){Text="Media controls";ClientSize=new Size(340,125);StartPosition=FormStartPosition.CenterScreen;FormBorderStyle=FormBorderStyle.FixedToolWindow;TopMost=true;BackColor=Palette.Card;ForeColor=Palette.Text;Font=Palette.Font(9);Controls.Add(new Label{Text="WINDOWS MEDIA CONTROLS",Font=Palette.Font(9,FontStyle.Bold),ForeColor=Palette.Violet,Left=15,Top=13,Width=310,Height=24});Controls.Add(new Label{Text="Controls the active Windows media player.",ForeColor=Palette.Muted,Left=15,Top=38,Width=310,Height=22});var prev=new RoundedButton{Text="Previous",Left=15,Top=72,Width=95,Height=34,BackColor=Palette.Card2,ForeColor=Palette.Text};prev.Click+=(_,__)=>send(0xB1);var play=new RoundedButton{Text="Play / pause",Left=119,Top=72,Width=100,Height=34,BackColor=Palette.Card2,ForeColor=Palette.Text};play.Click+=(_,__)=>send(0xB3);var next=new RoundedButton{Text="Next",Left=228,Top=72,Width=95,Height=34,BackColor=Palette.Card2,ForeColor=Palette.Text};next.Click+=(_,__)=>send(0xB0);Controls.AddRange(new Control[]{prev,play,next});HandleCreated+=(_,__)=>WindowChrome.Apply(this);}
}

sealed class PrismLogo:Control {
    public PrismLogo(){DoubleBuffered=true;BackColor=Palette.Side;}
    protected override void OnPaint(PaintEventArgs e){base.OnPaint(e);var g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;using var light=new SolidBrush(Palette.Text);using var shade=new SolidBrush(Color.FromArgb(160,174,202));g.FillPolygon(light,new[]{new PointF(14,5),new PointF(26,28),new PointF(14,22),new PointF(2,28)});g.FillPolygon(shade,new[]{new PointF(14,5),new PointF(14,22),new PointF(26,28)});using var font=Palette.Font(14,FontStyle.Bold);g.DrawString("PRISM",font,light,33,4);}
}

// Peak-based master-volume ducking, deliberately disabled at launch. It only
// monitors the endpoint meter; it does not capture or transmit audio samples.
sealed class AudioDucker:IDisposable {
    readonly MMDeviceEnumerator enumerator; readonly MMDevice device; readonly string id; float baseline,lastSet; long nextDeviceCheck; bool disposed;
    public float Peak{get;private set;} public float Current{get;private set;} public string DeviceName=>device.FriendlyName;
    public AudioDucker(){enumerator=new MMDeviceEnumerator();try{device=enumerator.GetDefaultAudioEndpoint(DataFlow.Render,Role.Multimedia);id=device.ID;baseline=device.AudioEndpointVolume.MasterVolumeLevelScalar;lastSet=baseline;Current=baseline;}catch{enumerator.Dispose();throw;}}
    public void Tick(int threshold){if(disposed)return;if(Environment.TickCount64>=nextDeviceCheck){nextDeviceCheck=Environment.TickCount64+2000;using var currentDevice=enumerator.GetDefaultAudioEndpoint(DataFlow.Render,Role.Multimedia);if(!string.Equals(id,currentDevice.ID,StringComparison.OrdinalIgnoreCase))throw new InvalidOperationException("The default audio device changed. Re-enable it for the new device.");}
        float actual=device.AudioEndpointVolume.MasterVolumeLevelScalar;if(Math.Abs(actual-lastSet)>.005f){baseline=actual;lastSet=actual;}Peak=Math.Clamp(device.AudioMeterInformation.MasterPeakValue,0,1);float next=AudioBalance.NextVolume(baseline,actual,Peak,threshold);if(Math.Abs(next-actual)>.003f){device.AudioEndpointVolume.MasterVolumeLevelScalar=next;lastSet=next;}Current=next;
    }
    public void Dispose(){if(disposed)return;disposed=true;try{float actual=device.AudioEndpointVolume.MasterVolumeLevelScalar;if(Math.Abs(actual-lastSet)<=.005f)device.AudioEndpointVolume.MasterVolumeLevelScalar=baseline;}catch{}device.Dispose();enumerator.Dispose();}
}

static class AudioBalance {
    public static float NextVolume(float baseline,float actual,float peak,int threshold){baseline=Math.Clamp(baseline,0f,1f);actual=Math.Clamp(actual,0f,1f);peak=Math.Clamp(peak,0f,1f);float limit=Math.Clamp(threshold,10,100)/100f;float gain=peak>limit?Math.Clamp(limit/peak,.2f,1f):1f;float target=baseline*gain;float alpha=target<actual ? .45f : .04f;return Math.Clamp(actual+(target-actual)*alpha,0,baseline);}
}
