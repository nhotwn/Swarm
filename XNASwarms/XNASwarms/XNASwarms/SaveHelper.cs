﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SwarmEngine;
using System.IO;
using System.Xml.Serialization;
using Windows.Storage;
using System.Threading.Tasks;
using WinRtUtility;


namespace XNASwarms
{
    public static class SaveHelper
    {
        #region Save
        public static void Save(string filename, SaveAllSpecies savespecies)
        {
#if WINDOWS
            try
            {
                Stream stream = File.Create(filename + ".xml");
                XmlSerializer serializer = new XmlSerializer(typeof(SaveAllSpecies));
                serializer.Serialize(stream, savespecies);
                stream.Close();
            }
            catch
            {
                //
            }
#elif NETFX_CORE
            SaveW8(filename, savespecies);
#endif

        }

        private static async void SaveW8(string filename, SaveAllSpecies savespecies)
        {
            await W8SaveFile(filename, savespecies);
        }

        static async Task<SaveAllSpecies> W8SaveFile(string filename, SaveAllSpecies data)
        {
            var objectStorageHelper = new ObjectStorageHelper<SaveAllSpecies>(StorageType.Local);
            await objectStorageHelper.SaveAsync(data, filename + ".xml");
            return data;
        }
        #endregion

        public static SaveAllSpecies Load(string filename)
        {
#if WINDOWS
            try
            {
                Stream stream = File.OpenRead(filename + ".xml");
                XmlSerializer serializer = new XmlSerializer(typeof(SaveAllSpecies));
                var item = (SaveAllSpecies)serializer.Deserialize(stream);
                stream.Close();
                return item;
            }
            catch
            {
                return null;
            }
#else
            return null;

#endif
        }

        public static async Task<SaveAllSpecies> LoadGameFile(string filename)
        {
            var save = await LoadFile(filename);
            if (save == null)
            {
                return new SaveAllSpecies();
            }
            else
            {
                return save;
            }


        }

        static async Task<SaveAllSpecies> LoadFile(string filename)
        {
            SaveAllSpecies data = new SaveAllSpecies();
            try
            {
                var objectStorageHelper = new ObjectStorageHelper<SaveAllSpecies>(StorageType.Local);
                data = await objectStorageHelper.LoadAsync(filename + ".xml");
                return data;
            }
            catch (Exception e)
            {
                //Shit
                return new SaveAllSpecies();
            }
        }

    }
}

