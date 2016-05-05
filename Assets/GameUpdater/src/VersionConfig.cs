using System.Collections.Generic;
using System.Collections;

namespace newx
{
	[System.Serializable]
    public class VersionConfig
    {
        public string versionNum;
        public string bundleRelativePath;
        public List<BundleInfo> bundles;
    }

	[System.Serializable]
    public class BundleInfo
    {
        public string name;
        public string md5;
        public uint size;
        public string[] include;
        public string[] dependency;
    }

}
