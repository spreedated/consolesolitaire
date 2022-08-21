#pragma warning disable S3011

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using neXn.Lib.Playingcards.Classes;
using neXn.Lib.Playingcards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConsoleSolitaire.Classes.Json
{
    internal class SolitaireObjectsResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            List<JsonProperty> props = null;

            if (type == typeof(Deck))
            {
                props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.Name == "Carddeck" || x.Name == "PoppedCards" || x.Name == "Options").Select(f => base.CreateProperty(f, memberSerialization)).ToList();
                props.ForEach(p => { p.Writable = true; p.Readable = true; });
                return props.ToList();
            }

            if (type == typeof(TableuPile) || type == typeof(BuildingPile))
            {
                props = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(x => x.Name == "pile").Select(f => base.CreateProperty(f, memberSerialization)).ToList();
                props.ForEach(p => { p.Writable = true; p.Readable = true; });
                return props.ToList();
            }

            if (type == typeof(Card))
            {
                props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.Name == "Value" || x.Name == "Suit" || x.Name == "Decktype" || x.Name == "Hidden").Select(f => base.CreateProperty(f, memberSerialization)).ToList();
                props.ForEach(p => { p.Writable = true; p.Readable = true; });
                return props.ToList();
            }

            props = type.GetProperties().Select(p => base.CreateProperty(p, memberSerialization)).Union(type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Select(f => base.CreateProperty(f, memberSerialization))).ToList();

            props.ForEach(p => { p.Writable = true; p.Readable = true; });
            return props.ToList();
        }
    }
}
