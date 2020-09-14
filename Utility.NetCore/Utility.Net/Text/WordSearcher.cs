using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.NetCore.Text
{
    /// <summary>
    /// 关键词搜索
    /// </summary>
    public class WordSearcher
    {
        Dictionary<char, SearchNode> _dict = null;
        List<char> _ignoreCharList = null;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="words">所有关键词</param>
        /// <param name="ignoreCharList">需要忽略的字符</param>
        public void Init(List<string> words, List<char> ignoreCharList = null)
        {
            var dict = new Dictionary<char, SearchNode>();
            foreach (var word in words.Where(d => !string.IsNullOrWhiteSpace(d)))
            {
                var keyword = word.ToLower();
                if (!dict.TryGetValue(keyword[0], out SearchNode pointer))
                {
                    dict[keyword[0]] = pointer = new SearchNode();
                }
                for (int keywordPos = 1; keywordPos < keyword.Length; keywordPos++)
                {
                    var c = keyword[keywordPos];
                    if (ignoreCharList != null && ignoreCharList.Contains(c))
                    {
                        continue;
                    }

                    if (!pointer.Children.TryGetValue(c, out SearchNode nextNode))
                    {
                        pointer.Children[c] = nextNode = new SearchNode();
                    }

                    pointer = nextNode;
                }
                if (pointer.Word == null)
                {
                    pointer.Word = keyword;
                }
            }
            _dict = dict;
            _ignoreCharList = ignoreCharList;
        }

        /// <summary>
        /// 搜索一段文字当中的关键词
        /// </summary>
        /// <param name="content">需要搜索的文字</param>
        /// <param name="limit">数量限制，找到相应数量敏感词的时候立即返回，0=无限数量</param>
        /// <returns></returns>
        public List<string> Match(string content, int limit)
        {
            if (_dict == null)
            {
                throw new Exception("关键词未初始化");
            }
            var dict = _dict;

            content = content.ToLower();
            List<string> matchWords = new List<string>();
            Dictionary<int, SearchNode> preMatches = new Dictionary<int, SearchNode>(); //暂时匹配的字符位置和指针
            for (int contentPosition = 0; contentPosition < content.Length; contentPosition++)
            {
                var c = content[contentPosition];
                if (_ignoreCharList != null && _ignoreCharList.Contains(c))
                {
                    continue;
                }

                var keys = preMatches.Keys.ToList();
                foreach (var prematchPos in keys)
                {
                    if (preMatches[prematchPos].Children.TryGetValue(c, out SearchNode nextNode))
                    {
                        preMatches[prematchPos] = nextNode;
                        if (nextNode.Word != null)  //找到一个
                        {
                            matchWords.Add(nextNode.Word);
                            if (limit > 0 && matchWords.Count >= limit)
                            {
                                preMatches.Clear();
                                return matchWords;
                            }
                        }
                    }
                    else
                    {
                        preMatches.Remove(prematchPos);  //确认无法匹配
                    }
                }

                if (dict.TryGetValue(c, out SearchNode newNode))
                {
                    preMatches[contentPosition] = newNode;
                    if (newNode.Word != null)
                    {
                        matchWords.Add(newNode.Word);
                        if (limit > 0 && matchWords.Count >= limit)
                        {
                            preMatches.Clear();
                            return matchWords;
                        }
                    }
                }
            }

            preMatches.Clear();
            return matchWords;
        }
    }

    /// <summary>
    /// 查找节点
    /// </summary>
    internal class SearchNode
    {
        /// <summary>
        /// 子节点
        /// </summary>
        public Dictionary<char, SearchNode> Children { get; set; } = new Dictionary<char, SearchNode>();

        /// <summary>
        /// 查到的数据
        /// </summary>
        public string Word { get; set; }
    }
}
