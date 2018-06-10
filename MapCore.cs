using DataCore;
using MapCore.Enum;
using MapCore.Events;
using MapCore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MapCore
{
	/// <summary>
	/// Global manager for all type of the file
	/// </summary>
	public class MapCore
	{
		/// <summary>
		/// Map coordonate x
		/// </summary>
		public Vector Location { get; set; } = new Vector();

		/// <summary>
		/// Real name of the file
		/// </summary>
		public string Name => $"m{Location.X.ToString("000")}_{Location.Y.ToString("000")}";


		#region Manager map file

		/// <summary>
		/// Manager for config
		/// </summary>
		public ConfigManager Cfg { get; set; }
		
		/// <summary>
		/// Manager for collision
		/// </summary>
		public CollisionManager Nfa { get; set; }

		/// <summary>
		/// Manager for region
		/// </summary>
		public RegionManager Nfc { get; set; }

		/// <summary>
		/// Manager for event area
		/// </summary>
		public EventAreaManager Nfe { get; set; }

		/// <summary>
		/// Manager for light
		/// </summary>
		public LightManager Nfl { get; set; }

		/// <summary>
		/// Manager for terrain
		/// </summary>
		public TerrainManager Nfm { get; set; }

		/// <summary>
		/// Manager for unknow
		/// </summary>
		public UnknowManager Nfp { get; set; }

		/// <summary>
		/// Manager for script
		/// </summary>
		public ScriptManager Nfs { get; set; }

		/// <summary>
		/// Manager for water
		/// </summary>
		public WaterManager Nfw { get; set; }

		/// <summary>
		/// Manager for quest prop
		/// </summary>
		public QuestPropManager Qpf { get; set; }

		/// <summary>
		/// Manager for potencial visible set
		/// </summary>
		public PotencialManager Pvs { get; set; }
		
		/// <summary>
		/// Manager for plan
		/// </summary>
		public ImageManager Plan { get; set; }

		#endregion

		#region Events

		/// <summary>
		/// Event when add objet
		/// </summary>
		public event EventHandler<AddedArgs> Added;

		/// <summary>
		/// Event when remove objet
		/// </summary>
		public event EventHandler<RemovedArgs> Removed;

		/// <summary>
		/// Event when render
		/// </summary>
		public event EventHandler<RenderArgs> Rendering;

		/// <summary>
		/// Event when load
		/// </summary>
		public event EventHandler<EventArgs> Reset;

		/// <summary>
		/// Event when update objet
		/// </summary>
		public event EventHandler<UpdatedArgs> Updated;

		/// <summary>
		/// Logger event
		/// </summary>
		public event EventHandler<LogArgs> Logger;

		#endregion

		#region Constructor

		/// <summary>
		/// Initialize a new instance
		/// </summary>
		public MapCore()
		{
			//
			//Collision file
			//
			Nfa = new CollisionManager(this);
			Nfa.Added += Added;
			Nfa.Removed += Removed;
			Nfa.Rendering += Rendering;
			Nfa.Updated += Updated;
			//
			//Region file
			//
			Nfc = new RegionManager(this);
			Nfc.Added += Added;
			Nfc.Removed += Removed;
			Nfc.Rendering += Rendering;
			Nfc.Updated += Updated;
			//
			//Area file
			//
			Nfe = new EventAreaManager(this);
			Nfe.Added += Added;
			Nfe.Removed += Removed;
			Nfe.Rendering += Rendering;
			Nfe.Updated += Updated;
			//
			//Light file
			//
			Nfl = new LightManager(this);
			Nfl.Added += Added;
			Nfl.Removed += Removed;
			Nfl.Rendering += Rendering;
			Nfl.Updated += Updated;
			//
			//Unknown file
			//
			Nfp = new UnknowManager(this);
			Nfp.Added += Added;
			Nfp.Removed += Removed;
			Nfp.Rendering += Rendering;
			Nfp.Updated += Updated;
			//
			//Script file
			//
			Nfs = new ScriptManager(this);
			Nfs.Added += Added;
			Nfs.Removed += Removed;
			Nfs.Rendering += Rendering;
			Nfs.Updated += Updated;
			//
			//Water file
			//
			Nfw = new WaterManager(this);
			Nfw.Added += Added;
			Nfw.Removed += Removed;
			Nfw.Rendering += Rendering;
			Nfw.Updated += Updated;
			//
			//Quest prop file
			//
			Qpf = new QuestPropManager(this);
			Qpf.Added += Added;
			Qpf.Removed += Removed;
			Qpf.Rendering += Rendering;
			Qpf.Updated += Updated;
			//
			//Map plan image
			//
			Plan = new ImageManager(this);
		}

		#endregion

		#region Etc

		/// <summary>
		/// Dispose mapping
		/// </summary>
		public void Dispose()
		{
			Reset?.Invoke(this, EventArgs.Empty);

			Nfa.Blank();
			Nfc.Blank();
			Nfe.Blank();
			Nfl.Blank();
			Nfm.Blank();
			Nfp.Blank();
			Nfs.Blank();
			Nfw.Blank();
			Pvs.Blank();
			Qpf.Blank();
		}

		/// <summary>
		/// Make new project
		/// </summary>
		public void New(string file)
		{
			Dispose();
			ResolveName(file);

			Log(Levels.Info, $"New project map {file}.\n");
		}

		/// <summary>
		/// Resolve current location
		/// </summary>
		/// <param name="name"></param>
		private void ResolveName(string name)
		{
			var match = Regex.Matches(name, "[0-9]+");
			if (match.Count == 2)
			{
				Location.X = int.Parse(match[0].Value);
				Location.Y = int.Parse(match[1].Value);
			}
			else
			{
				Location.X = 0;
				Location.Y = 0;
			}
		}

		/// <summary>
		/// Resolve encoding file 
		/// </summary>
		/// 
		/// <example>
		/// m012_000(ascii)
		/// </example>
		/// <param name="name"></param>
		/// <returns>
		/// turple :
		/// value1 = m012_000
		/// value2 = (ascii)
		/// </returns>
		public static (string, string) ResolveEncode(string name)
		{
			var realname = name;
			var codepage = string.Empty;

			var match = Regex.Match(name, @"(\(.*?\))");
			if (match.Success)
			{
				codepage = match.Groups[1].Value;
				realname = name.Replace(codepage, string.Empty);
			}
			return (realname, codepage);
		}

		#endregion

		#region Export

		/// <summary>
		/// Saving all file from data
		/// </summary>
		/// <param name="core"></param>
		/// <param name="folder"></param>
		public void Export(Core core, string folder)
		{
			Log(Levels.Info, $"Saving map {Name} from DataCore v4...");

			Export(core, $"{Name}.nfa", new Func<byte[]>(Nfa.GetBuffer));
			Export(core, $"{Name}.nfc", new Func<byte[]>(Nfc.GetBuffer));
			Export(core, $"{Name}.nfe", new Func<byte[]>(Nfe.GetBuffer));
			Export(core, $"{Name}.nfl", new Func<byte[]>(Nfl.GetBuffer));
			Export(core, $"{Name}.nfm", new Func<byte[]>(Nfm.GetBuffer));
			Export(core, $"{Name}.nfp", new Func<byte[]>(Nfp.GetBuffer));
			Export(core, $"{Name}.nfs", new Func<byte[]>(Nfs.GetBuffer));
			Export(core, $"{Name}.nfw", new Func<byte[]>(Nfw.GetBuffer));
			Export(core, $"{Name}.pvs", new Func<byte[]>(Pvs.GetBuffer));
			Export(core, $"{Name}.qpf", new Func<byte[]>(Qpf.GetBuffer));
			core.Save(folder);

			Log(Levels.Info, "Map saving completed.");
		}

		/// <summary>
		/// Export file one by one from data
		/// </summary>
		/// <param name="core"></param>
		/// <param name="filename"></param>
		/// <param name="action"></param>
		private void Export(Core core, string filename, Func<byte[]> action)
		{
			Log(Levels.Info, $"Saving {filename}...\t");

			core.ImportFileEntry(filename, action());
		}

		#endregion

		#region Import

		/// <summary>
		/// Load a existing project map by data
		/// </summary>
		/// <param name="core"></param>
		/// <param name="file"></param>
		/// <param name="encoding"></param>
		public void Import(Core core, string file, string encoding)
		{
			Dispose();
			ResolveName(file);

			Log(Levels.Info, $"Loading map {file} from DataCore v4...");

			Import(core, $"terrainpropinfo{encoding}.cfg", new Action<byte[]>(Cfg.LoadProp));
			Import(core, $"terraintextureinfo{encoding}.cfg", new Action<byte[]>(Cfg.LoadTexture));
			Import(core, $"{file}{encoding}.nfa", new Action<byte[]>(Nfa.Load));
			Import(core, $"{file}{encoding}.nfc", new Action<byte[]>(Nfc.Load));
			Import(core, $"{file}{encoding}.nfe", new Action<byte[]>(Nfe.Load));
			Import(core, $"{file}{encoding}.nfl", new Action<byte[]>(Nfl.Load));
			Import(core, $"{file}{encoding}.nfm", new Action<byte[]>(Nfm.Load));
			Import(core, $"{file}{encoding}.nfp", new Action<byte[]>(Nfp.Load));
			Import(core, $"{file}{encoding}.nfs", new Action<byte[]>(Nfs.Load));
			Import(core, $"{file}{encoding}.nfw", new Action<byte[]>(Nfw.Load));
			Import(core, $"{file}{encoding}.pvs", new Action<byte[]>(Pvs.Load));
			Import(core, $"{file}{encoding}.qpf", new Action<byte[]>(Qpf.Load));

			Log(Levels.Info, "Map loading completed.");
		}

		/// <summary>
		/// Load file one by one by data
		/// </summary>
		/// <param name="core"></param>
		/// <param name="filename"></param>
		/// <param name="action"></param>
		private void Import(Core core, string filename, Action<byte[]> action)
		{
			Log(Levels.Info, $"Loading {filename}...\t");

			if (!core.GetEntryExists(filename))
			{
				Log(Levels.Warning, "Introuvable");
				return;
			}

			action(core.GetFileBytes(filename));
		}

		#endregion

		#region Load

		/// <summary>
		/// Load a existing project map by file
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="file"></param>
		/// <param name="encoding"></param>
		public void Load(string folder, string file, string encoding)
		{
			Dispose();
			ResolveName(file);

			Log(Levels.Info, $"Loading map {file} from path {folder}...");
			
			Load(folder + @"\cfg", $"terrainpropinfo{encoding}.cfg", new Action<byte[]>(Cfg.LoadProp));
			Load(folder + @"\cfg", $"terraintextureinfo{encoding}.cfg", new Action<byte[]>(Cfg.LoadTexture));
			Load(folder + @"\nfa\", $"{file}{encoding}.nfa", new Action<byte[]>(Nfa.Load));
			Load(folder + @"\nfc\", $"{file}{encoding}.nfc", new Action<byte[]>(Nfc.Load));
			Load(folder + @"\nfe\", $"{file}{encoding}.nfe", new Action<byte[]>(Nfe.Load));
			Load(folder + @"\nfl\", $"{file}{encoding}.nfl", new Action<byte[]>(Nfl.Load));
			Load(folder + @"\nfm\", $"{file}{encoding}.nfm", new Action<byte[]>(Nfm.Load));
			Load(folder + @"\nfp\", $"{file}{encoding}.nfp", new Action<byte[]>(Nfp.Load));
			Load(folder + @"\nfs\", $"{file}{encoding}.nfs", new Action<byte[]>(Nfs.Load));
			Load(folder + @"\nfw\", $"{file}{encoding}.nfw", new Action<byte[]>(Nfw.Load));
			Load(folder + @"\pvs\", $"{file}{encoding}.pvs", new Action<byte[]>(Pvs.Load));
			Load(folder + @"\qpf\", $"{file}{encoding}.qpf", new Action<byte[]>(Qpf.Load));

			Log(Levels.Info, "Map loading completed.");
		}

		/// <summary>
		/// Load file one by one
		/// </summary>
		/// <param name="path"></param>
		/// <param name="filename"></param>
		/// <param name="action"></param>
		private void Load(string path, string filename, Action<byte[]> action)
		{
			Log(Levels.Info, $"Loading {filename}...\t");

			var fullname = Path.Combine(path, filename);

			if (!File.Exists(fullname))
			{
				Log(Levels.Warning, "Introuvable");
				return;
			}

			action(File.ReadAllBytes(fullname));
		}

		#endregion

		#region Save

		/// <summary>
		/// Save all file
		/// </summary>
		/// <param name="folder"></param>
		public void Save(string folder)
		{
			Log(Levels.Info, "Saving map {filename} on path {folder}...");

			Save(folder + @"\nfa\", $"{Name}.nfa", new Func<byte[]>(Nfa.GetBuffer));
			Save(folder + @"\nfc\", $"{Name}.nfc", new Func<byte[]>(Nfc.GetBuffer));
			Save(folder + @"\nfe\", $"{Name}.nfe", new Func<byte[]>(Nfe.GetBuffer));
			Save(folder + @"\nfl\", $"{Name}.nfl", new Func<byte[]>(Nfl.GetBuffer));
			Save(folder + @"\nfm\", $"{Name}.nfm", new Func<byte[]>(Nfm.GetBuffer));
			Save(folder + @"\nfp\", $"{Name}.nfp", new Func<byte[]>(Nfp.GetBuffer));
			Save(folder + @"\nfs\", $"{Name}.nfs", new Func<byte[]>(Nfs.GetBuffer));
			Save(folder + @"\nfw\", $"{Name}.nfw", new Func<byte[]>(Nfw.GetBuffer));
			Save(folder + @"\pvs\", $"{Name}.pvs", new Func<byte[]>(Pvs.GetBuffer));
			Save(folder + @"\qpf\", $"{Name}.qpf", new Func<byte[]>(Qpf.GetBuffer));

			Log(Levels.Info, "Map saving completed.");
		}

		/// <summary>
		/// Save file one by one
		/// </summary>
		/// <param name="path"></param>
		/// <param name="filename"></param>
		/// <param name="action"></param>
		private void Save(string path, string filename, Func<byte[]> action)
		{
			var fullname = Path.Combine(path, filename);

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			Log(Levels.Info, $"Saving {filename}...\t");
			File.WriteAllBytes(fullname, action());
		}

		#endregion

		#region Watch

		/// <summary>
		/// Watch directory for retrieves all file
		/// </summary>
		/// <param name="path"></param>
		public static List<string> Watch(string path)
		{
			var mapping = new List<string>();
			var directory = Path.Combine(path, "nfm");

			if (Directory.Exists(directory))
			{
				var files = Directory.GetFiles(directory, "m*_*", SearchOption.AllDirectories);
				foreach (var file in files)
				{
					var name = Path.GetFileNameWithoutExtension(file);
					var match = Regex.Match(name, @"\bm([0-9]+)_([0-9]+)");
					if (match.Success)
					{
						if (!mapping.Contains(name)) mapping.Add(name);
					}
				}
			}
			return mapping.OrderBy(r => r).ToList();
		}

		/// <summary>
		/// Watch data for retieves all file
		/// </summary>
		/// <param name="core"></param>
		/// <returns></returns>
		public static List<string> Watch(Core core)
		{
			var mapping = new List<string>();
			var entries = core.GetEntriesByExtension("nfm");

			foreach (var file in entries)
			{
				var name = Path.GetFileNameWithoutExtension(file.Name);
				var match = Regex.Match(name, @"\bm([0-9]+)_([0-9]+)");
				if (match.Success)
				{
					if (!mapping.Contains(name)) mapping.Add(name);
				}
			}
			return mapping.OrderBy(r => r).ToList();
		}

		#endregion

		/// <summary>
		/// Log push event
		/// </summary>
		/// <param name="level"></param>
		/// <param name="message"></param>
		public void Log(Levels level, string message)
		{
			Logger?.Invoke(this, new LogArgs(level, message));

			#if DEBUG == true
				Console.Write(message);
			#endif
		}
	}
}
