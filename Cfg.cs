using MapCore.Enum;
using MapCore.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace MapCore
{
	public class Cfg
	{
		public List<PropInfo> Props { get; set; } = new List<PropInfo>();
		public List<TextureInfo> Textures { get; set; } = new List<TextureInfo>();

		public MapManager Parent;

		/// <summary>
		/// Initialize a new instance
		/// </summary>
		public Cfg(MapManager parent)
		{
			Parent = parent;
		}

		/// <summary>
		/// Load existing prop
		/// </summary>
		/// <param name="buffer"></param>
		public void LoadProp(byte[] buffer)
		{
			Props = new List<PropInfo>();
			try
			{
				using (StreamReader b = new StreamReader(new MemoryStream(buffer)))
				{
					string line;

					string category = "";
					string renderType = "";
					string lightMapType = "";
					string visibleRatio = "";

					while ((line = b.ReadLine()) != null)
					{
						line.Trim();

						var properties = line.Split(new char[] { '=' }, 2);
						if (!line.StartsWith(";") && properties.Length == 2)
						{
							if (properties[0] == "CATEGORY") category = properties[1].ToString();
							else if (properties[0] == "RENDERTYPE") renderType = properties[1];
							else if (properties[0] == "LIGHTMAPTYPE") lightMapType = properties[1];
							else if (properties[0] == "VISIBLE_RATIO") visibleRatio = properties[1];
							else if (properties[0] == "PROPNAME")
							{
								var values = properties[1].Split(new char[] { ',' }, 2);
								if (values.Length == 2)
								{
									var prop = new PropInfo();
									prop.Id = uint.Parse(values[0]);
									prop.Category = category;
									prop.PropName = values[1];
									prop.LightMapType = lightMapType;
									prop.VisibleRatio = visibleRatio;
									prop.RenderType = renderType;
									Props.Add(prop);
								}
							}
						}
					}
				}

				Parent.Log(Levels.Good, "Ok\n");
			}
			catch (Exception exception)
			{
				Props = new List<PropInfo>();
				Parent.Log(Levels.Error, "Failed\n");
				Parent.Log(Levels.Fatal, string.Format("CfgManager::Prop::Load<Exception> -> {0}\n", exception));
			}
		}

		/// <summary>
		/// Load existing collision
		/// </summary>
		/// <param name="buffer"></param>
		public void LoadTexture(byte[] buffer)
		{
			Textures = new List<TextureInfo>();
			try
			{
				using (StreamReader b = new StreamReader(new MemoryStream(buffer)))
				{
					string line;

					string category = "";
					string details = "";

					while ((line = b.ReadLine()) != null)
					{
						line.Trim();

						var properties = line.Split(new char[] { '=' }, 2);
						if (!line.StartsWith(";") && properties.Length == 2)
						{
							if (properties[0] == "CATEGORY") category = properties[1].ToString();
							else if (properties[0] == "DETAIL") details = properties[1];
							else if (properties[0] == "TEXTURE")
							{
								var values = properties[1].Split(new char[] { ',' }, 2);
								if (values.Length == 2)
								{
									var texture = new TextureInfo();
									texture.Id = ushort.Parse(values[0]);
									texture.Detail = details;
									texture.Category = category;
									texture.TextureName = values[1];
									Textures.Add(texture);
								}
							}
						}
					}
				}

				Parent.Log(Levels.Good, "Ok\n");
			}
			catch (Exception exception)
			{
				Textures = new List<TextureInfo>();
				Parent.Log(Levels.Error, "Failed\n");
				Parent.Log(Levels.Fatal, string.Format("CfgManager::Texture::Load<Exception> -> {0}\n", exception));
			}
		}
	}
}
