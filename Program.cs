using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BeatSaberBeatmapsFixer
{

	class Program
	{
		static string[] DefaultOrder = { "_version", "_events", "_notes", "_obstacles" };
		static string info = "info.dat";
		[STAThread]
		static void Main(string[] args)
		{
			Console.Title = "Beat Saber v1.20.0 Beatmaps Fixer by Ivan_Alone";
			string select = "Select directory with your Beat Saber beatmaps";

			Console.WriteLine(select + " (now will appears select window)");
			Thread.Sleep(100);
			Console.WriteLine();

			using (var fbd = new FolderBrowserDialog())
			{
				fbd.Description = select;
				DialogResult result = fbd.ShowDialog();

				if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
				{
					string[] dirs = Directory.GetDirectories(fbd.SelectedPath);

					List<string> brokenBeatmaps = new List<string>();

					Console.WriteLine("Now we are scanning your beatmaps directory to find broken beatmaps...");
					Console.WriteLine();
					foreach (var dir in dirs)
					{
						if (File.Exists(Path.Combine(dir, info)))
						{
							foreach (var lvl in Directory.GetFiles(dir, "*.dat"))
							{
								if (Path.GetFileName(lvl.ToLower()) == info.ToLower()) continue;
								bool needMapToBeFixed = false;

								using (StreamReader r = new StreamReader(lvl))
								{
									string json = r.ReadToEnd();
									r.Close();

									bool isStringNow = false;
									int i = 0;
									bool nextScreened = false;

									// Checking any char in JSON to find formatting
									foreach (char c in json.Trim())
									{
										// If current char is escaped by backslash, current symbol is not formatting
										if (nextScreened)
										{
											nextScreened = false;
										}
										else
										{
											// Escaping test
											if (isStringNow && c == '\\')
											{
												nextScreened = true;
											}
											// String begin test
											else if (c == '"')
											{
												isStringNow = !isStringNow;
											}
											else
											{
												// If non-string now and formatting char found, beatmap needs to be fixed
												if (!isStringNow && (c == '\r' || c == '\n' || c == '\t' || c == ' '))
												{
													needMapToBeFixed = true;
													break;
												}
											}
										}
										i++;
									}

									if (!needMapToBeFixed && !IsBeatmapLoadable(JsonConvert.DeserializeObject<JObject>(json)))
									{
										needMapToBeFixed = true;
									}
								}

								if (needMapToBeFixed && !brokenBeatmaps.Contains(dir))
								{
									brokenBeatmaps.Add(dir);
								}
							}
						}
					}

					if (brokenBeatmaps.Count() > 0)
					{
						Console.WriteLine("These beatmaps seems to be broken. Please check them and input NUMBERS what you want to fix (leave blank to fix all):");
						Console.WriteLine();
						var i = 1;
						foreach (var bm in brokenBeatmaps)
						{
							Console.WriteLine(i + ". " + GetBeatmapDescription(bm));
							i++;
						}
						Console.WriteLine();
						Console.Write("> ");

						List<int> numbers = new List<int>();
						string line = Console.ReadLine();
						List<string> willFixed = new List<string>();

						if (line.Trim().Length == 0)
						{
							Console.WriteLine("Will fixed all these beatmaps!");
							willFixed = brokenBeatmaps;
						}
						else
						{
							char[] breakers = { ';', ',', ':', '!', '@', '#', '$', '%', '^', '&', '(', '\t', '.', '"', '\'', '{', '}', ']', '[', '<', '>', '/', '?', '\\', '|', '+', '-', '*', '`', '~' };
							foreach (var replace in breakers)
							{
								line = line.Replace(replace, ' ');
							}

							foreach (var t in line.Split(' '))
							{
								string lex = t.Trim();

								if (lex.Length < 1) continue;

								int sel = 0;
								try
								{
									sel = Int32.Parse(lex);
								}
								catch { }
								if (sel > 0 && sel <= brokenBeatmaps.Count() && !numbers.Contains(sel))
								{
									numbers.Add(sel);
								}
							}
							Console.WriteLine();

							Console.Write("These beatmaps will fixed: ");
							for (int k = 0; k < numbers.Count(); k++)
							{
								willFixed.Add(brokenBeatmaps[numbers[k] - 1]);
								Console.Write(numbers[k] + (k < numbers.Count() - 1 ? ", " : ""));
							}
							Console.WriteLine();
						}
						Console.WriteLine();

						foreach (var beatmap in willFixed)
						{
							Console.WriteLine("Fixing beatmap now: " + GetBeatmapDescription(beatmap));

							foreach (var lvl in Directory.GetFiles(beatmap, "*.dat"))
							{
								if (Path.GetFileName(lvl.ToLower()) == info.ToLower()) continue; // If info.dat file - it does not needs any fix

								using (StreamReader r = new StreamReader(lvl))
								{
									string json = r.ReadToEnd();
									JObject j = JsonConvert.DeserializeObject<JObject>(json);
									j = RecompileOrder(j); // Fixing descriptors order in Beatmap file

									File.Copy(lvl, lvl + ".bak", true); // Creating backup
									r.Close();

									File.WriteAllText(lvl, JsonConvert.SerializeObject(j)); // Write fixed level file
								}
							}
						}
						Console.WriteLine();
						Console.WriteLine("Fixing done!");
						Console.WriteLine();
					}
					else
					{
						Console.WriteLine("No possible broken beatmap found!");
					}
				}
				else
				{
					Console.WriteLine("Selection was cancelled, aborting!");
				}
			}
			Console.WriteLine();
			Pause();
		}
		public static void Pause(bool exit_after = false)
		{
			Console.Write("Press any key to continue...");
			Console.ReadKey(true);
			if (exit_after)
				Environment.Exit(0);
		}
		public static string GetBeatmapDescription(string path)
		{
			string info_f = Path.Combine(path, info);
			if (!Directory.Exists(path)) return "";
			if (!File.Exists(info_f)) return "";

			using (StreamReader r = new StreamReader(info_f))
			{
				string json = r.ReadToEnd();
				JObject obj = JsonConvert.DeserializeObject<JObject>(json);
				string sub_test = obj.GetValue("_songAuthorName").ToString();
				string auth_test = obj.GetValue("_levelAuthorName").ToString();

				return obj.GetValue("_songAuthorName").ToString() + (sub_test.Length > 0 ? " (" + sub_test + ")" : "") + " - " + obj.GetValue("_songName").ToString() + (auth_test.Length > 0 ? " [" + auth_test + "]" : "");
			}
		}
		public static bool IsBeatmapLoadable(JObject beatmap) {
			var en = beatmap.GetEnumerator();
			while (en.MoveNext())
			{
				if (en.Current.Key == DefaultOrder[0] // For detecting (possible) valid v2.*.* beatmaps 
					|| en.Current.Key == "version") // For ckecking (possible) valid v3.0.0 beatmaps
				{
					return true;
				}
				break;
			}

			return false;
		}
		public static JObject RecompileOrder(JObject obj) {
			JObject recompile = new JObject();

			if (!obj.TryGetValue(DefaultOrder[0], out JToken mz)) // Detecting beatmaps that does not have any "_version" descriptor. It can happens too, and it's bad, and needs to be fixed.
			{
				recompile.Add(DefaultOrder[0], "2.0.0");
			}

			foreach (var order in DefaultOrder)
			{
				if (obj.TryGetValue(order, out JToken val))
				{
					recompile.Add(order, val);
				}
			}
			var en = obj.GetEnumerator();
			while (en.MoveNext())
			{
				var cur = en.Current;
				if (!recompile.TryGetValue(cur.Key, out JToken val))
				{
					recompile.Add(cur.Key, cur.Value);
				}
			}

			return recompile;
		}
	}
}
