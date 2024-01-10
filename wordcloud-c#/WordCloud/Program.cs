using MeCab;

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