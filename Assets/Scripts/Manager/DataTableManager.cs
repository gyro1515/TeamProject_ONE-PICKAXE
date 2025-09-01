using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

// 모든 데이터 클래스가 구현해야 할 인터페이스
public interface IData
{
    int ID { get; }
}

public class DataTableManager : SingletonMono<DataTableManager>
{
    private Dictionary<Type, object> _singleData = new Dictionary<Type, object>();
    private Dictionary<Type, object> _collectionData = new Dictionary<Type, object>();
    private const string DATA_PATH = "GeneratedJson";

    // 단일 객체 데이터 로드 (예: PlayerData)
    public void LoadSingleData<T>() where T : class
    {
        string fileName = typeof(T).Name;
        TextAsset jsonAsset = Resources.Load<TextAsset>($"{DATA_PATH}/{fileName}");
        if (jsonAsset == null)
        { 
            Debug.LogWarning($"[DataTableManager] {fileName} not found in Resources/{DATA_PATH}");
            return; 
        }

        T instance = JsonUtility.FromJson<T>(jsonAsset.text);
        _singleData[typeof(T)] = instance;
    }

    // 목록형 데이터 로드 (예: ItemDataTable)
    public void LoadCollectionData<TTable>()
    {
        //테이블 클래스 이름에서 "Table"을 제거하여 원본 데이터 클래스 이름을 얻음
        // 예: "ItemDataTable" -> "ItemData"
        string dataClassName = typeof(TTable).Name.Replace("Table", "");

        // 추정한 데이터 클래스 이름을 기반으로 List 필드 이름을 정확하게 생성
        // 예: "ItemData" -> "itemdata"
        string listFieldName = $"{dataClassName.ToLower()}";
        FieldInfo listField = typeof(TTable).GetField(listFieldName);
        if (listField == null)
        {
            Debug.LogError($"[DataTableManager] Field '{listFieldName}' not found in '{typeof(TTable).Name}'.");
            return;
        }

        // List<T>에서 실제 데이터 타입(T) 추출
        Type dataType = listField.FieldType.GetGenericArguments()[0];

        // JSON 파일 이름도 원본 데이터 클래스 이름 사용
        string fileName = dataClassName;
        TextAsset jsonAsset = Resources.Load<TextAsset>($"{DATA_PATH}/{fileName}");
        if (jsonAsset == null)
        { 
            Debug.LogWarning($"[DataTableManager] {fileName} not found in Resources/{DATA_PATH}");
            return; 
        }

        TTable tableObject = JsonUtility.FromJson<TTable>(jsonAsset.text);
        IEnumerable<IData> dataList = (IEnumerable<IData>)listField.GetValue(tableObject);

        var newTable = new Dictionary<int, IData>();
        foreach (IData data in dataList)
        {
            newTable[data.ID] = data;
        }

        _collectionData[dataType] = newTable;
    }

    // 단일 객체 데이터 가져오기
    public T GetSingleData<T>() where T : class
    {
        _singleData.TryGetValue(typeof(T), out object data);
        return data as T;
    }

    // 목록형 데이터 가져오기
    public T GetCollectionData<T>(int id) where T : class, IData
    {
        if (_collectionData.TryGetValue(typeof(T), out object table))
        {
            var dataTable = (Dictionary<int, IData>)table;
            dataTable.TryGetValue(id, out IData data);
            return data as T;
        }
        return null;
    }
}
