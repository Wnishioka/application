using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVManager
{
    //区切り文字
    public char delim = ',';

    // CSVファイルを読み込む
    public List<string[]> CSVRead(string dataDir, string dataFile)
    {
        //ファイルパスを構築
        var filePath = dataDir + "/" + dataFile;

        //ディレクトリがあるか
        if (System.IO.Directory.Exists(dataDir))
        {
            //指定したパスのファイルがないか
            if (!System.IO.File.Exists(filePath))
            {
                //ファイル作成
                System.IO.File.Create(filePath);
                return new List<string[]>();
            }
        }
        else
        {
            //ディレクトリ作成
            System.IO.Directory.CreateDirectory(dataDir);

            //ファイル作成
            System.IO.File.Create(filePath);
            return new List<string[]>();
        }

        List<string[]> data = new List<string[]>();

        using (var dataText = new StreamReader(filePath))
        {
            while (dataText.Peek() > -1)
            {
                string line = dataText.ReadLine().Replace("\\n", "\n");
                data.Add(line.Split(delim));
            }
            return data;
        }
    }

    // CSVファイルに書きだす
    public void CSVWrite(List<string[]> writeDatas, string writeDataPath="write_data.csv", bool isAppend=false)
    {
        try
        {
            using (var sw = new System.IO.StreamWriter(writeDataPath, append: isAppend))
            {
                foreach (var data in writeDatas)
                {
                    sw.WriteLine(string.Join(",", data));
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public List<string[]> CSVReadFromResource(string resourcePath)
    {
        List<string[]> csvs = new List<string[]>();

        TextAsset rawText = Resources.Load(resourcePath) as TextAsset;
        StringReader reader = new StringReader(rawText.text);

        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine().Replace("\\n", "\n");
            csvs.Add(line.Split(delim)); // リストに入れる
        }

        return csvs;
    }
}
