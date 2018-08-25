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

		/// <summary>
		/// Get or set the file path
		/// </summary>
		public string Target { get; set; }

		/// <summary>
		/// Get or set the origine of the file
		/// </summary>
		public DataSource Source { get; private set; } = DataSource.FILE;


		#region Manager map file
		#pragma warning disable CS1591

		public readonly Core _core;
		public readonly ConfigManager _configManager;
		public readonly CollisionManager _collisionManager;
		public readonly EventAreaManager _eventAreaManager;
		public readonly CartographerManager _cartographerManager;
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
		/// Event when clear all objets
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
		/// <param name="path">Path to the data.000</param>
		public MapCore(string path)
		{
			// Core
			//
			_core = new Core();
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
			_cartographerManager = new CartographerManager(this);
		}

		#endregion

		#region Export

		/// <summary>
		/// Saving all file from data
		/// </summary>
		public void Export(string directory)
		{
			Log(Levels.Info, $"Export map {Name} with DataCore...");

			_core.Load(directory);

			Export($"{Name}.nfa", _collisionManager.GetBuffer);
			Export($"{Name}.nfc", _regionManager.GetBuffer);
			Export($"{Name}.nfe", _eventAreaManager.GetBuffer);
			Export($"{Name}.nfl", _lightManager.GetBuffer);
			Export($"{Name}.nfm", _terrainManager.GetBuffer);
			Export($"{Name}.nfp", _unknowManager.GetBuffer);
			Export($"{Name}.nfs", _scriptManager.GetBuffer);
			Export($"{Name}.nfw", _waterManager.GetBuffer);
			Export($"{Name}.pvs", _potencialManager.GetBuffer);
			Export($"{Name}.qpf", _questPropManager.GetBuffer);

			_core.Save();

			Log(Levels.Info, "Map export completed.");
		}

		/// <summary>
		/// Export file one by one from data
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="action"></param>
		private void Export(string filename, Func<byte[]> action)
		{
			Log(Levels.Info, $"Exporting {filename}...\t");
			_core.ImportFileEntry(filename, action());
		}

		#endregion

		#region Import

		/// <summary>
		/// Load a existing project map by data
		/// </summary>
		/// <param name="directory">Directory to the data.000</param>
		/// <param name="file"></param>
		/// <param name="encoding"></param>
		public void Import(string directory, string file, string encoding)
		{
			Dispose();
			ResolveName(file);
			SetSource(DataSource.CORE);
			SetPath(directory);

			Log(Levels.Info, $"Import map {file} with DataCore...");

			_core.Load(directory);

			Import($"terrainpropinfo{encoding}.cfg", _configManager.LoadProp);
			Import($"terraintextureinfo{encoding}.cfg", _configManager.LoadTexture);
			Import($"{file}{encoding}.nfa", _collisionManager.Load);
			Import($"{file}{encoding}.nfc", _regionManager.Load);
			Import($"{file}{encoding}.nfe", _eventAreaManager.Load);
			Import($"{file}{encoding}.nfl", _lightManager.Load);
			Import($"{file}{encoding}.nfm", _terrainManager.Load);
			Import($"{file}{encoding}.nfp", _unknowManager.Load);
			Import($"{file}{encoding}.nfs", _scriptManager.Load);
			Import($"{file}{encoding}.nfw", _waterManager.Load);
			Import($"{file}{encoding}.pvs", _potencialManager.Load);
			Import($"{file}{encoding}.qpf", _questPropManager.Load);

			_cartographerManager.Load(_core, directory, file, encoding);

			Log(Levels.Info, "Import loading completed.");
		}

		/// <summary>
		/// Load file one by one by data
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="action"></param>
		private void Import(string filename, Action<byte[]> action)
		{
			Log(Levels.Info, $"Importing {filename}...\t");

			if (!_core.GetEntryExists(filename))
			{
				Log(Levels.Warning, "Introuvable");
				return;
			}

			action(_core.GetFileBytes(filename));
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
			SetPath(folder);
			SetSource(DataSource.FILE);

			Log(Levels.Info, $"Loading map {file} from path {folder}...");
			
			Load(folder + @"\cfg", $"terrainpropinfo{encoding}.cfg", _configManager.LoadProp);
			Load(folder + @"\cfg", $"terraintextureinfo{encoding}.cfg", _configManager.LoadTexture);
			Load(folder + @"\nfa\", $"{file}{encoding}.nfa", _collisionManager.Load);
			Load(folder + @"\nfc\", $"{file}{encoding}.nfc", _regionManager.Load);
			Load(folder + @"\nfe\", $"{file}{encoding}.nfe", _eventAreaManager.Load);
			Load(folder + @"\nfl\", $"{file}{encoding}.nfl", _lightManager.Load);
			Load(folder + @"\nfm\", $"{file}{encoding}.nfm", _terrainManager.Load);
			Load(folder + @"\nfp\", $"{file}{encoding}.nfp", _unknowManager.Load);
			Load(folder + @"\nfs\", $"{file}{encoding}.nfs", _scriptManager.Load);
			Load(folder + @"\nfw\", $"{file}{encoding}.nfw", _waterManager.Load);
			Load(folder + @"\pvs\", $"{file}{encoding}.pvs", _potencialManager.Load);
			Load(folder + @"\qpf\", $"{file}{encoding}.qpf", _questPropManager.Load);

			_cartographerManager.Load(folder, file, encoding);

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
		/// Save file with the source of data
		/// </summary>
		public void Save()
		{
			switch (Source)
			{
				case DataSource.FILE: Save(Target);		break;
				case DataSource.CORE: Export(Target);	break;
			}
		}

		/// <summary>
		/// Save all file
		/// </summary>
		/// <param name="folder"></param>
		public void Save(string folder)
		{
			Log(Levels.Info, $"Saving map {Name} on path {folder}...");

			Save(folder + @"\nfa\", $"{Name}.nfa", _collisionManager.GetBuffer);
			Save(folder + @"\nfc\", $"{Name}.nfc", _regionManager.GetBuffer);
			Save(folder + @"\nfe\", $"{Name}.nfe", _eventAreaManager.GetBuffer);
			Save(folder + @"\nfl\", $"{Name}.nfl", _lightManager.GetBuffer);
			Save(folder + @"\nfm\", $"{Name}.nfm", _terrainManager.GetBuffer);
			Save(folder + @"\nfp\", $"{Name}.nfp", _unknowManager.GetBuffer);
			Save(folder + @"\nfs\", $"{Name}.nfs", _scriptManager.GetBuffer);
			Save(folder + @"\nfw\", $"{Name}.nfw", _waterManager.GetBuffer);
			Save(folder + @"\pvs\", $"{Name}.pvs", _potencialManager.GetBuffer);
			Save(folder + @"\qpf\", $"{Name}.qpf", _questPropManager.GetBuffer);

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
		/// Watch from data for retieves all file
		/// </summary>
		/// <returns></returns>
		public List<string> Watch()
		{
			var mapping = new List<string>();

			if (_core.RowCount != 0)
			{
				var names = _core.GetEntriesByExtension("nfs")
								.Select(u => Path.GetFileNameWithoutExtension(u.Name).ToLower());

				foreach (var name in names.Distinct())
				{
					var match = Regex.Match(name, @"\bm([0-9]+)_([0-9]+)");
					if (match.Success)
					{
						mapping.Add(name);
					}
				}
			}

			return mapping.OrderBy(r => r).ToList();
		}

		/// <summary>
		/// Watch from directory for retrieves all file
		/// </summary>
		/// <param name="path"></param>
		public List<string> Watch(string path)
		{
			var mapping = new List<string>();
			var directory = Path.Combine(path, "nfs");

			if (Directory.Exists(directory))
			{
				var names = Directory.GetFiles(directory, "*.nfs", SearchOption.AllDirectories)
								.Select(u => Path.GetFileNameWithoutExtension(u).ToLower());

				foreach (var name in names.Distinct())
				{
					var match = Regex.Match(name, @"\bm([0-9]+)_([0-9]+)");
					if (match.Success)
					{
						mapping.Add(name);
					}
				}
			}

			return mapping.OrderBy(r => r).ToList();
		}

		#endregion

		/// <summary>
		/// Dispose mapping
		/// </summary>
		public void Dispose()
		{
			_collisionManager.Dispose();
			_regionManager.Dispose();
			_eventAreaManager.Dispose();
			_lightManager.Dispose();
			_terrainManager.Dispose();
			_unknowManager.Dispose();
			_scriptManager.Dispose();
			_waterManager.Dispose();
			_potencialManager.Dispose();
			_questPropManager.Dispose();

			Reset?.Invoke(this, EventArgs.Empty);
		}

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

		/// <summary>
		/// Make new project
		/// </summary>
		public void New(string file)
		{
			Dispose();
			ResolveName(file);
			SetSource(DataSource.FILE);

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

		/// <summary>
		/// Change the directory target
		/// </summary>
		/// <param name="path"></param>
		private void SetPath(string path)
		{
			Target = path;
		}

		/// <summary>
		/// Change the origin of the file
		/// </summary>
		/// <param name="source"></param>
		private void SetSource(DataSource source)
		{
			Source = source;
		}
	}
}
