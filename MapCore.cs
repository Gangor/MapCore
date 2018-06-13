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
		/// Map coordonate
		/// </summary>
		public Vector Location { get; set; } = new Vector();

		/// <summary>
		/// Real name of the file
		/// </summary>
		public string Name => $"m{Location.X.ToString("000")}_{Location.Y.ToString("000")}";


		#region Manager map file
		#pragma warning disable CS1591

		public readonly ConfigManager _configManager;
		public readonly CollisionManager _collisionManager;
		public readonly EventAreaManager _eventAreaManager;
		public readonly ImageManager _imageManager;
		public readonly QuestPropManager _questPropManager;
		public readonly LightManager _lightManager;
		public readonly PotencialManager _potencialManager;
		public readonly RegionManager _regionManager;
		public readonly ScriptManager _scriptManager;
		public readonly TerrainManager _terrainManager;
		public readonly UnknowManager _unknowManager;
		public readonly WaterManager _waterManager;

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
			_collisionManager = new CollisionManager(this);
			_collisionManager.Added += Added;
			_collisionManager.Removed += Removed;
			_collisionManager.Rendering += Rendering;
			_collisionManager.Updated += Updated;
			//
			//Region file
			//
			_regionManager = new RegionManager(this);
			_regionManager.Added += Added;
			_regionManager.Removed += Removed;
			_regionManager.Rendering += Rendering;
			_regionManager.Updated += Updated;
			//
			//Area file
			//
			_eventAreaManager = new EventAreaManager(this);
			_eventAreaManager.Added += Added;
			_eventAreaManager.Removed += Removed;
			_eventAreaManager.Rendering += Rendering;
			_eventAreaManager.Updated += Updated;
			//
			//Light file
			//
			_lightManager = new LightManager(this);
			_lightManager.Added += Added;
			_lightManager.Removed += Removed;
			_lightManager.Rendering += Rendering;
			_lightManager.Updated += Updated;
			//
			//Unknown file
			//
			_unknowManager = new UnknowManager(this);
			_unknowManager.Added += Added;
			_unknowManager.Removed += Removed;
			_unknowManager.Rendering += Rendering;
			_unknowManager.Updated += Updated;
			//
			//Script file
			//
			_scriptManager = new ScriptManager(this);
			_scriptManager.Added += Added;
			_scriptManager.Removed += Removed;
			_scriptManager.Rendering += Rendering;
			_scriptManager.Updated += Updated;
			//
			//Water file
			//
			_waterManager = new WaterManager(this);
			_waterManager.Added += Added;
			_waterManager.Removed += Removed;
			_waterManager.Rendering += Rendering;
			_waterManager.Updated += Updated;
			//
			//Quest prop file
			//
			_questPropManager = new QuestPropManager(this);
			_questPropManager.Added += Added;
			_questPropManager.Removed += Removed;
			_questPropManager.Rendering += Rendering;
			_questPropManager.Updated += Updated;
			//
			//Map plan image
			//
			_imageManager = new ImageManager(this);
		}

		#endregion

		#region Etc

		/// <summary>
		/// Dispose mapping
		/// </summary>
		public void Dispose()
		{
			Reset?.Invoke(this, EventArgs.Empty);

			_collisionManager.Blank();
			_regionManager.Blank();
			_eventAreaManager.Blank();
			_lightManager.Blank();
			_terrainManager.Blank();
			_unknowManager.Blank();
			_scriptManager.Blank();
			_waterManager.Blank();
			_potencialManager.Blank();
			_questPropManager.Blank();
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

			Export(core, $"{Name}.nfa", new Func<byte[]>(_collisionManager.GetBuffer));
			Export(core, $"{Name}.nfc", new Func<byte[]>(_regionManager.GetBuffer));
			Export(core, $"{Name}.nfe", new Func<byte[]>(_eventAreaManager.GetBuffer));
			Export(core, $"{Name}.nfl", new Func<byte[]>(_lightManager.GetBuffer));
			Export(core, $"{Name}.nfm", new Func<byte[]>(_terrainManager.GetBuffer));
			Export(core, $"{Name}.nfp", new Func<byte[]>(_unknowManager.GetBuffer));
			Export(core, $"{Name}.nfs", new Func<byte[]>(_scriptManager.GetBuffer));
			Export(core, $"{Name}.nfw", new Func<byte[]>(_waterManager.GetBuffer));
			Export(core, $"{Name}.pvs", new Func<byte[]>(_potencialManager.GetBuffer));
			Export(core, $"{Name}.qpf", new Func<byte[]>(_questPropManager.GetBuffer));
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

			Import(core, $"terrainpropinfo{encoding}.cfg", new Action<byte[]>(_configManager.LoadProp));
			Import(core, $"terraintextureinfo{encoding}.cfg", new Action<byte[]>(_configManager.LoadTexture));
			Import(core, $"{file}{encoding}.nfa", new Action<byte[]>(_collisionManager.Load));
			Import(core, $"{file}{encoding}.nfc", new Action<byte[]>(_regionManager.Load));
			Import(core, $"{file}{encoding}.nfe", new Action<byte[]>(_eventAreaManager.Load));
			Import(core, $"{file}{encoding}.nfl", new Action<byte[]>(_lightManager.Load));
			Import(core, $"{file}{encoding}.nfm", new Action<byte[]>(_terrainManager.Load));
			Import(core, $"{file}{encoding}.nfp", new Action<byte[]>(_unknowManager.Load));
			Import(core, $"{file}{encoding}.nfs", new Action<byte[]>(_scriptManager.Load));
			Import(core, $"{file}{encoding}.nfw", new Action<byte[]>(_waterManager.Load));
			Import(core, $"{file}{encoding}.pvs", new Action<byte[]>(_potencialManager.Load));
			Import(core, $"{file}{encoding}.qpf", new Action<byte[]>(_questPropManager.Load));

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
			
			Load(folder + @"\cfg", $"terrainpropinfo{encoding}.cfg", new Action<byte[]>(_configManager.LoadProp));
			Load(folder + @"\cfg", $"terraintextureinfo{encoding}.cfg", new Action<byte[]>(_configManager.LoadTexture));
			Load(folder + @"\nfa\", $"{file}{encoding}.nfa", new Action<byte[]>(_collisionManager.Load));
			Load(folder + @"\nfc\", $"{file}{encoding}.nfc", new Action<byte[]>(_regionManager.Load));
			Load(folder + @"\nfe\", $"{file}{encoding}.nfe", new Action<byte[]>(_eventAreaManager.Load));
			Load(folder + @"\nfl\", $"{file}{encoding}.nfl", new Action<byte[]>(_lightManager.Load));
			Load(folder + @"\nfm\", $"{file}{encoding}.nfm", new Action<byte[]>(_terrainManager.Load));
			Load(folder + @"\nfp\", $"{file}{encoding}.nfp", new Action<byte[]>(_unknowManager.Load));
			Load(folder + @"\nfs\", $"{file}{encoding}.nfs", new Action<byte[]>(_scriptManager.Load));
			Load(folder + @"\nfw\", $"{file}{encoding}.nfw", new Action<byte[]>(_waterManager.Load));
			Load(folder + @"\pvs\", $"{file}{encoding}.pvs", new Action<byte[]>(_potencialManager.Load));
			Load(folder + @"\qpf\", $"{file}{encoding}.qpf", new Action<byte[]>(_questPropManager.Load));

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

			Save(folder + @"\nfa\", $"{Name}.nfa", new Func<byte[]>(_collisionManager.GetBuffer));
			Save(folder + @"\nfc\", $"{Name}.nfc", new Func<byte[]>(_regionManager.GetBuffer));
			Save(folder + @"\nfe\", $"{Name}.nfe", new Func<byte[]>(_eventAreaManager.GetBuffer));
			Save(folder + @"\nfl\", $"{Name}.nfl", new Func<byte[]>(_lightManager.GetBuffer));
			Save(folder + @"\nfm\", $"{Name}.nfm", new Func<byte[]>(_terrainManager.GetBuffer));
			Save(folder + @"\nfp\", $"{Name}.nfp", new Func<byte[]>(_unknowManager.GetBuffer));
			Save(folder + @"\nfs\", $"{Name}.nfs", new Func<byte[]>(_scriptManager.GetBuffer));
			Save(folder + @"\nfw\", $"{Name}.nfw", new Func<byte[]>(_waterManager.GetBuffer));
			Save(folder + @"\pvs\", $"{Name}.pvs", new Func<byte[]>(_potencialManager.GetBuffer));
			Save(folder + @"\qpf\", $"{Name}.qpf", new Func<byte[]>(_questPropManager.GetBuffer));

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
			var directory = Path.Combine(path, "nfs");

			if (Directory.Exists(directory))
			{
				var files = Directory.GetFiles(directory, "*.nfs", SearchOption.AllDirectories);
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
			var entries = core.GetEntriesByExtension("nfs");

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
