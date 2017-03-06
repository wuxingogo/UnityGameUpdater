
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace wuxingogo.bundle
{

    public class XBundleBrowserWin : XBaseWindow
    {

        List<FileInfo> fileSet = new List<FileInfo>();

        AssetBundle currBundle = null;
        Object[] currAssets = null;

        bool isShowBundle = false;

        public override void OnXGUI()
        {
            base.OnXGUI();

            DoButton( "BundleManager Window", () =>
             {
                 var window = InitWindow<XBundleManagerWin>();
                 window.position = this.position;
                 this.Close();
             } );

            DoButton( "Browse Resources ", () =>
             {
                 fileSet.Clear();
                 ListFiles( new DirectoryInfo( "Assets/Resources/" + BundleConfig.bundleRelativePath ) );
             } );


            DoButton( "Browse AssetBundles", () =>
             {
                 fileSet.Clear();
                 ListFiles( new DirectoryInfo( "AssetBundles/" ) );
             } );


            CreateSpaceBox();

            if( !isShowBundle )
            {
                for( int i = 0; i < fileSet.Count; i++ )
                {
                    DoButton( fileSet[i].Name, () =>
                    {
                        FileStream stream = fileSet[i].OpenRead();
                        byte[] buffer = new byte[stream.Length];
                        stream.Read( buffer, 0, Convert.ToInt32( stream.Length ) );
                        currBundle = LoadAssetBundle( buffer );
                        isShowBundle = true;
                        currAssets = null;
                    } );
                }
            }
            else
            {
                if( currAssets == null )
                {
                    currAssets = currBundle.LoadAllAssets();
                }
                for( int i = 0; i < currAssets.Length; i++ )
                {
                    CreateObjectField( currAssets[i] );
                }

                DoButton( "Clear", () =>
                 {
                     isShowBundle = false;
                     currBundle.Unload( true );
                     currBundle = null;
                 } );
            }
            

            CreateSpaceBox();
        }

        AssetBundle LoadAssetBundle(byte[] memory )
        {
            AssetBundle bundle = null;
            using( var bundleStream = BundleEncode.DeompressAndDecryptLZMA( memory, BundleConfig.password ) )
            {
                if( bundleStream == null )
                    return bundle;
                bundle = AssetBundle.LoadFromMemory( bundleStream.ToArray() );
            }
            return bundle;
        }

        void OnDisable()
        {
            if( currBundle != null )
            {
                currBundle.Unload( true );
                currBundle = null;
            }
        }


        void ListFiles( FileSystemInfo info )
        {
            if( !info.Exists )
                return;

            DirectoryInfo dir = info as DirectoryInfo;
            if( dir == null )
                return;

            FileSystemInfo[] files = dir.GetFileSystemInfos();
            for( int i = 0; i < files.Length; i++ )
            {
                FileInfo file = files[i] as FileInfo;

                if( file != null && file.Name.EndsWith( BundleConfig.suffix ) && file.Name != BundleConfig.versionFileName + BundleConfig.suffix)
                    fileSet.Add( file );
                else
                    ListFiles( files[i] );

            }
        }

    }
}

