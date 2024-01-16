using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ContentData { } // 컨텐츠 데이터 정의
public abstract class ContentEntity { } // 컨텐츠 데이터 내의 각 엔티티 정의

public interface ContentDataHelper
{
    public string GetFilePath();
    public void Convert(ContentData dataBlcok);
    public ContentEntity GetContentEntity(int index);
    public int GetContentLength();
}

public class Content<DataDefinition, DataHelper>
    where DataDefinition : ContentData, new()
    where DataHelper : ContentDataHelper, new()
{
    public static ContentDataHelper Init()
    {
        var _helper = new DataHelper();

        TextAsset json = (TextAsset)Resources.Load(_helper.GetFilePath(), typeof(TextAsset));
        DataDefinition _dataBlock = JsonUtility.FromJson<DataDefinition>(json.ToString());

        _helper.Convert(_dataBlock);

        return _helper;
    }
}
