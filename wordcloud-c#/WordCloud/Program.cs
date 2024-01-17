using MeCab;
using AngleSharp;
using System.Text;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System.Text.RegularExpressions;

List<string> urls = [
    "https://www.alpine.co.jp/solution/productandservice/rentalcar/column/20230201-01",
    "https://s-landusage.jp/post-1224/",
    "https://j-net21.smrj.go.jp/startup/research/service/cons-rentacycle.html",
    "https://at-living.press/life/19490/",
    "https://note.com/estyle_blog/n/n8a78af55ac68",
    "https://www.waseda.jp/sem-fox/memb/12s/mutou/mutou.index.html"
];

List<string> rmElementName = ["header", "footer", "script", "noscript", "nav", "button", "style", "iframe", "figure", "img"];

// この辺で色々設定する
var config = Configuration.Default.WithDefaultLoader();

// Headless Browser的なものを作る
using var context = BrowsingContext.New(config);

var builder = new StringBuilder();
foreach (var url in urls)
{
    var doc = await context.OpenAsync(url);
    if (doc.Body is null)
    {
        continue;
    }
    builder.AppendLine(url);
    var deleteElements = rmElementName.SelectMany(name => doc.Body.QuerySelectorAll(name)).ToList();
    foreach (var deleteElement in deleteElements){
        deleteElement.Remove();
    }
    var comments = doc.Body.ChildNodes.Where(node => node is IComment).ToList();
    foreach (var comment in comments)
    {
        doc.Body.RemoveChild(comment);
    }
    var spanElements = doc.Body.QuerySelectorAll<IHtmlSpanElement>("span");
    /*foreach  (var spanElement in spanElements){
        spanElement.Replace([.. spanElement.ChildNodes]);
    }*/
    foreach (var element in doc.Body.QuerySelectorAll("*")){
        comments = element.ChildNodes.Where(node => node is IComment).ToList();
        foreach (var comment in comments)
        {
            element.RemoveChild(comment);
        }
        for(int i=0;i< element.Attributes.Length;i++){
            element.RemoveAttribute(element.Attributes[i]!.NamespaceUri,element.Attributes[i]!.Name);
        }
    }
    builder.AppendLine(doc.Body.Prettify());
}

var reg = new Regex(@"^\s*<.+>");
var html = builder.ToString();
html = reg.Replace(html,"");

using (var file = File.Create("web-html-body.txt"))
using (var writer = new StreamWriter(file))
{
    await writer.WriteAsync(html);
    await writer.FlushAsync();
}
/*
var sentence = "行く川のながれは絶えずして、しかももとの水にあらず。";

var parameter = new MeCabParam();
var tagger = MeCabTagger.Create(parameter);

foreach (var node in tagger.ParseToNodes(sentence))
{
    if (node.CharType > 0)
    {
        var features = node.Feature.Split(',');
        var displayFeatures = string.Join(", ", features);

        Console.WriteLine($"{node.Surface}\t{displayFeatures}");
    }
}
*/