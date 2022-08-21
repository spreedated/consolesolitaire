using ConsoleSolitaire.Classes.Json;
using ConsoleSolitaire.Models;
using ConsoleSolitaire.Models.Protobuf;
using Newtonsoft.Json;
using neXn.Lib.Playingcards.Classes;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace ConsoleSolitaire.Classes
{
    internal static class SaveGame
    {
        internal const string SAVESUFFIX = "nxn";
        internal readonly static string saveDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "saves");

        internal static string SerializeTableuPile(TableuPile pile)
        {
            string jsonWrongStacked = JsonConvert.SerializeObject(pile, new JsonSerializerSettings() { ContractResolver = new SolitaireObjectsResolver() });
            TableuPile u = JsonConvert.DeserializeObject<TableuPile>(jsonWrongStacked, new JsonSerializerSettings() { ContractResolver = new SolitaireObjectsResolver() });
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(u, new JsonSerializerSettings() { ContractResolver = new SolitaireObjectsResolver(), Formatting = Formatting.Indented })));
        }

        internal static string SerializeBuildingPile(BuildingPile pile)
        {
            string jsonWrongStacked = JsonConvert.SerializeObject(pile, new JsonSerializerSettings() { ContractResolver = new SolitaireObjectsResolver() });
            BuildingPile u = JsonConvert.DeserializeObject<BuildingPile>(jsonWrongStacked, new JsonSerializerSettings() { ContractResolver = new SolitaireObjectsResolver() });
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(u, new JsonSerializerSettings() { ContractResolver = new SolitaireObjectsResolver(), Formatting = Formatting.Indented })));
        }

        internal static string SerializeTalon(Deck deck)
        {
            string jsonWrongStacked = JsonConvert.SerializeObject(deck, new JsonSerializerSettings() { ContractResolver = new SolitaireObjectsResolver() });
            Deck u = JsonConvert.DeserializeObject<Deck>(jsonWrongStacked, new JsonSerializerSettings() { ContractResolver = new SolitaireObjectsResolver() });
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(u, new JsonSerializerSettings() { ContractResolver = new SolitaireObjectsResolver(), Formatting = Formatting.Indented })));
        }

        internal static void PrepareDir()
        {
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }
        }

        public static bool SaveExists(string filename)
        {
            return File.Exists(Path.Combine(saveDirectory, filename + $".{SAVESUFFIX}"));
        }

        public static SaveStateResponse SaveState(string name = null)
        {
            SaveState s = new()
            {
                Pile1 = SerializeTableuPile(Program.Pile1),
                Pile2 = SerializeTableuPile(Program.Pile2),
                Pile3 = SerializeTableuPile(Program.Pile3),
                Pile4 = SerializeTableuPile(Program.Pile4),
                Pile5 = SerializeTableuPile(Program.Pile5),
                Pile6 = SerializeTableuPile(Program.Pile6),
                Pile7 = SerializeTableuPile(Program.Pile7),
                BuildingPile1 = SerializeBuildingPile(Program.BuildingPile1),
                BuildingPile2 = SerializeBuildingPile(Program.BuildingPile2),
                BuildingPile3 = SerializeBuildingPile(Program.BuildingPile3),
                BuildingPile4 = SerializeBuildingPile(Program.BuildingPile4),
                Talon = SerializeTalon(Program.Talon)
            };

            if (name == null)
            {
                name ??= Guid.NewGuid().ToString().Replace("-", "").Substring(0, 4);
                while (SaveExists(name))
                {
                    name ??= Guid.NewGuid().ToString().Replace("-", "").Substring(0, 4);
                }
            }

            using (MemoryStream ms = new())
            {
                ProtoBuf.Serializer.Serialize(ms, s);

                using (FileStream fs = File.Open(Path.Combine(saveDirectory, name + $".{SAVESUFFIX}"), FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.CopyTo(fs);
                }
            }

            return new()
            {
                Filename = name
            };
        }

        public static LoadGameResponse LoadState(string name)
        {
            if (!SaveExists(name))
            {
                return new()
                {
                    Filename = name
                };
            }

            using (MemoryStream ms = new())
            {
                using (FileStream fs = File.Open(Path.Combine(saveDirectory, name + $".{SAVESUFFIX}"), FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    fs.CopyTo(ms);
                }
                ms.Seek(0, SeekOrigin.Begin);

                var ss = ProtoBuf.Serializer.Deserialize<SaveState>(ms);

                Program.TalonOpen.Clear();
                Program.Talon = JsonConvert.DeserializeObject<Deck>(Encoding.UTF8.GetString(Convert.FromBase64String(ss.Talon)), new JsonSerializerSettings() { ContractResolver = new SolitaireObjectsResolver() });
                Program.Pile1 = JsonConvert.DeserializeObject<TableuPile>(Encoding.UTF8.GetString(Convert.FromBase64String(ss.Pile1)), new JsonSerializerSettings() { ContractResolver = new SolitaireObjectsResolver() });
                Program.Pile2 = JsonConvert.DeserializeObject<TableuPile>(Encoding.UTF8.GetString(Convert.FromBase64String(ss.Pile2)), new JsonSerializerSettings() { ContractResolver = new SolitaireObjectsResolver() });
                Program.Pile3 = JsonConvert.DeserializeObject<TableuPile>(Encoding.UTF8.GetString(Convert.FromBase64String(ss.Pile3)), new JsonSerializerSettings() { ContractResolver = new SolitaireObjectsResolver() });
                Program.Pile4 = JsonConvert.DeserializeObject<TableuPile>(Encoding.UTF8.GetString(Convert.FromBase64String(ss.Pile4)), new JsonSerializerSettings() { ContractResolver = new SolitaireObjectsResolver() });
                Program.Pile5 = JsonConvert.DeserializeObject<TableuPile>(Encoding.UTF8.GetString(Convert.FromBase64String(ss.Pile5)), new JsonSerializerSettings() { ContractResolver = new SolitaireObjectsResolver() });
                Program.Pile6 = JsonConvert.DeserializeObject<TableuPile>(Encoding.UTF8.GetString(Convert.FromBase64String(ss.Pile6)), new JsonSerializerSettings() { ContractResolver = new SolitaireObjectsResolver() });
                Program.Pile7 = JsonConvert.DeserializeObject<TableuPile>(Encoding.UTF8.GetString(Convert.FromBase64String(ss.Pile7)), new JsonSerializerSettings() { ContractResolver = new SolitaireObjectsResolver() });
                Program.BuildingPile1 = JsonConvert.DeserializeObject<BuildingPile>(Encoding.UTF8.GetString(Convert.FromBase64String(ss.BuildingPile1)), new JsonSerializerSettings() { ContractResolver = new SolitaireObjectsResolver() });
                Program.BuildingPile2 = JsonConvert.DeserializeObject<BuildingPile>(Encoding.UTF8.GetString(Convert.FromBase64String(ss.BuildingPile2)), new JsonSerializerSettings() { ContractResolver = new SolitaireObjectsResolver() });
                Program.BuildingPile3 = JsonConvert.DeserializeObject<BuildingPile>(Encoding.UTF8.GetString(Convert.FromBase64String(ss.BuildingPile3)), new JsonSerializerSettings() { ContractResolver = new SolitaireObjectsResolver() });
                Program.BuildingPile4 = JsonConvert.DeserializeObject<BuildingPile>(Encoding.UTF8.GetString(Convert.FromBase64String(ss.BuildingPile4)), new JsonSerializerSettings() { ContractResolver = new SolitaireObjectsResolver() });

                Program.piles = new TableuPile[] { Program.Pile1, Program.Pile2, Program.Pile3, Program.Pile4, Program.Pile5, Program.Pile6, Program.Pile7 };
                Program.buildingPiles = new BuildingPile[] { Program.BuildingPile1, Program.BuildingPile2, Program.BuildingPile3, Program.BuildingPile4 };
            }

            return new()
            {
                Filename = name
            };
        }
    }
}
