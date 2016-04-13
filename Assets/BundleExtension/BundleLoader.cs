//
//  BundleLoader.cs
//
//  Author:
//       ${wuxingogo} <52111314ly@gmail.com>
//
//  Copyright (c) 2016 ly-user
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using UnityEngine;
using System.Collections;
using System.IO;
using wuxingogo.bundle;
using LitJson;
using System;

public class BundleLoader
{
	public static VersionConfig LoadLocalVersion (string relativePath)
	{
		var str = LoadFileText (relativePath);
		VersionConfig bundleConfig = JsonMapper.ToObject<VersionConfig> (str);

		if (!Directory.Exists (Application.temporaryCachePath + "/" + bundleConfig.bundleRelativePath)) {
			Directory.CreateDirectory (Application.temporaryCachePath + "/" + bundleConfig.bundleRelativePath);
		}

		return bundleConfig;
	}

	public static void UpdateVersionConfig(VersionConfig localVersionConfig)
	{
		File.WriteAllText(Application.temporaryCachePath + "/" + localVersionConfig.bundleRelativePath, JsonMapper.ToJson (localVersionConfig));
	}

	public static byte[] LoadFileMemory (string relativePath)
	{
		if (File.Exists (Application.temporaryCachePath + "/" + relativePath)) {
			return File.ReadAllBytes (Application.temporaryCachePath + "/" + relativePath);
		} else {
			if(relativePath.Contains (".")){
				relativePath = relativePath.Substring (0, relativePath.LastIndexOf ("."));
			}
			var objInResources = Resources.Load (relativePath, typeof( TextAsset )) as TextAsset;
			var memory = objInResources.bytes;
			Resources.UnloadAsset (objInResources);
			return memory;
		}
	}

	public static string LoadFileText (string relativePath)
	{
		if (File.Exists (Application.temporaryCachePath + "/" + relativePath)) {
			return File.ReadAllText (Application.temporaryCachePath + "/" + relativePath);
		} else {
			if(relativePath.Contains (".")){
				relativePath = relativePath.Substring (0, relativePath.LastIndexOf ("."));
			}
            var objResources = Resources.Load<TextAsset>( relativePath );
            var text = objResources.text;
            Resources.UnloadAsset( objResources );
            return text;
		}
	}

    public static string LoadResourcesText( string path )
    {
        string result = string.Empty;
        string resourcePath = string.Format( @"Assets/Resources/{0}", path );
        if( File.Exists( resourcePath ) )
        {
            result = File.ReadAllText( resourcePath );
        }
        return result;
    }

	private static string LoadMemoryString (string relativePath)
	{
		return BitConverter.ToString(LoadFileMemory (relativePath));
	}
}

