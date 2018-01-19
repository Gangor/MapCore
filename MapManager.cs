using DataCore;
using MapCore.Enum;
using MapCore.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MapCore
{
	/// <summary>
	/// Global manager for all type of the file
	/// </summary>
	public class MapManager
	{
		public static MapManager Instance = null;

		/// <summary>
		/// Variable for manage file
		/// </summary>
		#region Manager map file

		public Cfg Cfg { get; set; }
		public Nfa Nfa { get; set; }
		public Nfc Nfc { get; set; }
		public Nfe Nfe { get; set; }
		public Nfl Nfl { get; set; }
		public Nfm Nfm { get; set; }
		public Nfp Nfp { get; set; }
		public Nfs Nfs { get; set; }
		public Nfw Nfw { get; set; }
		public Qpf Qpf { get; set; }
		public Pvs Pvs { get; set; }

		public ImageManager Plan { get; set; }

		#endregion

		/// <summary>
		/// Real name of the file
		/// </summary>
		public string Name => $"m{Location.X.ToString("000")}_{Location.Y.ToString("000")}";

		/// <summary>
		/// Map coordonate x
		/// </summary>
		private Point Location = new Point();

		/// <summary>
		/// Event when painting
		/// </summary>
		public event EventHandler<PaintingArgs> Painting;

		/// <summary>
		/// Event when load
		/// </summary>
		public event EventHandler<EventArgs> Reset;

		/// <summary>
		/// Logger event
		/// </summary>
		public event EventHandler<LogArgs> Logger;

		#region Constructor

		/// <summary>
		/// Initialize a new instance
		/// </summary>
		protected MapManager(string folder)
		{
			///Map plan image
			Plan = new ImageManager(this);

			///Collision file
			Nfa = new Nfa(this);
			Nfa.Added += (o, e) => Refresh();
			Nfa.Rendering += (o, e) => Refresh(false);
			Nfa.Removed += (o, e) => Refresh();

			///Region file
			Nfc = new Nfc(this);
			Nfc.Added += (o, e) => Refresh();
			Nfc.Painting += (o, e) => Refresh(false);
			Nfc.Removed += (o, e) => Refresh();

			///Area file
			Nfe = new Nfe(this);
			Nfe.Added += (o, e) => Refresh();
			Nfe.Painting += (o, e) => Refresh(false);
			Nfe.Removed += (o, e) => Refresh();

			///Light file
			Nfl = new Nfl(this);
			Nfl.Added += (o, e) => Refresh();
			Nfl.Painting += (o, e) => Refresh(false);
			Nfl.Removed += (o, e) => Refresh();

			///Unknown file
			Nfp = new Nfp(this);
			Nfp.Added += (o, e) => Refresh();
			Nfp.Painting += (o, e) => Refresh(false);
			Nfp.Removed += (o, e) => Refresh();

			///Script file
			Nfs = new Nfs(this);
			Nfs.AddedLocation += (o, e) => Refresh();
			Nfs.AddedPropScript += (o, e) => Refresh();
			Nfs.Painting += (o, e) => Refresh(false);
			Nfs.Removed += (o, e) => Refresh();

			///Water file
			Nfw = new Nfw(this);
			Nfw.Added += (o, e) => Refresh();
			Nfw.Painting += (o, e) => Refresh(false);
			Nfw.Removed += (o, e) => Refresh();

			///Quest prop file
			Qpf = new Qpf(this);
			Qpf.Added += (o, e) => Refresh();
			Qpf.Painting += (o, e) => Refresh(false);
			Qpf.Removed += (o, e) => Refresh();
		}

		/// <summary>
		/// Initialize singleton instance
		/// </summary>
		/// <returns></returns>
		public static bool Init(string folder)
		{
			if (Instance == null)
			{
				Instance = new MapManager(folder);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Destroy singleton instance
		/// </summary>
		/// <returns></returns>
		public static bool DeInit()
		{
			if (Instance != null)
			{
				Instance = null;
				return true;
			}

			return false;
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

			Refresh();
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
		/// <returns></returns>
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

		/// <summary>
		/// Refresh all element in the map
		/// </summary>
		/// <param name="changed"></param>
		public void Refresh(bool changed = true)
		{
			Painting?.Invoke(this, new PaintingArgs(
				nfa: Nfa,
				nfc: Nfc,
				nfe: Nfe,
				nfl: Nfl,
				nfp: Nfp,
				nfs: Nfs,
				nfw: Nfw,
				pvs: Pvs,
				qpf: Qpf,
				changed: changed
			));
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
		/// <param name="folder"></param>
		/// <param name="file"></param>
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
