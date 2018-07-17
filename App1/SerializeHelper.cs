﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace App1
{
    public class SerializeHelper
    {

        public static void SaveData<T>(string fileName, T dataToSave)
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {

                    if (store.FileExists(fileName))
                    {
                        store.DeleteFile(fileName);
                    }

                    using (IsolatedStorageFileStream stream = store.OpenFile(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                    {
                        var serializer = new DataContractSerializer(typeof(T));
                        serializer.WriteObject(stream, dataToSave);
                    }
                }
                catch (Exception e)
                {
                    //MessageBox.Show(e.Message);
                    return;
                }

            }
        }

        public static T ReadData<T>(string fileName)
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {

                if (store.FileExists(fileName))
                {
                    using (IsolatedStorageFileStream stream = store.OpenFile(fileName, FileMode.OpenOrCreate, FileAccess.Read))
                    {

                        try
                        {
                            var serializer = new DataContractSerializer(typeof(T));
                            return (T)serializer.ReadObject(stream);

                        }
                        catch (Exception)
                        {
                            return default(T);
                        }
                    }
                }
                return default(T);
            }
        }

    }
}
