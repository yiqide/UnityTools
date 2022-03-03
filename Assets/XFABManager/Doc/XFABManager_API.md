## XFABManager API

### AssetBundleManager 类

#### static CheckResUpdateRequest CheckResUpdate(string projectName)

	描述：

		检测某一个模块的资源,可以根据检测的结果判断是否需要更新、下载、释放等等...

	参数：

		projectName: 模块名

	返回值：

		返回值是一个 CheckResUpdateRequest 的对象, 该对象继承 CustomAsyncOperation , 所以在协程中是可等待的,
		可以通过 CustomAsyncOperation 中的 isDone 属性来判断是否执行完成,progress 来获取进度, completed 来注册完成的回调,error 来获取是否出错
		在 CheckResUpdateRequest 中有一个 result 的属性,可以通过这个字段来获取到检测的结果,这个字段的内容如下:
		
		public class CheckUpdateResult
		{
			// 更新的类型
			public UpdateType updateType;
			
			// 需要更新的文件总大小 单位/字节
			public long updateSize;
			
			// 更新的内容
			public string message = string.Empty;

			// 项目名
			public string projectName;
			
			// 需要更新的文件列表
			public BundleInfo[] need_update_bundles;

			// 当前的版本
			public string version;
		}
		
	
#### static CheckResUpdatesRequest CheckResUpdates(string projectName)

	描述：

		检测某一个模块 及其 依赖模块 的资源,

	参数：

		projectName: 模块名

	返回值：

		CheckResUpdatesRequest 里面的 results 是 result 的数组!
		
		
#### static void ClearProjectCache(string projectName)

	描述：

		清空某一个模块在本地的所有资源文件

	参数：

		projectName: 模块名

	返回值: 无
	
	
#### static void GetProjectCacheSize(string projectName)

	描述：

		获取某一个资源模块 在本地的所有资源大小

	参数：

		projectName: 模块名

	返回值: 类型 : long 资源大小 单位:字节
	

#### static DownloadOneAssetBundleRequest DownloadOneAssetBundle(string projectName, string bundleName)

	描述：

		下载某一个模块中某一个AssetBundle和它依赖的AssetBundle !

	参数：

		projectName: 模块名
		bundleName: 要下载的bundle的名称

	返回值: DownloadOneAssetBundleRequest 同样也是继承 CustomAsyncOperation , 可等待的,用法同上!
			此外 DownloadOneAssetBundleRequest 有一个 Speed 的属性,表示当前的下载速度,单位:字节/秒
			
	
#### static ExtractResRequest ExtractRes(string projectName)

	描述：

		释放AssetBunle ( 从 StreamingAssets 把bundle文件复制到 persistentDataPath ) 
        仅释放当前项目资源 不包含其依赖项目
		
		*注:该方法在释放前会先进行资源检测,只有当检测结果类型为 UpdateType.ExtractRes 才会进行释放,否则不会处理任何逻辑!
			*检测结果取决于本地资源 和 配置,具体请参考[资源检测逻辑](Assets/XFABManager/Doc/XFABManager教程.md)
		
	重载函数:
			
			static ExtractResRequest ExtractRes(CheckUpdateResult result)
			
			描述:
				直接根据检测的结果来进行释放 , 需要自己手动检测!
		
	参数：

		projectName: 模块名

	返回值: ExtractResRequest 继承 CustomAsyncOperation , 可等待的,用法同上!
			
			
#### static string[] GetAssetBundleDependences(string projectName, string bundleName)

	描述：

		获取某一个AssetBundle的依赖的AssetBundle的名称

	参数：

		projectName: 模块名
		bundleName:  bundle的名称

	返回值: string[] 依赖的AssetBundle的数组!
			 
			 
#### static string GetAssetBundlePath(string projectName, string bundleName, string suffix = "")

	描述：

		获取某一个AssetBundle本地所在的路径

	参数：

		projectName:	模块名
		bundleName:  	bundle的名称
		suffix:			AssetBundle的后缀

	返回值: string 路径
	
	
#### static string GetAssetBundleSuffix(string projectName)

	描述：

		获取某一个模块AssetBundle的后缀
		*注:前提是本地要有这个模块的资源,如果没有是获取不到的!

	参数：

		projectName:	模块名

	返回值: string 后缀
	
	
#### static Profile GetProfile(string projectName = "")

	描述：

		获取某一个模块的配置数据

	参数：

		projectName:	模块名	// 可以不传 , 如果不传会认为获取默认配置

	返回值: Profile 具体的配置数据,内容如下:
	
	
			public class Profile
			{
				public string url = string.Empty;						//	资源更新地址

				public UpdateMode updateModel = UpdateMode.LOCAL;		//	更新模式 默认 DEBUG

				public LoadMode loadMode = LoadMode.Assets;				//	加载模式 默认 Assets

				public bool useDefaultGetProjectVersion = true;			//	是否使用默认的 获取项目版本
			}
			
	
#### static GetProjectDependenciesRequest GetProjectDependencies(string projectName)

	描述：

		获取某个项目的依赖了哪些项目

	参数：

		projectName:	模块名

	返回值: GetProjectDependenciesRequest 可以通过属性 dependencies  来获取到依赖了哪些模块!
	
	
#### static IsHaveBuiltInResRequest IsHaveBuiltInRes(string projectName )

	描述：

		判断安装包中是否内置了某个模块的资源

	参数：

		projectName:	模块名

	返回值: IsHaveBuiltInResRequest 中有一个 isHave 的属性 , 可以用来判断是否内置
	

#### static bool IsHaveResOnLocal(string projectName)

	描述：

		判断本地是否有某个模块的资源

	参数：

		projectName:	模块名

	返回值: bool 如果有返回true, 没有则返回false;
	
	
#### static bool IsLoadedAssetBundle(string projectName, string bundleName)

	描述：

		判断是否已经加载 某个AssetBundle
        如果是 编辑器模式 并且从 Assets 加载资源 一直返回True

	参数：

		projectName:	模块名
		bundleName:		bundle名称
		

	返回值: bool 如果已经加载了某个AssetBundle 返回true 否则 返回false
	
	
#### static LoadAllAssetBundlesRequest LoadAllAssetBundles(string projectName)

	描述：

		加载某个模块所有的AssetBundle 

	参数：

		projectName:	模块名

	返回值: LoadAllAssetBundlesRequest 可等待的,同上!
	
	
#### static UnityEngine.Object[] LoadAllAssets(string projectName, string bundleName)

	描述：

		加载某个bundle所有的资源

	参数：

		projectName:	模块名
		bundleName:		bundle名称

	返回值: 返回所有资源的数组 UnityEngine.Object[]
	
	
#### static LoadAssetsRequest LoadAllAssetsAsync(string projectName, string bundleName)

	描述：

		异步加载某个bundle所有的资源

	参数：

		projectName:	模块名
		bundleName:		bundle名称

	返回值: 加载完成之后可以通过 LoadAssetsRequest 中的 assets 获取到加载的结果!
	
	
		
#### static T LoadAsset<T>(string projectName, string bundleName, string assetName) where T : UnityEngine.Object

	描述：

		从某个bundle中加载指定资源

	参数：

		projectName:	模块名
		bundleName:		bundle名称
		assetName:		资源名称
		T:				要加载的资源类型
	
	重载函数:
	
			static UnityEngine.Object LoadAsset(string projectName, string bundleName, string assetName, Type type)
			
			描述:
				功能相同,T 参数变成了 type !

	返回值: 返回 T 类型的资源,如果加载失败返回null!
	
	
#### static LoadAssetRequest LoadAssetAsync<T>(string projectName, string bundleName, string assetName) where T : UnityEngine.Object

	描述：

		异步从某个bundle中加载指定资源

	参数：

		projectName:	模块名
		bundleName:		bundle名称
		assetName:		资源名称
		T:				要加载的资源类型
		
	重载函数:
			LoadAssetRequest LoadAssetAsync(string projectName, string bundleName, string assetName, Type type)
			
			描述:
				功能相同,T 参数变成了 type !
		
	返回值: LoadAssetRequest 中有一个 asset 的属性,可以通过这个属性获取到资源
	
	
#### static T[] LoadAssetWithSubAssets<T>(string projectName, string bundleName, string assetName) where T : UnityEngine.Object

	描述：

		加载资源和它的子资源

	参数：

		projectName:	模块名
		bundleName:		bundle名称
		assetName:		资源名称
		T:				要加载的资源类型
		
	重载函数:
			static UnityEngine.Object[] LoadAssetWithSubAssets(string projectName, string bundleName, string assetName, Type type)
			
			描述:
				功能相同,T 参数变成了 type !
		
	返回值: 返回 T 类型的资源数组,如果加载失败返回null!
	
	
	
#### static LoadAssetsRequest LoadAssetWithSubAssetsAsync<T>(string projectName, string bundleName, string assetName) where T : UnityEngine.Object

	描述：

		异步加载资源和它的子资源

	参数：

		projectName:	模块名
		bundleName:		bundle名称
		assetName:		资源名称
		T:				要加载的资源类型
		
	重载函数:
			static LoadAssetsRequest LoadAssetWithSubAssetsAsync(string projectName, string bundleName, string assetName, Type type)
			
			描述:
				功能相同,T 参数变成了 type !
		
	返回值: LoadAssetsRequest 中可以获取到对应的数据
	
	
#### static void LoadScene(string projectName, string bundleName, string sceneName, LoadSceneMode mode)

	描述：

		加载某一个bundle中的场景!

	参数：

		projectName:	模块名
		bundleName:		bundle名称
		sceneName:		场景名
		mode:			加载场景的模式 Single 还是 Additive
		
		
	返回值: 无
	
	
#### static AsyncOperation LoadSceneAsync(string projectName, string bundleName, string sceneName, LoadSceneMode mode)

	描述：

		加载某一个bundle中的场景!

	参数：

		projectName:	模块名
		bundleName:		bundle名称
		sceneName:		场景名
		mode:			加载场景的模式 Single 还是 Additive
		
		
	返回值: AsyncOperation
	
	
#### static ReadyResRequest ReadyRes(string projectName)

	描述：

		准备某个模块的资源,这个方法会自行检测需要做什么处理,比如:更新 下载 或者 释放... 
		当这个方法执行完成之后,代表这个模型的资源已经准备完成,可以进行资源加载了!

	参数：

		projectName:	模块名
	
	重载函数:
	
			static ReadyResRequest ReadyRes(CheckUpdateResult[] results)
			
			描述:
				自己手动检测,根据检测的结果来准备资源!
		
	返回值: ReadyResRequest , 可以通过 ReadyResRequest 获取到进度 和 结果! 方式同上!
	
	
	
#### static void SetGetProjectVersion<T>() where T : IGetProjectVersion

	描述：

		设置获取版本的接口 , 自定义获取项目版本号时,需要通过这个方法来设置!

	参数：

		T:	类型
	
	重载函数:
			
			static void SetGetProjectVersion(Type type) // T 类型 参数改为 : Type 
	
			static void SetGetProjectVersion(IGetProjectVersion getProjectVersion) // 已过时,不推荐使用,将会在未来的版本中移除
		
	返回值: 无
	
	
#### static void SetServerFilePath(IServerFilePath serverFilePath)

	描述：

		设置服务端文件路径接口 
		*注:此方法要在 AssetBundleManager 初始化完成之后调用 才会生效!
		
		具体使用方法可以参考视频: https://www.bilibili.com/video/BV1uX4y1w7M9?p=6

	参数：

		serverFilePath:	实现了 IServerFilePath 这个接口对象

	返回值: 无
	
		
#### static void UnLoadAllAssetBundles(string projectName, bool unloadAllLoadedObjects = true)

	描述：

		卸载某个资源模块的所有AssetBundle

	参数：

		projectName:				模块名
		unloadAllLoadedObjects:		是否卸载已经加载的资源,建议传true

	返回值: 无
	
	
#### static void UnLoadAssetBundle(string projectName, string bundleName)

	描述：

		卸载某个模块中的某个AssetBundle 
		*注:不会卸载依赖的AssetBundle,后面会考虑加入依赖bundle引用计数功能,来对依赖的bundle及时清理!

	参数：

		projectName:	模块名
		bundleName:		bundle名称

	返回值: 无
	
	
#### static UpdateOrDownloadResRequest UpdateOrDownloadRes(string projectName)

	描述：
	
		更新或下载某个模块的资源

	参数：

		projectName:	模块名
		
	重载函数:
	
			static UpdateOrDownloadResRequest UpdateOrDownloadRes(CheckUpdateResult result)
			
			描述:
				自己手动检测,根据检测的结果来更新或下载资源!
				这种方式可以把需要更新的内容,大小 等信息展示给用户,让用户来决定要不要更新!
			

	返回值: UpdateOrDownloadResRequest 可以等待的,同上!



### XFABTools 类

#### static string BuildInDataPath(string projectName)

	描述：

		某一个模块的内置资源的目录

	参数：

		projectName: 模块名

	返回值：string 路径
	
	
#### static FileMD5Request CaculateFileMD5(string path)

	描述：

		计算一个文件的md5值

	参数：

		path: 文件路径

	返回值：通过 FileMD5Request 中的 md5 属性 来获取计算的结果!


#### static string DataPath(string projectName)

	描述：

		某一个模块的 AssetBundle文件本地存放目录 

	参数：

		projectName: 模块名

	返回值：string 路径
	
	
#### static string GetCurrentPlatformName()

	描述：

		获取当前平台的名称

	参数: 无

	返回值：string 名称
	

#### static bool IsBaseByClass(Type source, Type target)

	描述：

		判断一个类 是否 继承另外一个类

	参数: 
	
		source : 源类型 
		target : 目标类型 
	

	返回值：bool 如果 source 继承自 target  返回true, 否则返回false
	
	
#### static bool IsImpInterface(Type source,Type target)

	描述：

		判断一个类 是否 实现了某个接口

	参数: 
	
		source : 源类型 
		target : 目标接口类型 
	

	返回值：bool 如果 source 实现了接口 target  返回true, 否则返回false
	
	
#### static string LocalResPath(string projectName, string fileName)

	描述：

		某个AssetBundle在本地的文件路径 ( 数据目录 )

	参数: 
	
		projectName : 模块名 
		fileName 	: 文件名 
	

	返回值：string 路径
	
	
#### static string md5(string source)

	描述：

		计算字符串的MD5值

	参数: 
	
		source : 字符串 

	返回值：string md5值


#### static string md5file(string file)

	描述：

		计算文件的MD5值

	参数: 
	
		file : 文件路径 

	返回值：string md5值	
	
	
### DownloadFileRequest 类

#### static DownloadFileRequest Download(string file_url, string localfile, long length = 0)

	描述：

		下载一个文件

	参数：

		file_url	: 	文件的网络路径	
		localfile	:	下载后存放的本地路径
		length		:	文件的大小

	返回值：DownloadFileRequest , 可以通过返回值获取到 Speed 下载速度
	

### DownloadFilesRequest 类

#### static DownloadFilesRequest DownloadFiles(List<DownloadObjectInfo> files)

	描述：

		下载多个文件

	参数：

		files	: 	要下载的文件的信息集合	


	返回值：DownloadFilesRequest , 可以通过返回值获取到 Speed 下载速度
	
	
### FileTools 类

#### static CopyFileRequest Copy(string source, string des)

	描述：

		复制一个文件

	参数：

		source : 源文件地址
		des	   : 要复制到的目标地址


	返回值：CopyFileRequest 可等待的!
	
	
#### static bool CopyDirectory(string sourceDir, string destDir, Action<string, float> progress,string[] excludeSuffix = null)

	描述：

		复制一个文件夹所有的内容

	参数：

        sourceDir		:	源文件夹
        destDir			:	目标文件夹
        progress		:	进度改变的回调 第一个参数是正在复制的文件名称 第二个是复制的进度
        excludeSuffix	:	不需要复制的文件的后缀

	返回值：bool 如果返回true 复制成功 否则 失败
	
	
#### static void DeleteDirectory(string dirPath)

	描述：

		删除一个文件夹

	参数：

        dirPath		:	文件夹路径

	返回值：无
	
	
#### static long GetDirectorySize(string dirPath)

	描述：

		获取文件夹中所有文件的大小

	参数：

        dirPath		:	文件夹路径

	返回值：long 文件大小 , 单位:字节
	
	
### CoroutineStarter 类

#### static Coroutine Start(IEnumerator enumerator)

	描述：

		开启协程

	参数：

		enumerator : 迭代器


	返回值：Coroutine 协程
	
	
#### static void Stop(Coroutine coroutine)

	描述：

		停止协程

	参数：

		coroutine : 协程


	返回值：无
	
	
### ZipTools 类

#### static bool CreateZipFile(string filesPath, string zipFilePath)

	描述：

		压缩文件

	参数：

        filesPath		:	要压缩的文件夹路径
        zipFilePath		:	压缩后的zip文件路径


	返回值：bool 如果返回true 则代表压缩成功,否则压缩失败
	
	
#### static bool UnZipFile(string zipFilePath,string targetDirectory)

	描述：

		解压zip文件

	参数：

        zipFilePath			:	压缩后的zip文件路径
        targetDirectory		:	要解压的目标路径

	返回值：bool 如果返回true 则代表压缩成功,否则压缩失败
	
	
	
	

### 如有疑问 或 遗漏请及时联系群主,qq交流群:1058692748