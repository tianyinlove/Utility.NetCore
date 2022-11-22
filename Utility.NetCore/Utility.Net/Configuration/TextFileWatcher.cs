namespace Utility.Configuration
{
    /// <summary>
    /// 文本内容监听
    /// </summary>
    public class TextFileWatcher : FileWatcher<string>
    {
        /// <summary>
        /// 文本内容监听
        /// </summary>
        /// <param name="fileFullName">文本文件完整路径</param>
        /// <param name="delayMillisecond">变更生效的延时时间</param>
        public TextFileWatcher(string fileFullName, int delayMillisecond = 2000) :
            base(fileFullName, d => d, () => null, delayMillisecond)
        {
        }
    }
}
