//
//  BundleConfig.cs
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

namespace wuxingogo.bundle
{
	public class BundleConfig
	{
		public static string bundleRelativePath = "publish";
		public static string versionFileName = "vc";
		public static string bundlePoolRelativePath = "AssetBundlePool";
		public static bool compress = true;
		public static string password = "password";
		public static string suffix = ".bytes";
		public static readonly string resourcesPath = Application.dataPath + "/Resources";
		
	}
}
