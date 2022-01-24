using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;

using Lucene.Net.QueryParsers.Classic;

namespace LuenceDemoConsoleApp 
{
    public class Program
    { 
        
        const LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;
        public static void Main(string[] args)
        {
            // Construct a machine-independent path for the index
            var basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var indexPath = Path.Combine(basePath, "index");
            System.IO.Directory.Delete(indexPath, true);
            using var dir = FSDirectory.Open(indexPath);
            // Create an analyzer to process the text
            var analyzer = new StandardAnalyzer(AppLuceneVersion);
            // Create an index writer
            List<Dictionary<string, string>> fldTextList = new List<Dictionary<string, string>>();
            JsonFileHelper jsonFileHelper = new JsonFileHelper("documents");
            var dataList = jsonFileHelper.ReadList<News>("data");
            foreach (var data in dataList)
            {
                Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                valuePairs.Add("guid", Convert.ToString(data.GUID));
                valuePairs.Add("title", Convert.ToString(data.Title));
                valuePairs.Add("content", Convert.ToString(data.Content));
                fldTextList.Add(valuePairs);
            }
            var indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer);
            using var writer = new IndexWriter(dir, indexConfig);
            foreach (var fldText in fldTextList)
            {
                Document doc = new Document();
                foreach (var each in fldText)
                {
                    if (each.Key.ToLower().Contains("id") || each.Key.ToLower().Contains("guid"))
                    {
                        doc.Add(new Field(each.Key, each.Value, Field.Store.YES, Field.Index.NOT_ANALYZED));
                    }
                    else
                    {
                        doc.Add(new Field(each.Key, each.Value, Field.Store.YES, Field.Index.ANALYZED));
                    }
                }
                writer.AddDocument(doc);
            }
            writer.Commit();
            writer.Dispose();

            var source = new{
                                Name = "Kermit the Frog",
                                FavoritePhrase = "The quick brown fox jumps over the lazy dog"
                            };
           
            Console.WriteLine("Please input the keyword you want to search");
            var  keyword = Console.ReadLine().Trim();
            while(true)
                {

                Dictionary<string, string> queryFields = new Dictionary<string, string>();
                queryFields.Add("title", keyword);
                queryFields.Add("content", keyword);
                IndexSearcher isearcher;
                var directory = FSDirectory.Open(new DirectoryInfo(indexPath));

                // Now search the index:
                IndexReader ir = DirectoryReader.Open(directory);
                isearcher = new IndexSearcher(ir);

                string[] queries = queryFields.Values.ToArray();
                string[] fields = queryFields.Keys.ToArray();

                Query query = MultiFieldQueryParser.Parse(AppLuceneVersion, queries, fields, analyzer);

                int resultCounts = 10;
                var topDocs = isearcher.Search(query, resultCounts);
                ScoreDoc[] hits = topDocs.ScoreDocs;
                foreach (var hit in hits)
                {
                    Document hitDoc = isearcher.Doc(hit.Doc);
                    var content = hitDoc.Get("content");
                    var title = hitDoc.Get("title");
                    var guid = hitDoc.Get("guid");
                    var score = hit.Score;
                    Console.WriteLine("Title:  {0} ",  title);
                    Console.WriteLine("Content:  {0} ", content);
                    Console.WriteLine("Score:  {0}", score);
                }
                Console.WriteLine("Please input the keyword you want to search");
                keyword = Console.ReadLine().Trim();

            }

        }
    }
}
