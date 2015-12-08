using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace CyberCube
{
    /// <summary>
    /// Manager singleton for opening and closing the storage container and files.
    /// 
    /// Developed by Morgan Wynne.
    /// </summary>
    public class StorageManager
    {
        /// <summary>
        /// The name of the container to load/save from within the save file.
        /// </summary>
        private string containerName = "cybercubesave";

        /// <summary>
        /// Singleton instance of the StorageManager class.
        /// </summary>
        private static readonly StorageManager instance = new StorageManager();

        /// <summary>
        /// 
        /// </summary>
        private StorageContainer container;

        /// <summary>
        /// 
        /// </summary>
        private IAsyncResult ShowSelectorResult;

        /// <summary>
        /// Represents a storage device for user data, such as a memory unit or hard drive.
        /// </summary>
        private StorageDevice storageDevice;

        /// <summary>
        /// 
        /// </summary>
        private Stream fileStream = null;

        static StorageManager()
        {
        }
        private StorageManager()
        {
        }

        /// <summary>
        /// Singleton instance of the StorageManager class.
        /// </summary>
        public static StorageManager Instance
        { get { return instance; } }

        private void OpenContainer()
        {
            storageDevice = null;
            this.ShowSelectorResult = StorageDevice.BeginShowSelector( PlayerIndex.One, null, null );
            storageDevice = StorageDevice.EndShowSelector( this.ShowSelectorResult );
            if( storageDevice == null || !( storageDevice.IsConnected ) )
                throw new Exception( "StorageDevice was null or not connected." );

            IAsyncResult r = storageDevice.BeginOpenContainer( containerName, null, null );
            this.ShowSelectorResult.AsyncWaitHandle.WaitOne();

            this.container = storageDevice.EndOpenContainer( r );
        }

        /// <summary>
        /// Finalizes all saves and closes the container.
        /// </summary>
        public void Finish()
        {
            if( this.container.IsDisposed )
                return;

            this.fileStream.Close();
            this.fileStream = null;
            this.container.Dispose();
            this.ShowSelectorResult.AsyncWaitHandle.Close();
        }

        /// <summary>
        /// Opens a write stream to the designated file.
        /// </summary>
        /// <param name="fileName">The name of the file to open.</param>
        /// <returns>A write stream to the file.</returns>
        public Stream OpenWriteFile( string fileName )
        {
            // If a file stream is already open, close it.
            if( this.fileStream != null )
                this.fileStream.Close();

            // Ensures the container is open.
            if( this.container == null || this.container.IsDisposed )
                StorageManager.Instance.OpenContainer();

            // If the file already exists, delete it.
            if( container.FileExists( fileName ) )
                container.DeleteFile( fileName );

            this.fileStream = container.CreateFile( fileName );
            return this.fileStream;
        }

        public Stream OpenReadFile( string fileName )
        {
            // If a file stream is already open, close it.
            if( this.fileStream != null )
                this.fileStream.Close();

            // Ensures the container is open.
            if( this.container == null || this.container.IsDisposed )
                StorageManager.Instance.OpenContainer();

            // Throws an exception if the file does not already exist.
            if( !container.FileExists( fileName ) )
                throw new Exception( "File does not exist" );

            this.fileStream = container.OpenFile( fileName, FileMode.Open );
            return this.fileStream;
        }
    }
}
