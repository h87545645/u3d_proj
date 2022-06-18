public static  class GPath 
 {
     private static string _persistentAssetsPath = "cc";
     public static string PersistentAssetsPath
     {
         get { return _persistentAssetsPath;}
         set { _persistentAssetsPath = value;} 
    } 



     private static string _CDNUrl = "cc";
     public static string CDNUrl
     {
         get { return _CDNUrl;}
         set { _CDNUrl = value;} 
    } 

     private static string _versionFileName = "cc";
     public static string VersionFileName
     {
         get { return _versionFileName;}
         set { _versionFileName = value;} 
    } 

     private static string _streamingAssetsPath = "cc";
     public static string StreamingAssetsPath
     {
         get { return _streamingAssetsPath;}
         set { _streamingAssetsPath = value;} 
    } 
}